using System;
using TitansOfTheSea.Contracts;
using UnityEngine;

namespace TitansOfTheSea.World
{
    /// Owns its random sequence; never touches UnityEngine.Random's shared state.
    public sealed class WeatherSimulation
    {
        private readonly WeatherSettings _settings;
        private readonly System.Random _random;
        private WeatherSnapshot _from;
        private WeatherSnapshot _target;
        private float _transitionElapsed;
        private float _untilWeather;
        private float _untilLightning;
        public WeatherSnapshot Current { get; private set; }
        public bool Automatic { get; set; } = true;
        public event Action LightningStrike;

        public WeatherSimulation(WeatherSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            if (settings.Profiles == null || settings.Profiles.Length == 0) throw new ArgumentException("Weather profiles are required.");
            if (!Finite(settings.TransitionSeconds) || settings.TransitionSeconds < 0 ||
                !Finite(settings.MinWeatherSeconds) || !Finite(settings.MaxWeatherSeconds) ||
                !Finite(settings.MinLightningSeconds) || !Finite(settings.MaxLightningSeconds) ||
                !Finite(settings.LightningThreshold) || settings.LightningThreshold <= 0 || settings.LightningThreshold > 1 ||
                !Finite(settings.InitialWindDirection.x) || !Finite(settings.InitialWindDirection.y))
                throw new ArgumentException("Weather timing, direction and lightning threshold must be finite; threshold must be greater than zero.");
            foreach (var profile in settings.Profiles)
                if (profile == null || !Finite(profile.WindSpeed) || !Finite(profile.Rain) || !Finite(profile.Fog) ||
                    !Finite(profile.StormIntensity) || !Finite(profile.WaveScale))
                    throw new ArgumentException("Weather profile values must be finite.");
            foreach (WeatherType type in Enum.GetValues(typeof(WeatherType)))
            {
                int matches = 0;
                foreach (var profile in settings.Profiles) if (profile != null && profile.Type == type) matches++;
                if (matches != 1) throw new ArgumentException("Provide exactly one profile per WeatherType: " + type);
            }
            _settings = settings; _random = new System.Random(settings.Seed);
            Vector2 direction = settings.InitialWindDirection.sqrMagnitude > 0.0001f ? settings.InitialWindDirection.normalized : Vector2.right;
            Current = Profile(settings.InitialWeather).Snapshot(direction);
            _from = _target = Current;
            _transitionElapsed = Mathf.Max(0, settings.TransitionSeconds);
            _untilWeather = WeatherInterval(); _untilLightning = LightningInterval();
        }
        public void ForceWeather(WeatherType type, bool instant = false)
        {
            _from = Current;
            _target = Profile(type).Snapshot(Current.WindDirection);
            _transitionElapsed = 0;
            _untilWeather = WeatherInterval();
            // Type identifies the destination while continuous values blend from the current state.
            var current = Current; current.Type = type; Current = current;
            if (instant || _settings.TransitionSeconds <= 0)
            {
                Current = _target;
                _from = _target;
                _transitionElapsed = Mathf.Max(0, _settings.TransitionSeconds);
            }
        }
        public void SetWind(Vector2 direction, float speed)
        {
            if (!Finite(direction.x) || !Finite(direction.y) || direction.sqrMagnitude < 0.0001f || !Finite(speed))
                throw new ArgumentException("Wind requires a finite nonzero direction and finite speed.");
            _from = Current; _target = Current;
            _target.WindDirection = direction.normalized; _target.WindSpeed = Mathf.Clamp(speed, 0, 30);
            _transitionElapsed = 0;
            if (_settings.TransitionSeconds <= 0) Current = _target;
        }
        public void Advance(float seconds)
        {
            if (!Finite(seconds) || seconds < 0) throw new ArgumentOutOfRangeException(nameof(seconds));
            if (seconds == 0) return;
            // Slice long advances at schedule boundaries, so no automatic changes are skipped.
            float remaining = seconds;
            while (remaining > 0)
            {
                float step = Automatic ? Mathf.Min(remaining, _untilWeather) : remaining;
                Blend(step);
                AdvanceLightning(step);
                remaining = Mathf.Max(0, remaining - step);
                if (Automatic)
                {
                    _untilWeather -= step;
                    if (_untilWeather <= 0)
                    {
                        var types = _settings.Profiles;
                        int choice = _random.Next(types.Length - 1);
                        int selected = 0;
                        for (int i = 0; i < types.Length; i++)
                        {
                            if (types[i].Type == Current.Type) continue;
                            if (choice-- == 0) { selected = i; break; }
                        }
                        ForceWeather(types[selected].Type);
                        float angle = (float)_random.NextDouble() * Mathf.PI * 2;
                        _target.WindDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                        if (_settings.TransitionSeconds <= 0) Current = _target;
                    }
                }
            }
        }
        private void Blend(float seconds)
        {
            _transitionElapsed += seconds;
            float fraction = _settings.TransitionSeconds <= 0 ? 1 : Mathf.Clamp01(_transitionElapsed / _settings.TransitionSeconds);
            float angleFrom = Mathf.Atan2(_from.WindDirection.y, _from.WindDirection.x) * Mathf.Rad2Deg;
            float angleTo = Mathf.Atan2(_target.WindDirection.y, _target.WindDirection.x) * Mathf.Rad2Deg;
            float angle = Mathf.LerpAngle(angleFrom, angleTo, fraction) * Mathf.Deg2Rad;
            Current = new WeatherSnapshot {
                Type = _target.Type, WindDirection = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)),
                WindSpeed = Mathf.Lerp(_from.WindSpeed, _target.WindSpeed, fraction),
                Rain = Mathf.Lerp(_from.Rain, _target.Rain, fraction), Fog = Mathf.Lerp(_from.Fog, _target.Fog, fraction),
                StormIntensity = Mathf.Lerp(_from.StormIntensity, _target.StormIntensity, fraction),
                WaveScale = Mathf.Lerp(_from.WaveScale, _target.WaveScale, fraction)
            };
        }
        private void AdvanceLightning(float seconds)
        {
            if (Current.StormIntensity < _settings.LightningThreshold) return;
            _untilLightning -= seconds;
            while (_untilLightning <= 0)
            {
                _untilLightning += LightningInterval();
                LightningStrike?.Invoke();
            }
        }
        private WeatherProfile Profile(WeatherType type)
        {
            foreach (var profile in _settings.Profiles) if (profile.Type == type) return profile;
            throw new ArgumentOutOfRangeException(nameof(type));
        }
        private float WeatherInterval() => Interval(_settings.MinWeatherSeconds, _settings.MaxWeatherSeconds);
        private float LightningInterval() => Interval(_settings.MinLightningSeconds, _settings.MaxLightningSeconds);
        private float Interval(float minimum, float maximum)
        {
            float low = Mathf.Max(1, minimum); float high = Mathf.Max(low, maximum);
            return low + (float)_random.NextDouble() * (high - low);
        }
        private static bool Finite(float value) => !float.IsNaN(value) && !float.IsInfinity(value);
    }
}
