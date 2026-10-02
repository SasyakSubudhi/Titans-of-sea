namespace TitansOfTheSea.Contracts
{
    public interface IVfxService
    {
        void Spawn(string vfxId, UnityEngine.Vector3 position, UnityEngine.Quaternion rotation);
    }
}
