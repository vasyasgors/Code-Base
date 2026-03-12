
using CodeBase.Infrastructure;
using System.Threading.Tasks;
using UnityEngine.Events;


namespace CodeBase.Infrastructure
{
    
    public interface IPlayer : IService
    {
        void Init();
        Task InitAsync();
        void Login(UnityAction login = null, UnityAction error = null);
        bool IsAuth { get; }
    }
}





