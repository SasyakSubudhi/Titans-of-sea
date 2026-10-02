using System.Collections;
using NUnit.Framework;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using TitansOfTheSea.Player;
using TitansOfTheSea.Ship;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
namespace TitansOfTheSea.Tests
{
    public sealed class PlayerIntegrationTests
    {
        private sealed class TestInput : IPlayerInput
        {
            public PlayerInputState Current { get; set; }
            public bool Enabled { get; set; } = true;
        }
        [UnityTest] public IEnumerator InputServiceMovesPlayerAndActualSailingDeckCarriesThem()
        {
            yield return SceneManager.LoadSceneAsync("B_Sandbox_Player"); yield return null;
            var motor = Object.FindFirstObjectByType<PlayerMotor>();
            var oldInput = Services.Get<IPlayerInput>();
            var input = new TestInput(); Services.Register<IPlayerInput>(input);
            try
            {
                motor.Teleport(new Vector3(8, .6f, -3));
                input.Current = new PlayerInputState { Move = Vector2.up };
                float z = motor.transform.position.z;
                yield return new WaitForSeconds(.4f);
                Assert.That(motor.transform.position.z, Is.GreaterThan(z + 1));
                input.Enabled = false; z = motor.transform.position.z;
                yield return new WaitForSeconds(.2f);
                Assert.That(motor.transform.position.z, Is.EqualTo(z).Within(.05));
                input.Current = default; input.Enabled = true;
                var ship = Object.FindFirstObjectByType<ShipSailing>();
                motor.Teleport(ship.transform.TransformPoint(new Vector3(0, .85f, -1)));
                yield return new WaitForSeconds(2);
                Assert.That(motor.Platform, Is.EqualTo(ship.transform));
                Vector3 local = ship.transform.InverseTransformPoint(motor.transform.position);
                var weather = Services.Get<IWeatherService>() as TitansOfTheSea.World.WeatherService;
                weather.Automatic = false; weather.SetWind(Vector2.up, 8);
                (Services.Get<IOceanSurface>() as TitansOfTheSea.World.FlatOcean).Waves = true;
                ship.SetSail(1, 0); ship.SetAnchor(false);
                Vector3 initial = motor.transform.position;
                yield return new WaitForSeconds(3);
                Assert.That(Vector3.Distance(initial, motor.transform.position), Is.GreaterThan(1));
                Assert.That(Vector3.Distance(local, ship.transform.InverseTransformPoint(motor.transform.position)), Is.LessThan(.5));
                Assert.That(motor.Grounded, Is.True);
                var session = Services.Get<GameSession>(); session.Transition(GameState.Cutscene);
                local = ship.transform.InverseTransformPoint(motor.transform.position);
                yield return new WaitForSeconds(.3f);
                Assert.That(input.Enabled, Is.False);
                Assert.That(Vector3.Distance(local, ship.transform.InverseTransformPoint(motor.transform.position)), Is.LessThan(.5));
                session.Transition(GameState.Playing);
            }
            finally { Services.Unregister<IPlayerInput>(input); Services.RegisterFallback<IPlayerInput>(oldInput); }
        }
        private static void Step(PlayerMotor motor, PlayerInputState input, int count)
        { for (int i = 0; i < count; i++) { Physics.SyncTransforms(); motor.Simulate(input, .02f); } }
        [UnityTest] public IEnumerator PlayerFollowsTranslatingRotatingDeckAndJumpsFree()
        {
            yield return SceneManager.LoadSceneAsync("B_Sandbox_Player"); yield return null;
            var motor = Object.FindFirstObjectByType<PlayerMotor>(); motor.AutomaticSimulation = false;
            var ship = Object.FindFirstObjectByType<ShipSailing>(); ship.enabled = false;
            ship.GetComponent<ShipBuoyancy>().enabled = false;
            var body = ship.GetComponent<Rigidbody>(); body.isKinematic = true; body.interpolation = RigidbodyInterpolation.None;
            ship.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            motor.Teleport(new Vector3(0, .8f, -1)); Step(motor, default, 20);
            Assert.That(motor.Platform, Is.EqualTo(ship.transform));
            Vector3 local = ship.transform.InverseTransformPoint(motor.transform.position);
            Vector3 beforeMove = motor.transform.position;
            ship.transform.SetPositionAndRotation(new Vector3(1, .1f, 1), Quaternion.Euler(0, 20, 0));
            Physics.SyncTransforms(); Step(motor, default, 1);
            Assert.That(Vector3.Distance(ship.transform.InverseTransformPoint(motor.transform.position), local), Is.LessThan(.12));
            Assert.That(Vector3.Distance(beforeMove, motor.transform.position), Is.GreaterThan(.5f));
            Assert.That(motor.transform.eulerAngles.y, Is.EqualTo(20).Within(1));
            float y = motor.transform.position.y;
            Step(motor, new PlayerInputState { JumpPressed = true }, 1); Step(motor, default, 8);
            Assert.That(motor.transform.position.y, Is.GreaterThan(y + .4f));
            Assert.That(motor.Platform, Is.Null);
        }
        [UnityTest] public IEnumerator PlayerWalksStepsSwimsDivesAndClimbs()
        {
            yield return SceneManager.LoadSceneAsync("B_Sandbox_Player"); yield return null;
            var motor = Object.FindFirstObjectByType<PlayerMotor>(); motor.AutomaticSimulation = false;
            motor.Teleport(new Vector3(8, .6f, 0)); Step(motor, default, 20);
            Assert.That(motor.Grounded, Is.True);
            float startZ = motor.transform.position.z, peak = 0;
            for (int i = 0; i < 36; i++)
            { Step(motor, new PlayerInputState { Move = Vector2.up }, 1); peak = Mathf.Max(peak, motor.transform.position.y); }
            Assert.That(motor.transform.position.z - startZ, Is.GreaterThan(2));
            Assert.That(peak, Is.GreaterThan(.64f), "Character should step over the .2m block.");
            motor.Teleport(new Vector3(-5, -.8f, 0)); Step(motor, default, 150);
            Assert.That(motor.Swimming, Is.True);
            Assert.That(motor.transform.position.y, Is.EqualTo(-.65f).Within(.12));
            Step(motor, new PlayerInputState { AimHeld = true }, 30);
            Assert.That(motor.transform.position.y, Is.LessThan(-1.5f));
            Step(motor, default, 200); Assert.That(motor.transform.position.y, Is.EqualTo(-.65f).Within(.15));
            motor.Teleport(new Vector3(8, .55f, 3.5f));
            Assert.That(motor.BeginClimb(new Vector3(8, .55f, 3.5f), new Vector3(8, 3.5f, 3.5f), new Vector3(9, 3.55f, 3.5f)), Is.True);
            Step(motor, new PlayerInputState { Move = Vector2.up }, 90);
            Assert.That(motor.transform.position.y, Is.GreaterThan(3.3f)); Assert.That(motor.Climbing, Is.False);
        }
        [UnityTest] public IEnumerator InteractionRoutesWheelRopeAndAnchorAndReleasesOnDisable()
        {
            yield return SceneManager.LoadSceneAsync("B_Sandbox_Player"); yield return null;
            var motor = Object.FindFirstObjectByType<PlayerMotor>(); motor.AutomaticSimulation = false;
            var actor = motor.GetComponent<PlayerInteractor>();
            var ship = Object.FindFirstObjectByType<ShipSailing>();
            var body = ship.GetComponent<Rigidbody>(); body.isKinematic = true; body.interpolation = RigidbodyInterpolation.None;
            ship.enabled = ship.GetComponent<ShipBuoyancy>().enabled = false;
            ship.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
            motor.Teleport(new Vector3(0, .8f, -1)); Physics.SyncTransforms(); actor.Scan();
            Assert.That(actor.Focused, Is.Not.Null); Assert.That(actor.TryInteract(), Is.True);
            Assert.That(motor.MovementLocked && ship.PlayerAtControls, Is.True);
            ship.ApplyControls(new PlayerInputState { SteerAxis = .8f, SailAxis = 1 }, 1);
            Assert.That(ship.WheelAngle01, Is.EqualTo(.8f)); Assert.That(ship.SailRaise01[0], Is.Zero);
            actor.TryInteract(); Assert.That(motor.MovementLocked, Is.False);
            motor.Teleport(new Vector3(-1, .8f, -2)); Physics.SyncTransforms(); actor.Scan(); Assert.That(actor.TryInteract(), Is.True);
            ship.ApplyControls(new PlayerInputState { SteerAxis = -.5f, SailAxis = 1 }, 1);
            Assert.That(ship.SailRaise01[0], Is.GreaterThan(0)); Assert.That(ship.WheelAngle01, Is.EqualTo(.8f));
            actor.enabled = false; Assert.That(ship.PlayerAtControls, Is.False); Assert.That(motor.MovementLocked, Is.False);
            actor.enabled = true; motor.Teleport(new Vector3(1, .8f, -2)); Physics.SyncTransforms(); actor.Scan();
            bool wasDown = ship.AnchorDown; Assert.That(actor.TryInteract(), Is.True); Assert.That(ship.AnchorDown, Is.Not.EqualTo(wasDown));
        }
    }
}
