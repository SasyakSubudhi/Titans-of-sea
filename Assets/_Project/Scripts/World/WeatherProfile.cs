using System;
using TitansOfTheSea.Contracts;
using UnityEngine;

namespace TitansOfTheSea.World
{
    [Serializable]
    public sealed class WeatherProfile
    {
        public WeatherType Type;
        [Range(0, 30)] public float WindSpeed = 6f;
        [Range(0, 1)] public float Rain;
        [Range(0, 1)] public float Fog;
        [Range(0, 1)] public float StormIntensity;
        [Range(0.3f, 2.5f)] public float WaveScale = 1f;
        public WeatherSnapshot Snapshot(Vector2 direction) => new WeatherSnapshot
        {
            Type = Type, WindDirection = direction.normalized,
            WindSpeed = Mathf.Clamp(WindSpeed, 0, 30), Rain = Mathf.Clamp01(Rain),
            Fog = Mathf.Clamp01(Fog), StormIntensity = Mathf.Clamp01(StormIntensity),
            WaveScale = Mathf.Clamp(WaveScale, 0.3f, 2.5f)
        };
    }
}
