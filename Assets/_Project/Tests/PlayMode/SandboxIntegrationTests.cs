using System.Collections;
using NUnit.Framework;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using TitansOfTheSea.World;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TitansOfTheSea.Tests
{
    public sealed class SandboxIntegrationTests
    {
        [UnityTest] public IEnumerator GeneratedSandboxConnectsClockWeatherAndOcean()
        {
            yield return SceneManager.LoadSceneAsync("B_Sandbox_Buoyancy", LoadSceneMode.Single);
            yield return null;
            var clock = Services.Get<ITimeOfDay>() as TimeOfDayService;
            var weather = Services.Get<IWeatherService>() as WeatherService;
            var ocean = Services.Get<IOceanSurface>() as FlatOcean;
            Assert.That(clock, Is.Not.Null);
            Assert.That(weather, Is.Not.Null);
            Assert.That(ocean, Is.Not.Null);
            int days = 0; int changes = 0;
            System.Action<int> onDay = _ => days++;
            System.Action<WeatherSnapshot> onWeather = _ => changes++;
            clock.NewDay += onDay; weather.Changed += onWeather;
            try
            {
                clock.SetHour(23.9f); clock.Speed = 60;
                int initialDay = clock.DayNumber;
                yield return new WaitForSeconds(.25f);
                Assert.That(clock.DayNumber, Is.EqualTo(initialDay + 1));
                Assert.That(days, Is.EqualTo(1));
                weather.Automatic = false; weather.ForceWeather(WeatherType.Storm, true);
                Assert.That(changes, Is.GreaterThan(0));
                Assert.That(weather.Current.StormIntensity, Is.EqualTo(1));
                ocean.Waves = true;
                Assert.That(ocean.GetNormal(Vector3.zero).magnitude, Is.EqualTo(1).Within(.001));
                clock.Paused = weather.Paused = true;
                float hour = clock.Hour;
                var snapshot = weather.Current;
                yield return new WaitForSeconds(.1f);
                Assert.That(clock.Hour, Is.EqualTo(hour));
                Assert.That(weather.Current.WindSpeed, Is.EqualTo(snapshot.WindSpeed));
            }
            finally
            {
                clock.NewDay -= onDay; weather.Changed -= onWeather;
                clock.Speed = 1; clock.Paused = weather.Paused = false;
            }
        }
    }
}
