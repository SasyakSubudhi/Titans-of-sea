namespace TitansOfTheSea.Contracts
{
    public interface IShipState
    {
        string ShipId { get; }
        float HullHealth01 { get; }                         // overall
        float WaterLevel01 { get; }                         // how flooded (0 dry, 1 sinking)
        System.Collections.Generic.IReadOnlyList<float> HullSectionHealth01 { get; }
        System.Collections.Generic.IReadOnlyList<float> SailRaise01 { get; }   // per sail
        System.Collections.Generic.IReadOnlyList<float> SailAngleDeg { get; }
        float WheelAngle01 { get; }                         // -1..1
        bool AnchorDown { get; }
        float SpeedMetersPerSec { get; }
        bool IsSinking { get; }
        UnityEngine.Vector3 Velocity { get; }
    }
}
