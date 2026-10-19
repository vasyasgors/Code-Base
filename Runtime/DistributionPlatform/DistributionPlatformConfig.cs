using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public enum PlatformSDK
    {
        Local = 0,
        YandexSDK = 1,
        CrazySDK = 2,
        PrimeSDK = 3,
        AppLovin = 4
    }

    public enum DistributionPlatform
    {
        Any = 0,
        YandexGames = 1,
        CrazyGames = 2,
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
        public const string PlatfromSDKPrefix = "PLATFORM_SDK_";

        public PlatformSDK CurrentPlatformSDK = PlatformSDK.Local;
        public DistributionPlatform CurrentDistributionPlatform = DistributionPlatform.YandexGames;


        public string GetPlatformSDKDefine()
        {
            if (CurrentPlatformSDK == PlatformSDK.Local) return PlatfromSDKPrefix + "LOCAL";
            if (CurrentPlatformSDK == PlatformSDK.YandexSDK) return PlatfromSDKPrefix + "YANDEX";
            if (CurrentPlatformSDK == PlatformSDK.CrazySDK) return PlatfromSDKPrefix + "CRAZY";
            if (CurrentPlatformSDK == PlatformSDK.PrimeSDK) return PlatfromSDKPrefix + "PRIME";
            if (CurrentPlatformSDK == PlatformSDK.AppLovin) return PlatfromSDKPrefix + "APPLOVIN";

            return DistributionPlatformPrefix + "LOCAL";

        }

        public string GetDistributionPlatformDefine()
        {
            if (CurrentDistributionPlatform == DistributionPlatform.Any) return DistributionPlatformPrefix + "ANY";
            if (CurrentDistributionPlatform == DistributionPlatform.YandexGames) return DistributionPlatformPrefix + "YANDEX_GAMES";
            if (CurrentDistributionPlatform == DistributionPlatform.CrazyGames) return DistributionPlatformPrefix + "CRAZY_GAMES";
            if (CurrentDistributionPlatform == DistributionPlatform.Poki) return DistributionPlatformPrefix + "POKI";
            if (CurrentDistributionPlatform == DistributionPlatform.Y8) return DistributionPlatformPrefix + "Y8";
            if (CurrentDistributionPlatform == DistributionPlatform.PlayGama) return DistributionPlatformPrefix + "PLAY_GAMA";
            if (CurrentDistributionPlatform == DistributionPlatform.GameDistribution) return DistributionPlatformPrefix + "GAME_DISTRIBUTION";
            if (CurrentDistributionPlatform == DistributionPlatform.Lagged) return DistributionPlatformPrefix + "LAGGED";
            if (CurrentDistributionPlatform == DistributionPlatform.XiaomiWeb) return DistributionPlatformPrefix + "XIAOMI_WEB";
            if (CurrentDistributionPlatform == DistributionPlatform.RuStore) return DistributionPlatformPrefix + "RUSTORE";
            if (CurrentDistributionPlatform == DistributionPlatform.GooglePlay) return DistributionPlatformPrefix + "GOOGLE_PLAY";

            return DistributionPlatformPrefix + "ANY";

        }


        public string GetCurrentDistributionPlatformName()
        {
            return CurrentDistributionPlatform.ToString(); 
        }
    }


}
