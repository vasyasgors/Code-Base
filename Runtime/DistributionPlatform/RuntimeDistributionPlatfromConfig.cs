using UnityEngine;

namespace CodeBase.Infrastructure
{
    [CreateAssetMenu(fileName = "Runtime Distribution Platform Config", menuName = "Configs/Runtime Distribution Platfrom Config")]
    public class RuntimeDistributionPlatfromConfig : ScriptableObject
    {
        public DistributionPlatform CurrentPlatform = DistributionPlatform.YandexGames;
    }
}
