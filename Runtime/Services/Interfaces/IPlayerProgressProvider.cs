namespace CodeBase.Infrastructure
{
    public interface IPlayerProgressProvider : IService
    {
        int SaveVersion { get; }
        
        TProgress GetProgress<TProgress>() where TProgress : class;

        int ReadSaveVersionJSON(string json);
        void SetProgressJSON(string json);
        string GetProgressJSON();
        void IncrementSaveVerstion();
        void SetDefaultProgress();
    }
}


