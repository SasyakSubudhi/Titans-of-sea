using UnityEngine;
namespace TitansOfTheSea.Contracts
{
    public interface IOceanSurface
    {
        /// Height of the water surface (metres, world Y) at a world position, at the current time.
        float GetHeight(Vector3 worldPos);
        /// Surface normal (tilt) at a world position, used for ship rocking.
        Vector3 GetNormal(Vector3 worldPos);
        /// Fast batch version for buoyancy points (avoids allocations).
        void GetHeights(Vector3[] worldPositions, float[] heightsOut, int count);
    }
}
