using UnityEngine;

namespace CodeBase.Infrastructure
{
    [CreateAssetMenu(fileName = "Runtime Distribution Platform Config", menuName = "Configs/Runtime Distribution Platfrom Config")]
    public class RuntimeDistributionPlatfromConfig : ScriptableObject
    {
        [Header("Current Synchronized Targets")]
        public PlatformSDK CurrentPlatformSDK = PlatformSDK.Local;
        public DistributionPlatform CurrentDistributionPlatform = DistributionPlatform.YandexGames;
    }
}
