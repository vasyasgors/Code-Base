namespace CodeBase.Infrastructure
{
    public interface IPlayerProgressProvider : IService
    {
        int SaveVersion { get; }
        
        TProgress GetProgress<TProgress>();

        int ReadSaveVersionJSON(string json);
        void SetProgressJSON(string json);
        string GetProgressJSON();
        void IncrementSaveVerstion();
        void SetDefaultProgress();
    }
}


