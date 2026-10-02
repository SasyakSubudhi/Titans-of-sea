using System;
using TitansOfTheSea.Contracts;
using TitansOfTheSea.Core;
using UnityEngine;

namespace TitansOfTheSea.World
{
    [DefaultExecutionOrder(-800)]
    public sealed class TimeOfDayService : MonoBehaviour, ITimeOfDay
    {
        [SerializeField] private TimeOfDaySettings _settings;
        private TimeOfDayClock _clock;
        private float _speed = 1f;
        public bool Paused { get; set; }
        public float Hour => _clock != null ? _clock.Hour : 0;
        public int DayNumber => _clock != null ? _clock.DayNumber : 1;
        public bool IsNight => _clock != null && _clock.IsNight;
        public float DayProgress01 => _clock != null ? _clock.DayProgress01 : 0;
        public event Action<int> NewDay;
        public float Speed
        {
            get => _speed;
            set
            {
                if (float.IsNaN(value) || float.IsInfinity(value) || value < 0 || value > 1000)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _speed = value;
            }
        }
        public void Configure(TimeOfDaySettings settings)
        {
            _settings = settings;
            if (Application.isPlaying) Initialize();
        }
        private void Awake() => Initialize();
        private void Initialize()
        {
            if (_settings == null) throw new InvalidOperationException("TimeOfDayService requires a TimeOfDaySettings asset.");
            if (_clock != null) _clock.NewDay -= RelayNewDay;
            _clock = new TimeOfDayClock(_settings.RealSecondsPerDay, _settings.DawnHour, _settings.DuskHour,
                _settings.StartDay, _settings.StartHour);
            _clock.NewDay += RelayNewDay;
        }
        private void OnEnable() => Services.Register<ITimeOfDay>(this);
        private void OnDisable() => Services.Unregister<ITimeOfDay>(this);
        private void Update()
        {
            if (!Paused && (!Services.TryGet<GameSession>(out var session) || session.AdvancesWorldTime))
                _clock.Advance((double)Time.deltaTime * _speed);
        }
        private void RelayNewDay(int day) => NewDay?.Invoke(day);
        public void SetHour(float hour) => _clock.Restore(DayNumber, hour);
    }
}
