using System;
using NUnit.Framework;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.World;
using UnityEngine;

namespace TitansOfTheSea.Tests
{
    public sealed class TimeWeatherTests
    {
        [Test] public void ClockEmitsEveryCrossedDayAndRestoresWithoutEvents()
        {
            var clock = new TimeOfDayClock(24, 6, 18, 1, 23);
            var days = new System.Collections.Generic.List<int>();
            clock.NewDay += days.Add;
            clock.Advance(50);
            CollectionAssert.AreEqual(new[] { 2, 3, 4 }, days);
            Assert.That(clock.Hour, Is.EqualTo(1).Within(.0001));
            clock.Restore(10, 8);
            Assert.That(days.Count, Is.EqualTo(3));
            Assert.Throws<ArgumentOutOfRangeException>(() => clock.Advance(double.NaN));
        }
        [Test] public void DawnDuskAndZeroElapsedTimeHaveDefinedBehaviour()
        {
            var clock = new TimeOfDayClock(1440, 6, 18, 1, 6);
            Assert.That(clock.IsNight, Is.False);
            clock.Restore(1, 18); Assert.That(clock.IsNight, Is.True);
            clock.Advance(0); Assert.That(clock.Hour, Is.EqualTo(18));
            clock.Restore(1, 5.99f); Assert.That(clock.IsNight, Is.True);
        }
        [Test] public void InterruptedWeatherTransitionsStartFromCurrentValues()
        {
            var settings = ScriptableObject.CreateInstance<WeatherSettings>();
            try
            {
                settings.TransitionSeconds = 10;
                var weather = new WeatherSimulation(settings) { Automatic = false };
                weather.ForceWeather(WeatherType.Storm);
                weather.Advance(5);
                Assert.That(weather.Current.Rain, Is.EqualTo(.5f).Within(.001));
                weather.ForceWeather(WeatherType.Clear);
                weather.Advance(5);
                Assert.That(weather.Current.Rain, Is.EqualTo(.25f).Within(.001));
                weather.SetWind(Vector2.left, 30); weather.Advance(5);
                Assert.That(weather.Current.WindDirection.magnitude, Is.EqualTo(1).Within(.001));
            }
            finally { UnityEngine.Object.DestroyImmediate(settings); }
        }
        [Test] public void SeededSchedulesRepeatAndLightningRequiresStormIntensity()
        {
            var settings = ScriptableObject.CreateInstance<WeatherSettings>();
            try
            {
                settings.TransitionSeconds = 0; settings.MinWeatherSeconds = settings.MaxWeatherSeconds = 1;
                var a = new WeatherSimulation(settings); var b = new WeatherSimulation(settings);
                for (int i = 0; i < 50; i++)
                {
                    a.Advance(1); b.Advance(1);
                    Assert.That(a.Current.Type, Is.EqualTo(b.Current.Type));
                    Assert.That(a.Current.WindDirection, Is.EqualTo(b.Current.WindDirection));
                }
                var controlled = new WeatherSimulation(settings) { Automatic = false };
                int strikes = 0; controlled.LightningStrike += () => strikes++;
                controlled.Advance(100); Assert.That(strikes, Is.Zero);
                controlled.ForceWeather(WeatherType.Storm, true); controlled.Advance(100);
                Assert.That(strikes, Is.GreaterThan(0));
                Assert.That(controlled.Current.WindSpeed, Is.InRange(0, 30));
            }
            finally { UnityEngine.Object.DestroyImmediate(settings); }
        }
        [Test] public void InstantWeatherRemainsInstantOnFollowingFrames()
        {
            var settings = ScriptableObject.CreateInstance<WeatherSettings>();
            try
            {
                settings.TransitionSeconds = 30;
                var weather = new WeatherSimulation(settings) { Automatic = false };
                weather.ForceWeather(WeatherType.Storm, true);
                weather.Advance(1);
                Assert.That(weather.Current.StormIntensity, Is.EqualTo(1));
                Assert.That(weather.Current.Rain, Is.EqualTo(1));
            }
            finally { UnityEngine.Object.DestroyImmediate(settings); }
        }
    }
}
