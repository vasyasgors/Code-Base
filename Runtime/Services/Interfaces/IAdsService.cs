
using CodeBase.Infrastructure;
using System;
using UnityEngine.Events;

namespace CodeBase.Infrastructure
{
    public interface IAdsService : IService
    {
        bool IsShowFullscreenAd { get; set; }
        bool IsShowRewardedAd { get; set; }
        bool IsShowStickydBanner { get; set; }

        void ShowFullscreenAd(UnityAction onClosed = null);
        void ShowRewardedAd(UnityAction onRewarded = null, UnityAction onClosed = null);
        void ShowStickyBanner();
        void HideStickyBanner();
    }
}





