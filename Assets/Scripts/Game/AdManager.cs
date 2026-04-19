using System;
using UnityEngine;
using GoogleMobileAds.Api;

public class AdManager : MonoBehaviour
{
    public static AdManager instance = null;

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#if UNITY_IOS
    private const string REVIVE_AD_UNIT_ID = "ca-app-pub-3940256099942544/1712485313";
    private const string DECK_UNLOCK_AD_UNIT_ID = "ca-app-pub-3940256099942544/1712485313";
#else
    private const string REVIVE_AD_UNIT_ID = "ca-app-pub-3940256099942544/5224354917";
    private const string DECK_UNLOCK_AD_UNIT_ID = "ca-app-pub-3940256099942544/5224354917";
#endif
#else
    private const string REVIVE_AD_UNIT_ID = "ca-app-pub-4792779991638012/2237244154";
    private const string DECK_UNLOCK_AD_UNIT_ID = "ca-app-pub-4792779991638012/8477838586";
#endif

    private RewardedAd reviveAd;
    private RewardedAd deckUnlockAd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Initialize();
    }

    private void Initialize()
    {
        MobileAds.Initialize(_ =>
        {
            LoadReviveAd(allowRetry: true);
            LoadDeckUnlockAd(allowRetry: true);
        });
    }

    public void ShowReviveAd(Action onRewarded, Action onFailed = null)
    {
        if (reviveAd == null || !reviveAd.CanShowAd())
        {
            onFailed?.Invoke();
            LoadReviveAd(allowRetry: true);
            return;
        }

        bool rewarded = false;
        reviveAd.OnAdFullScreenContentClosed += () =>
        {
            LoadReviveAd(allowRetry: true);
            if (!rewarded)
                onFailed?.Invoke();
        };
        reviveAd.OnAdFullScreenContentFailed += _ =>
        {
            LoadReviveAd(allowRetry: true);
            if (!rewarded)
                onFailed?.Invoke();
        };

        reviveAd.Show(_ =>
        {
            rewarded = true;
            onRewarded?.Invoke();
        });
    }

    public void ShowDeckUnlockAd(Action onRewarded, Action onFailed = null)
    {
        if (deckUnlockAd == null || !deckUnlockAd.CanShowAd())
        {
            onFailed?.Invoke();
            LoadDeckUnlockAd(allowRetry: true);
            return;
        }

        bool rewarded = false;
        deckUnlockAd.OnAdFullScreenContentClosed += () =>
        {
            LoadDeckUnlockAd(allowRetry: true);
            if (!rewarded)
                onFailed?.Invoke();
        };
        deckUnlockAd.OnAdFullScreenContentFailed += _ =>
        {
            LoadDeckUnlockAd(allowRetry: true);
            if (!rewarded)
                onFailed?.Invoke();
        };

        deckUnlockAd.Show(_ =>
        {
            rewarded = true;
            onRewarded?.Invoke();
        });
    }

    private void LoadReviveAd(bool allowRetry)
    {
        if (reviveAd != null)
        {
            reviveAd.Destroy();
            reviveAd = null;
        }

        AdRequest request = new AdRequest();
        RewardedAd.Load(REVIVE_AD_UNIT_ID, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"[AdManager] Revive ad load failed: {error}");
                if (allowRetry)
                    LoadReviveAd(allowRetry: false);
                return;
            }

            reviveAd = ad;
        });
    }

    private void LoadDeckUnlockAd(bool allowRetry)
    {
        if (deckUnlockAd != null)
        {
            deckUnlockAd.Destroy();
            deckUnlockAd = null;
        }

        AdRequest request = new AdRequest();
        RewardedAd.Load(DECK_UNLOCK_AD_UNIT_ID, request, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogWarning($"[AdManager] Deck unlock ad load failed: {error}");
                if (allowRetry)
                    LoadDeckUnlockAd(allowRetry: false);
                return;
            }

            deckUnlockAd = ad;
        });
    }
}