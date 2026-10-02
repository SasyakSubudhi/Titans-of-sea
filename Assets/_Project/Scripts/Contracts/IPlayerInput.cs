namespace TitansOfTheSea.Contracts
{
    public struct PlayerInputState
    {
        public UnityEngine.Vector2 Move, Look;
        public bool Sprint, JumpPressed, InteractPressed, AttackPressed, AimHeld,
                    BlockHeld, DodgePressed, UseItemPressed, MapPressed, InventoryPressed;
        public float SteerAxis;      // -1..1 when at the ship wheel
        public float SailAxis;       // raise/lower sail
        public float SailTurnAxis;   // rotate sail
        public int HotbarSlot;       // -1 if none
    }
    public interface IPlayerInput { PlayerInputState Current { get; } bool Enabled { get; set; } }
}
