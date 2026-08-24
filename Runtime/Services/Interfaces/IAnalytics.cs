using System.Collections.Generic;

namespace CodeBase.Infrastructure
{
    public interface IAnalytics : IService
    {
        void Init();
        public void SendEvent(string eventName);
        public void SendEvent(string eventName, string nestedParam, string subNestedParam);
    }



}





