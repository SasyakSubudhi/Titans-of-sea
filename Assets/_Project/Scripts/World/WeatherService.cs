using System;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using UnityEngine;

namespace TitansOfTheSea.World
{
    [DefaultExecutionOrder(-800)]
    public sealed class WeatherService : MonoBehaviour, IWeatherService
    {
        [SerializeField] private WeatherSettings _settings;
        private WeatherSimulation _simulation;
        public bool Paused { get; set; }
        public WeatherSnapshot Current => _simulation != null ? _simulation.Current : default;
        public bool Automatic { get => _simulation.Automatic; set => _simulation.Automatic = value; }
        public event Action<WeatherSnapshot> Changed;
        public event Action LightningStrike;
        public void Configure(WeatherSettings settings)
        {
            _settings = settings;
            if (Application.isPlaying) Initialize();
        }
        private void Awake() => Initialize();
        private void Initialize()
        {
            if (_simulation != null) _simulation.LightningStrike -= RelayLightning;
            _simulation = new WeatherSimulation(_settings);
            _simulation.LightningStrike += RelayLightning;
        }
        private void OnEnable() => Services.Register<IWeatherService>(this);
        private void OnDisable() => Services.Unregister<IWeatherService>(this);
        private void Update()
        {
            if (Paused || Time.deltaTime <= 0) return;
            _simulation.Advance(Time.deltaTime);
            Changed?.Invoke(Current);
        }
        private void RelayLightning() => LightningStrike?.Invoke();
        public void ForceWeather(WeatherType type, bool instant = false)
        {
            _simulation.ForceWeather(type, instant);
            Changed?.Invoke(Current);
        }
        public void SetWind(Vector2 direction, float speed) => _simulation.SetWind(direction, speed);
    }
}
