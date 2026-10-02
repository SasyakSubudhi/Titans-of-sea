using System;

namespace TitansOfTheSea.World
{
    /// Engine-independent time model. One owner advances it; presentation reads its state.
    public sealed class TimeOfDayClock
    {
        private const double HOURS_PER_DAY = 24d;
        private readonly double _secondsPerDay;
        private readonly float _dawn;
        private readonly float _dusk;
        private double _hour;
        public float Hour => Math.Min((float)_hour, 23.999998f);
        public int DayNumber { get; private set; }
        public float DayProgress01 => Hour / (float)HOURS_PER_DAY;
        public bool IsNight => _dawn <= _dusk
            ? Hour < _dawn || Hour >= _dusk
            : Hour >= _dusk && Hour < _dawn;
        public event Action<int> NewDay;

        public TimeOfDayClock(double secondsPerDay, float dawn, float dusk, int day, float hour)
        {
            if (!Finite(secondsPerDay) || secondsPerDay <= 0) throw new ArgumentOutOfRangeException(nameof(secondsPerDay));
            ValidateHour(dawn); ValidateHour(dusk);
            _secondsPerDay = secondsPerDay; _dawn = dawn; _dusk = dusk;
            Restore(day, hour);
        }
        public void Advance(double realSeconds)
        {
            if (!Finite(realSeconds) || realSeconds < 0) throw new ArgumentOutOfRangeException(nameof(realSeconds));
            double total = _hour + realSeconds / _secondsPerDay * HOURS_PER_DAY;
            double crossed = Math.Floor(total / HOURS_PER_DAY);
            if (!Finite(total) || crossed > int.MaxValue - DayNumber) throw new ArgumentOutOfRangeException(nameof(realSeconds));
            _hour = total % HOURS_PER_DAY;
            for (int i = 0; i < (int)crossed; i++)
            {
                DayNumber++;
                NewDay?.Invoke(DayNumber);
            }
        }
        // Debug/restore does not synthesize elapsed-day events such as collector visits.
        public void Restore(int day, float hour)
        {
            if (day < 1) throw new ArgumentOutOfRangeException(nameof(day));
            ValidateHour(hour);
            DayNumber = day; _hour = hour;
        }
        private static bool Finite(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
        private static void ValidateHour(float hour)
        {
            if (!Finite(hour) || hour < 0 || hour >= HOURS_PER_DAY) throw new ArgumentOutOfRangeException(nameof(hour));
        }
    }
}
