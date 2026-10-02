using System.Collections;
using NUnit.Framework;
using TitansOfTheSea.Core;
using TitansOfTheSea.Ship;
using TitansOfTheSea.World;
using UnityEngine;
using UnityEngine.TestTools;

namespace TitansOfTheSea.Tests
{
    public sealed class BuoyancyStabilityTests
    {
        [UnityTest, Timeout(420000)] public IEnumerator FlatAndSineWaterRemainStableForFiveMinutes()
        {
            Services.ResetForNewSession();
            float previousTimeScale = Time.timeScale;
            Time.timeScale = 10f; // Five simulated minutes; fixed-step physics still runs every step.
            var oceanObject = new GameObject("testOcean");
            var hull = new GameObject("testHull");
            hull.SetActive(false);
            var settings = ScriptableObject.CreateInstance<BuoyancySettings>();
            try
            {
                var ocean = oceanObject.AddComponent<FlatOcean>();
                hull.transform.position = new Vector3(0, 2, 0);
                hull.AddComponent<BoxCollider>().size = new Vector3(3, 1.5f, 6);
                var body = hull.AddComponent<Rigidbody>();
                hull.AddComponent<ShipBuoyancy>().Configure(settings);
                hull.SetActive(true);
                float start = Time.time;
                float highestSpeed = 0f;
                while (Time.time - start < 300f)
                {
                    ocean.Waves = Time.time - start >= 150f;
                    yield return new WaitForFixedUpdate();
                    Assert.That(float.IsNaN(body.position.y) || float.IsInfinity(body.position.y), Is.False);
                    Assert.That(body.position.y, Is.InRange(-2f, 4f));
                    Assert.That(body.linearVelocity.magnitude, Is.LessThan(15f));
                    Assert.That(Vector3.Dot(hull.transform.up, Vector3.up), Is.GreaterThan(0.4f));
                    highestSpeed = Mathf.Max(highestSpeed, body.linearVelocity.magnitude);
                    if (Time.time - start > 30f && !ocean.Waves)
                        Assert.That(body.position.y, Is.EqualTo(0.125f).Within(0.2f));
                }
                Debug.Log("Five-minute flat/sine stability completed. Peak speed: " + highestSpeed);
            }
            finally
            {
                Time.timeScale = previousTimeScale;
                Object.Destroy(hull); Object.Destroy(oceanObject); Object.Destroy(settings);
                Services.ResetForNewSession();
            }
        }
    }
}
