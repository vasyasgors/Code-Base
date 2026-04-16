namespace CodeBase.Infrastructure
{
    public interface IPlayerProgressProviderBase : IService
    {
        int SaveVersion { get; }
        
        int ReadSaveVersionJSON(string json);
        void SetProgressJSON(string json);
        string GetProgressJSON();
        void IncrementSaveVerstion();
        void SetDefaultProgress();
    }
}


