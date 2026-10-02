using System;
using System.Collections.Generic;
using TitansOfTheSea.World;

internal static class Program
{
    private static int Main()
    {
        var clock = new TimeOfDayClock(1440, 6, 18, 1, 23);
        var days = new List<int>();
        clock.NewDay += days.Add;
        clock.Advance(180);
        Check(clock.DayNumber == 2 && Math.Abs(clock.Hour - 2) < .0001, "Midnight rollover");
        Check(days.Count == 1 && days[0] == 2, "New-day event");
        clock.Advance(2880);
        Check(clock.DayNumber == 4 && days.Count == 3, "Multiple skipped days");
        clock.Restore(10, 6); Check(!clock.IsNight && days.Count == 3, "Dawn/restore");
        clock.Restore(10, 18); Check(clock.IsNight, "Dusk boundary");
        clock.Advance(0); Check(clock.Hour == 18, "Paused clock");
        bool rejected = false;
        try { clock.Advance(double.NaN); } catch (ArgumentOutOfRangeException) { rejected = true; }
        Check(rejected && clock.Hour == 18, "Invalid advance preserves state");
        var reverse = new TimeOfDayClock(24, 18, 6, 1, 0);
        Check(!reverse.IsNight, "Wrapped daylight interval");
        var fine = new TimeOfDayClock(1440, 6, 18, 1, 8);
        for (int i = 0; i < 72000; i++) fine.Advance(.02);
        Check(fine.DayNumber == 2 && Math.Abs(fine.Hour - 8) < .001, "One full day of small timesteps");
        Console.WriteLine("PASS: 9 clock scenarios, executing the actual engine-independent C# clock source.");
        return 0;
    }
    private static void Check(bool condition, string label)
    {
        if (!condition) throw new Exception("FAIL: " + label);
    }
}
