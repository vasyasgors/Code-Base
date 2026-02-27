using System.Threading.Tasks;

namespace CodeBase.Infractructure
{
    public interface IPlayerProgressSaveLoad : IService
    {
        bool IsFirstLoad { get; }

        void Load();
        void Save();

        Task LoadAsync();
        Task SaveAsync();
    }
}


