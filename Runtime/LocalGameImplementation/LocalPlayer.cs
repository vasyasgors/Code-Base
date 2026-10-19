using CodeBase.Infrastructure;
using System.Threading.Tasks;
using UnityEngine.Events;

namespace CodeBase
{

    public class LocalPlayer : IPlayer
    {
        public bool IsAuth => true;

        public void Init()
        {

        }

        public Task InitAsync()
        {
            return Task.CompletedTask;
        }

        public void Login(UnityAction login = null, UnityAction error = null)
        {

        }
    }



}