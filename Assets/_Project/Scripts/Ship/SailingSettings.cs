using UnityEngine;

namespace TitansOfTheSea.Ship
{
    [CreateAssetMenu(menuName = "Titans/Person B/Sailing Settings")]
    public sealed class SailingSettings : ScriptableObject
    {
        [Min(1)] public float SailArea = 35;
        [Min(0)] public float ThrustCoefficient = 4;
        [Min(.1f)] public float MaximumSpeed = 12;
        [Min(0)] public float ForwardDrag = 70;
        [Min(0)] public float SideDrag = 600;
        [Min(0)] public float RudderAcceleration = .8f;
        [Min(0)] public float TurnDamping = 1.5f;
        [Min(.01f)] public float SailRaiseSpeed = .25f;
        [Min(0)] public float SailTurnSpeed = 30;
        [Range(0, 90)] public float MaximumSailAngle = 80;
        [Min(.1f)] public float AnchorDeceleration = 5;
    }
}
