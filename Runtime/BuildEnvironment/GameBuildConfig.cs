using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Infrastructure
{
    using System;
    using UnityEngine;

    public enum DistributionPlatform
    {
        All = 0,
        YandexGames = 1,
        CrazyGames = 2,
        PrimeSDK = 3,
        Poki = 4,
        Y8 = 5,
        PlayGama = 6,
        GameDistribution = 7,
        Lagged = 8,
        XiaomiWeb = 9
    }

    [CreateAssetMenu(fileName = "Distribution Platform Config", menuName = "Configs/Distribution Platform Config")]
    public class GameBuildConfig : ScriptableObject
    {
        public const string DistributionPlatformPrefix = "DISTRIBUTION_PLATFORM_";

        public DistributionPlatform currentDistributionPlatform = DistributionPlatform.YandexGames;

        public string GetDefineSymbol()
        {
            if (currentDistributionPlatform == DistributionPlatform.All) return DistributionPlatformPrefix + "ALL";
            if (currentDistributionPlatform == DistributionPlatform.YandexGames) return DistributionPlatformPrefix + "YANDEX_GAMES";
            if (currentDistributionPlatform == DistributionPlatform.CrazyGames) return DistributionPlatformPrefix + "CRAZY_GAMES";
            if (currentDistributionPlatform == DistributionPlatform.PrimeSDK) return DistributionPlatformPrefix + "PRIME_SDK";
            if (currentDistributionPlatform == DistributionPlatform.Poki) return DistributionPlatformPrefix + "POKI";
            if (currentDistributionPlatform == DistributionPlatform.Y8) return DistributionPlatformPrefix + "Y8";
            if (currentDistributionPlatform == DistributionPlatform.PlayGama) return DistributionPlatformPrefix + "PLAY_GAMA";
            if (currentDistributionPlatform == DistributionPlatform.GameDistribution) return DistributionPlatformPrefix + "GAME_DISTRIBUTION";
            if (currentDistributionPlatform == DistributionPlatform.Lagged) return DistributionPlatformPrefix + "LAGGED";
            if (currentDistributionPlatform == DistributionPlatform.XiaomiWeb) return DistributionPlatformPrefix + "XIAOMI_WEB";

            return DistributionPlatformPrefix + "ALL";

        }

        public string GetVariantName()
        {
            return currentDistributionPlatform.ToString(); 
        }
    }


}
