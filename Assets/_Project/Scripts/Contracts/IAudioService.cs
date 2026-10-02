namespace TitansOfTheSea.Contracts
{
    public enum MusicState { Exploration, Tension, Combat, Storm, Port, Home, Titan, Sad, Triumph }
    public interface IAudioService
    {
        void PlaySfx(string sfxId, UnityEngine.Vector3 position);
        void SetMusic(MusicState state);
    }
}
