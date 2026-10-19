using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public enum PlatformSDK
    {
        Local = 0,
        Yandex = 1,
        Crazy = 2,
        Prime = 3,
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

    [CreateAssetMenu(fileName = "Build Environment Config", menuName = "Configs/Build Environment Config")]
    public class BuildEnvironmentConfig : ScriptableObject
    {
        public const string DistributionPlatformPrefix = "DISTRIBUTION_PLATFORM_";
        public const string PlatfromSDKPostfix = "SDK";


        public PlatformSDK CurrentPlatformSDK = PlatformSDK.Local;
        public DistributionPlatform CurrentDistributionPlatform = DistributionPlatform.YandexGames;


        public string GetPlatformSDKDefine()
        {
            return CurrentPlatformSDK.ToString().ToUpper() + PlatfromSDKPostfix;
        }

        public string GetDistributionPlatformDefine()
        {
            return DistributionPlatformPrefix + CurrentDistributionPlatform.ToString().ToUpper();

        }


        public string GetCurrentDistributionPlatformName()
        {
            return CurrentDistributionPlatform.ToString(); 
        }
    }


}
