using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    using System;
    using UnityEngine;

    public enum DistributionPlatform
    {
        All,
        YandexGames,
        CrazyGames,
    }

    [CreateAssetMenu(fileName = "GameBuildConfig", menuName = "Build/Game Build Config")]
    public class GameBuildConfig : ScriptableObject
    {
        public const string DistributionPlatformPrefix = "DISTRIBUTION_PLATFORM_";

        public DistributionPlatform currentDistributionPlatform = DistributionPlatform.YandexGames;

        public string GetDefineSymbol()
        {
            if (currentDistributionPlatform == DistributionPlatform.All) return DistributionPlatformPrefix + "ALL";
            if (currentDistributionPlatform == DistributionPlatform.YandexGames) return DistributionPlatformPrefix + "YANDEX_GAMES";
            if (currentDistributionPlatform == DistributionPlatform.CrazyGames) return DistributionPlatformPrefix + "CRAZY_GAMES";

            return DistributionPlatformPrefix + "ALL";

        }

        public string GetVariantName()
        {
            return currentDistributionPlatform.ToString();
        }
    }


}
