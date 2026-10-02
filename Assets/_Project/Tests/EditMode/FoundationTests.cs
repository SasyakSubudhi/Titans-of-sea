using System;
using NUnit.Framework;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using TitansOfTheSea.Ship;
using TitansOfTheSea.World;
using UnityEngine;

namespace TitansOfTheSea.Tests
{
    public sealed class FoundationTests
    {
        private sealed class OceanStub : IOceanSurface
        {
            public float GetHeight(Vector3 position) => 0;
            public Vector3 GetNormal(Vector3 position) => Vector3.up;
            public void GetHeights(Vector3[] positions, float[] output, int count) { for (int i = 0; i < count; i++) output[i] = 0; }
        }
        [SetUp] public void Before() { Services.ResetForNewSession(); GameEvents.ResetForNewSession(); }
        [TearDown] public void After() { Services.ResetForNewSession(); GameEvents.ResetForNewSession(); }
        [Test] public void RealProviderReplacesMockAndMockCannotRemoveIt()
        {
            var mock = new OceanStub(); var real = new OceanStub();
            Assert.That(Services.RegisterFallback<IOceanSurface>(mock), Is.True);
            Services.Register<IOceanSurface>(real);
            Assert.That(Services.Unregister<IOceanSurface>(mock), Is.False);
            Assert.That(Services.Get<IOceanSurface>(), Is.SameAs(real));
            Assert.Throws<InvalidOperationException>(() => Services.Register<IOceanSurface>(new OceanStub()));
        }
        [Test] public void SessionResetRemovesServicesAndListeners()
        {
            int calls = 0;
            GameEvents.OnShipSunk += _ => calls++;
            GameEvents.RaiseShipSunk("test");
            GameEvents.ResetForNewSession();
            GameEvents.RaiseShipSunk("test");
            Assert.That(calls, Is.EqualTo(1));
            Services.Register<IOceanSurface>(new OceanStub()); Services.ResetForNewSession();
            Assert.That(Services.TryGet<IOceanSurface>(out _), Is.False);
        }
        [Test] public void SupportIsZeroAboveWaterAndBalancesWeightAtHalfDepth()
        {
            Assert.That(BuoyancyMath.SupportAcceleration(-1, .75f, 2, 2, 0, 9.81f), Is.Zero);
            Assert.That(BuoyancyMath.SupportAcceleration(.375f, .75f, 2, 2, 0, 9.81f), Is.EqualTo(9.81f).Within(.0001f));
            Assert.That(BuoyancyMath.SupportAcceleration(1, .75f, 2, 2, 100, 9.81f), Is.Zero);
        }
        [Test] public void OceanBatchMatchesScalarAndRejectsInvalidCount()
        {
            var go = new GameObject("testOcean");
            try
            {
                var ocean = go.AddComponent<FlatOcean>(); ocean.Waves = true;
                var positions = new[] { Vector3.zero, new Vector3(3, 5, 2) }; var heights = new float[2];
                ocean.GetHeights(positions, heights, 2);
                for (int i = 0; i < 2; i++) Assert.That(heights[i], Is.EqualTo(ocean.GetHeight(positions[i])).Within(.0001f));
                Assert.Throws<ArgumentOutOfRangeException>(() => ocean.GetHeights(positions, heights, 3));
                Assert.That(ocean.GetNormal(Vector3.zero).magnitude, Is.EqualTo(1).Within(.0001f));
            }
            finally { UnityEngine.Object.DestroyImmediate(go); }
        }
    }
}
