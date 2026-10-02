using UnityEngine;
namespace TitansOfTheSea.Player
{
    [CreateAssetMenu(menuName = "Titans/Person B/Player Movement")]
    public sealed class PlayerMovementSettings : ScriptableObject
    {
        [Min(.1f)] public float WalkSpeed = 4;
        [Min(.1f)] public float SprintSpeed = 6.5f;
        [Min(.1f)] public float SwimSpeed = 3;
        [Min(.1f)] public float ClimbSpeed = 2;
        [Min(0)] public float YawSpeed = 120;
        [Min(.1f)] public float JumpHeight = 1.2f;
        [Min(.1f)] public float Gravity = 25;
        [Min(.1f)] public float DiveSpeed = 2;
        [Min(.1f)] public float SurfaceSpring = 8;
        [Min(.1f)] public float SurfaceDamping = 4;
        [Range(.1f, 1.5f)] public float SwimDepth = .65f;
        [Range(0, 60)] public float SlopeLimit = 45;
        [Range(.05f, .5f)] public float StepHeight = .3f;
        public LayerMask GroundMask = ~0;
    }
}
