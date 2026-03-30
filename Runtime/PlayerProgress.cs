using UnityEngine;

namespace CodeBase.Infrastructure
{
    public partial class PlayerProgress
    {
        public int SaveVersion = 0;

        public string ToJsonString()
        {
            return JsonUtility.ToJson(this);
        }

        public static PlayerProgress FromJsonString(string progress)
        {
            return JsonUtility.FromJson<PlayerProgress>(progress);
        }
    }
}


