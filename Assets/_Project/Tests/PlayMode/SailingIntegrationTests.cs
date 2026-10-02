using System.Collections;
using NUnit.Framework;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using TitansOfTheSea.Ship;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace TitansOfTheSea.Tests
{
    public sealed class SailingIntegrationTests
    {
        private sealed class Wind : IWeatherService
        {
            public WeatherSnapshot Current => new WeatherSnapshot { WindDirection = Vector2.up, WindSpeed = 8 };
            public event System.Action<WeatherSnapshot> Changed { add { } remove { } }
            public event System.Action LightningStrike { add { } remove { } }
        }
        [UnityTest] public IEnumerator SloopSailsTurnsAndStopsWithoutLosingFlotation()
        {
            yield return SceneManager.LoadSceneAsync("B_Sandbox_Sailing");
            yield return null;
            var old = Services.Get<IWeatherService>();
            Services.Unregister(old);
            var wind = new Wind(); Services.Register<IWeatherService>(wind);
            var ship = Object.FindFirstObjectByType<ShipSailing>();
            var body = ship.GetComponent<Rigidbody>();
            float previousScale = Time.timeScale;
            try
            {
                Time.timeScale = 5;
                ship.SetSail(1, 0); ship.SetAnchor(false);
                yield return new WaitForSeconds(8);
                Assert.That(body.position.z, Is.GreaterThan(5), "Ship should travel with favourable wind.");
                Assert.That(Vector3.ProjectOnPlane(ship.Velocity, Vector3.up).magnitude, Is.GreaterThan(1));
                float heading = ship.transform.eulerAngles.y;
                ship.ApplyControls(new PlayerInputState { SteerAxis = .5f }, .02f);
                yield return new WaitForSeconds(2);
                Assert.That(Mathf.Abs(Mathf.DeltaAngle(heading, ship.transform.eulerAngles.y)), Is.GreaterThan(3));
                ship.ApplyControls(default, .02f);
                ship.SetAnchor(true);
                yield return new WaitForSeconds(4);
                Assert.That(Vector3.ProjectOnPlane(ship.Velocity, Vector3.up).magnitude, Is.LessThan(.15));
                Assert.That(body.position.y, Is.InRange(-1.5f, 1.5f), "Anchor must preserve flotation.");
                Assert.That(Vector3.Dot(ship.transform.up, Vector3.up), Is.GreaterThan(.7));
                var session = Services.Get<GameSession>();
                session.Transition(GameState.Paused);
                var stoppedAt = body.position;
                var hour = Services.Get<ITimeOfDay>().Hour;
                yield return new WaitForSecondsRealtime(.1f);
                Assert.That(body.position, Is.EqualTo(stoppedAt));
                Assert.That(Services.Get<ITimeOfDay>().Hour, Is.EqualTo(hour));
                Assert.That(Services.Get<IPlayerInput>().Enabled, Is.False);
                session.Transition(GameState.Playing);
                Assert.That(Services.Get<IPlayerInput>().Enabled, Is.True);
            }
            finally
            {
                Time.timeScale = previousScale;
                Services.Unregister<IWeatherService>(wind);
                Services.Register<IWeatherService>(old);
            }
        }
    }
}
