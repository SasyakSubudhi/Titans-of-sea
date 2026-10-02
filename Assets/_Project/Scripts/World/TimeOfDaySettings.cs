using UnityEngine;

namespace TitansOfTheSea.World
{
    [CreateAssetMenu(menuName = "Titans of the Sea/B/Time of Day Settings")]
    public sealed class TimeOfDaySettings : ScriptableObject
    {
        [Min(1)] public float RealSecondsPerDay = 1440f;
        [Range(0, 23.99f)] public float StartHour = 8f;
        [Min(1)] public int StartDay = 1;
        [Range(0, 23.99f)] public float DawnHour = 6f;
        [Range(0, 23.99f)] public float DuskHour = 18f;
    }
}
