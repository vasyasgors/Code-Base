using System;

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

    public interface IBeforeSaveEvent
    {
        event Action BeforeSave;
    }
    public interface IAfterSaveEvent
    {
        event Action AfterSave;
    }

    public interface IBeforeLoadEvent
    {
        event Action BeforeLoad;
    }

    public interface IAfterLoadEvent
    {
        event Action AfterLoad;
    }
}


