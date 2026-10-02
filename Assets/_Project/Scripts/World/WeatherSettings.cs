using TitansOfTheSea.Contracts;
using UnityEngine;

namespace TitansOfTheSea.World
{
    [CreateAssetMenu(menuName = "Titans of the Sea/B/Weather Settings")]
    public sealed class WeatherSettings : ScriptableObject
    {
        public int Seed = 1729;
        public WeatherType InitialWeather = WeatherType.Clear;
        public Vector2 InitialWindDirection = Vector2.right;
        [Min(0)] public float TransitionSeconds = 30f;
        [Min(1)] public float MinWeatherSeconds = 180f;
        [Min(1)] public float MaxWeatherSeconds = 360f;
        [Min(1)] public float MinLightningSeconds = 8f;
        [Min(1)] public float MaxLightningSeconds = 20f;
        [Range(0.01f,1)] public float LightningThreshold = 0.5f;
        public WeatherProfile[] Profiles = {
            new WeatherProfile { Type = WeatherType.Clear, WindSpeed = 6, WaveScale = 0.6f },
            new WeatherProfile { Type = WeatherType.Cloudy, WindSpeed = 9, Fog = 0.1f, WaveScale = 0.9f },
            new WeatherProfile { Type = WeatherType.Fog, WindSpeed = 3, Fog = 0.8f, WaveScale = 0.4f },
            new WeatherProfile { Type = WeatherType.Rain, WindSpeed = 12, Rain = 0.65f, Fog = 0.25f, WaveScale = 1.3f },
            new WeatherProfile { Type = WeatherType.Storm, WindSpeed = 22, Rain = 1, Fog = 0.4f, StormIntensity = 1, WaveScale = 2.2f }
        };
    }
}
