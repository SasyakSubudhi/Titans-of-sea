using NUnit.Framework;
using TitansOfTheSea.Ship;
using UnityEngine;

namespace TitansOfTheSea.Tests
{
    public sealed class SailingTests
    {
        [Test] public void WindAndSailControlsRespectNoGoAndReefing()
        {
            float running = SailingMath.Thrust(Vector3.forward, Vector2.up, 8, 1, 0, 35, 4);
            Assert.That(running, Is.GreaterThan(0));
            Assert.That(SailingMath.Thrust(Vector3.forward, Vector2.up, 8, .5f, 0, 35, 4), Is.EqualTo(running * .5f).Within(.01));
            Assert.That(SailingMath.Thrust(Vector3.forward, Vector2.down, 8, 1, 0, 35, 4), Is.Zero);
            Assert.That(SailingMath.Thrust(Vector3.forward, Vector2.up, 8, 0, 0, 35, 4), Is.Zero);
            Assert.That(SailingMath.Thrust(Vector3.forward, Vector2.zero, 8, 1, 0, 35, 4), Is.Zero);
            Assert.That(SailingMath.PointOfSail(70), Is.GreaterThan(SailingMath.PointOfSail(0)));
            Assert.That(SailingMath.Thrust(Vector3.forward, Vector2.up, 8, 1, 80, 35, 4), Is.LessThan(running));
        }
        [Test] public void AnchorCannotReverseVelocityOrCancelBuoyancy()
        {
            var acceleration = SailingMath.AnchorAcceleration(new Vector3(.02f, 3, 0), 5, .02f);
            Assert.That(acceleration.y, Is.Zero);
            Assert.That(.02f + acceleration.x * .02f, Is.EqualTo(0).Within(.0001));
            Assert.That(SailingMath.AnchorAcceleration(Vector3.zero, 5, .02f), Is.EqualTo(Vector3.zero));
        }
    }
}
