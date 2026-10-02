using UnityEngine;

namespace TitansOfTheSea.Ship
{
    public static class BuoyancyMath
    {
        // Weight-normalized spring support with bounded damping; never pulls a hull downward.
        public static float SupportAcceleration(float depth, float floatDepth, float lift, float damping, float velocityY, float gravity)
        {
            if (depth <= 0f) return 0f;
            float submerged = Mathf.Clamp01(depth / Mathf.Max(0.05f, floatDepth));
            return Mathf.Clamp(gravity * lift * submerged - damping * velocityY * submerged, 0f, gravity * lift * 2f);
        }
    }
}
