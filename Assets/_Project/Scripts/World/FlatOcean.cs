using System;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using UnityEngine;

namespace TitansOfTheSea.World
{
    /// Sandbox sampling only; does not render or replace A's ocean.
    [DefaultExecutionOrder(-900)]
    public sealed class FlatOcean : MonoBehaviour, IOceanSurface
    {
        [SerializeField] private float _height;
        [SerializeField] private bool _waves;
        [SerializeField, Min(0)] private float _amplitude = 0.35f;
        [SerializeField, Min(0.1f)] private float _wavelength = 12f;
        [SerializeField, Min(0)] private float _speed = 1f;
        [SerializeField] private bool _followWeatherWaveScale = true;
        private IWeatherService _weather;
        private float Amplitude => _amplitude * (_followWeatherWaveScale && _weather != null ? _weather.Current.WaveScale : 1f);
        public bool Waves { get => _waves; set => _waves = value; }
        public float Height { get => _height; set => _height = value; }
        private void OnEnable()
        {
            Services.Changed += OnServicesChanged;
            Services.RegisterFallback<IOceanSurface>(this);
            Services.TryGet(out _weather);
        }
        private void OnDisable()
        {
            Services.Changed -= OnServicesChanged;
            Services.Unregister<IOceanSurface>(this);
        }
        private void OnServicesChanged(Type type)
        {
            if (type == typeof(IWeatherService)) Services.TryGet(out _weather);
            if (type == typeof(IOceanSurface) && !Services.TryGet<IOceanSurface>(out _))
                Services.RegisterFallback<IOceanSurface>(this);
        }
        public float GetHeight(Vector3 worldPos) => SampleHeight(worldPos, Time.fixedTime);
        public float SampleHeight(Vector3 position, float time)
        {
            float phase = 2f * Mathf.PI / Mathf.Max(0.1f, _wavelength);
            return _height + (_waves ? Amplitude * Mathf.Sin(phase * position.x + _speed * time) : 0f);
        }
        public Vector3 GetNormal(Vector3 worldPos)
        {
            if (!_waves) return Vector3.up;
            float phase = 2f * Mathf.PI / Mathf.Max(0.1f, _wavelength);
            float slope = Amplitude * phase * Mathf.Cos(phase * worldPos.x + _speed * Time.fixedTime);
            return new Vector3(-slope, 1f, 0f).normalized;
        }
        public void GetHeights(Vector3[] worldPositions, float[] heightsOut, int count)
        {
            if (worldPositions == null || heightsOut == null) throw new ArgumentNullException();
            if (count < 0 || count > worldPositions.Length || count > heightsOut.Length)
                throw new ArgumentOutOfRangeException(nameof(count));
            float time = Time.fixedTime;
            for (int i = 0; i < count; i++) heightsOut[i] = SampleHeight(worldPositions[i], time);
        }
    }
}
