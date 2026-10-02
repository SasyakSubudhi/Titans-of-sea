using UnityEngine;

namespace TitansOfTheSea.Ship
{
    public static class SailingMath
    {
        // Wind direction describes the direction air travels, in world XZ.
        // Broad reach is strongest; the no-go zone prevents sailing directly into wind.
        public static float PointOfSail(float angleFromWind)
        {
            float angle = Mathf.Clamp(Mathf.Abs(angleFromWind), 0, 180);
            if (angle >= 145) return 0;
            if (angle <= 70) return Mathf.Lerp(.65f, 1, angle / 70);
            return Mathf.Lerp(1, 0, (angle - 70) / 75);
        }
        public static float Thrust(Vector3 forward, Vector2 windDirection, float windSpeed,
            float raised, float boomDegrees, float area, float coefficient)
        {
            if (windDirection.sqrMagnitude < .001f || windSpeed <= 0) return 0;
            var wind = new Vector3(windDirection.x, 0, windDirection.y).normalized;
            forward.y = 0;
            if (forward.sqrMagnitude < .001f) return 0;
            forward.Normalize();
            float angle = Vector3.Angle(forward, wind);
            float desiredBoom = Mathf.Clamp(Vector3.SignedAngle(forward, wind, Vector3.up), -80, 80);
            float alignment = Mathf.Clamp01(Mathf.Cos(Mathf.DeltaAngle(boomDegrees, desiredBoom) * Mathf.Deg2Rad));
            return Mathf.Clamp01(raised) * Mathf.Max(0, area) * Mathf.Max(0, coefficient)
                * Mathf.Min(windSpeed, 30) * Mathf.Min(windSpeed, 30) * PointOfSail(angle) * alignment;
        }
        public static Vector3 AnchorAcceleration(Vector3 velocity, float deceleration, float dt)
        {
            velocity.y = 0;
            if (dt <= 0 || velocity.sqrMagnitude < .000001f) return Vector3.zero;
            return -velocity.normalized * Mathf.Min(Mathf.Max(0, deceleration), velocity.magnitude / dt);
        }
    }
}
