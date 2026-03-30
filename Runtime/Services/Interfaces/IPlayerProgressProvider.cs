namespace CodeBase.Infrastructure
{
    public interface IPlayerProgressProvider : IService
    {
        int SaveVersion { get; }

        int ReadSaveVerstion(string json);
        void IncrementSaveVerstion();
        void SetProgress(string json);
        void SetDefaultProgress();
        string GetProgress();
    }
}


