
using CodeBase.Infrastructure;
using System;
using UnityEngine.Events;

namespace CodeBase.Infrastructure
{
    public interface IAdsService : IService
    {
        bool IsFullscreenAvailable { get; }
        bool IsRewardedAvailable { get;  }
        bool IsStickyAvailable { get;  }

        void ShowFullscreenAd(UnityAction onClosed = null);
        void ShowRewardedAd(UnityAction onRewarded = null, UnityAction onClosed = null);
        void ShowStickyBanner();
        void HideStickyBanner();
    }
}





