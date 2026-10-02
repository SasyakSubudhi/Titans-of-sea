namespace TitansOfTheSea.Contracts
{
    public interface ICutsceneService
    {
        bool IsPlaying { get; }
        void Play(string cutsceneId, System.Action onFinished); // B calls; A runs Timeline
        void Skip();
    }
}
