using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
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
        XiaomiWeb = 9,
        RuStore = 10,
        GooglePlay = 11
    }

    [CreateAssetMenu(fileName = "Distribution Platform Config", menuName = "Configs/Distribution Platform Config")]
    public class DistributionPlatformConfig : ScriptableObject
    {
        public const string DistributionPlatformPrefix = "DISTRIBUTION_PLATFORM_";

        public DistributionPlatform CurrentDistributionPlatform = DistributionPlatform.YandexGames;

        public string GetDefineSymbol()
        {
            if (CurrentDistributionPlatform == DistributionPlatform.All) return DistributionPlatformPrefix + "ALL";
            if (CurrentDistributionPlatform == DistributionPlatform.YandexGames) return DistributionPlatformPrefix + "YANDEX_GAMES";
            if (CurrentDistributionPlatform == DistributionPlatform.CrazyGames) return DistributionPlatformPrefix + "CRAZY_GAMES";
            if (CurrentDistributionPlatform == DistributionPlatform.PrimeSDK) return DistributionPlatformPrefix + "PRIME_SDK";
            if (CurrentDistributionPlatform == DistributionPlatform.Poki) return DistributionPlatformPrefix + "POKI";
            if (CurrentDistributionPlatform == DistributionPlatform.Y8) return DistributionPlatformPrefix + "Y8";
            if (CurrentDistributionPlatform == DistributionPlatform.PlayGama) return DistributionPlatformPrefix + "PLAY_GAMA";
            if (CurrentDistributionPlatform == DistributionPlatform.GameDistribution) return DistributionPlatformPrefix + "GAME_DISTRIBUTION";
            if (CurrentDistributionPlatform == DistributionPlatform.Lagged) return DistributionPlatformPrefix + "LAGGED";
            if (CurrentDistributionPlatform == DistributionPlatform.XiaomiWeb) return DistributionPlatformPrefix + "XIAOMI_WEB";
            if (CurrentDistributionPlatform == DistributionPlatform.RuStore) return DistributionPlatformPrefix + "RUSTORE";
            if (CurrentDistributionPlatform == DistributionPlatform.GooglePlay) return DistributionPlatformPrefix + "GOOGLE_PLAY";

            return DistributionPlatformPrefix + "ALL";

        }

        public string GetVariantName()
        {
            return CurrentDistributionPlatform.ToString(); 
        }
    }


}
