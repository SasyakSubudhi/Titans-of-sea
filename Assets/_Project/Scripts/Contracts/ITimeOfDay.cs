namespace TitansOfTheSea.Contracts
{
    public interface ITimeOfDay
    {
        float Hour { get; }          // 0..24
        int DayNumber { get; }
        bool IsNight { get; }
        float DayProgress01 { get; } // 0..1
        event System.Action<int> NewDay;
    }
}
