using UnityEngine;

namespace TitansOfTheSea.Ship
{
    [CreateAssetMenu(menuName = "Titans of the Sea/B/Buoyancy Settings")]
    public sealed class BuoyancySettings : ScriptableObject
    {
        [Min(1)] public float MassKg = 1000f;
        [Min(0.05f)] public float FloatDepth = 0.75f;
        [Min(1.05f)] public float LiftMultiplier = 2f;
        [Min(0)] public float VerticalDamping = 2f;
        [Min(0)] public float LinearDamping = 0.15f;
        [Min(0)] public float AngularDamping = 1f;
        public Vector3 CentreOfMass = new Vector3(0, -0.35f, 0);
        public Vector3[] LocalPoints = {
            new Vector3(-1.2f,-0.5f,-2.5f), new Vector3(1.2f,-0.5f,-2.5f),
            new Vector3(-1.2f,-0.5f,0), new Vector3(1.2f,-0.5f,0),
            new Vector3(-1.2f,-0.5f,2.5f), new Vector3(1.2f,-0.5f,2.5f)
        };
    }
}
