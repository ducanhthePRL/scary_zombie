using System;
using System.Collections;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using UnityEngine.Events;
using AppsFlyerSDK;
using System.Collections.Generic;

public class AdsManager : SingletonMonoDontDestroy<AdsManager>
{
    private ObscuredBool isInit = false;
    private UnityAction OnRewardDone = null;
    private UnityAction OnRewardFailed = null;
    private ObscuredBool _isShowingAd = false;
    public ObscuredBool isShowingAd => _isShowingAd;
    private Action OnInitDone = null;
    private GameObject currentBanner = null;
    private UnityAction OnInterDone = null;
    private ObscuredString _adBannerUnitId = "";
    private ObscuredString adBannerUnitId
    {
        get
        {
            if(string.IsNullOrEmpty(_adBannerUnitId))
                _adBannerUnitId = EnvironmentConfig.adBannerIdApplovinAOS;
            return _adBannerUnitId;
        }
    }
    private ObscuredString _adInterUnitId= "";
    private ObscuredString adInterUnitId
    {
        get
        {
            if (string.IsNullOrEmpty(_adInterUnitId))
                _adInterUnitId = EnvironmentConfig.adInterIdApplovinAOS;
            return _adInterUnitId;
        }
    }

    private ObscuredString _adRewardUnitId = "";
    private ObscuredString adRewardUnitId
    {
        get
        {
            if (string.IsNullOrEmpty(_adRewardUnitId))
                _adRewardUnitId = EnvironmentConfig.adRewardIdApplovinAOS;
            return _adRewardUnitId;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        if (environment == EnvironmentType.dev)
            Observer.Instance.AddObserver(ObserverKey.EnableCheatMode, EnableCheatMode);
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        if (environment == EnvironmentType.dev)
            Observer.Instance.RemoveObserver(ObserverKey.EnableCheatMode, EnableCheatMode);
    }

    //    // Start is called before the first frame update
    public void Init(Action action)
    {
        if (UserDatas.isEnableCheatMode)
        {
            action?.Invoke();
            return;
        }
        OnInitDone = action;
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        if (environment == EnvironmentType.live)
        {

            //List<string> deviceIds = new List<string>();
            //deviceIds.Add("56A31C832FD908961DF9D0B70303FB6A");
            //RequestConfiguration requestConfiguration = new RequestConfiguration
            //    .Builder()
            //    .SetTestDeviceIds(deviceIds)
            //    .build();
            //MobileAds.SetRequestConfiguration(requestConfiguration);
        }
        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
            // AppLovin SDK is initialized, start loading ads
            isInit = true; LoadAds();
            if(environment == EnvironmentType.dev)
                MaxSdk.ShowMediationDebugger();
        };

        MaxSdk.SetVerboseLogging(true);
        MaxSdk.SetSdkKey("zq2X1FfdfeIMOsmepidyMNbeqvHKzJyNwy6EI2lT_14Ns_yAy-XVUTSsThDAZ5-AqDJ2OU0CLSeP7euEiV4wor");
        MaxSdk.SetUserId("USER_ID");
        MaxSdk.InitializeSdk();
    }

    private void LoadAds()
    {
        RequestBanner();
        RequestInterstitial();
        RequestReward();
        OnInitDone?.Invoke();
        OnInitDone = null;
    }

    #region banner
    private void RequestBanner()
    {
        if (UserDatas.isBoughtRemoveAds) return;
#if UNITY_ANDROID
        string adUnitId = adBannerUnitId;
#elif UNITY_IPHONE
                string adUnitId = adBannerUnitId;
#else
                string adUnitId = "unexpected_platform";
#endif
        // Banners are automatically sized to 320?50 on phones and 728?90 on tablets
        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
        MaxSdk.CreateBanner(adUnitId, MaxSdkBase.BannerPosition.BottomCenter);
        MaxSdk.SetBannerExtraParameter(adUnitId, "adaptive_banner", "true");
        // Set background or background color for banners to be fully functional
        //MaxSdk.SetBannerBackgroundColor(adUnitId, Color.black);
        MaxSdk.SetBannerBackgroundColor(adUnitId, new Color(0, 0, 0, 0));

        if (MaxSdkUtils.IsTablet()) {
            MaxSdkUtils.GetAdaptiveBannerHeight(728);
            MaxSdk.SetBannerWidth(adUnitId, 728);
        }
        else {
            MaxSdkUtils.GetAdaptiveBannerHeight(320);
            MaxSdk.SetBannerWidth(adUnitId, 320);
        }
        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;

    }

    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) {
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
        string networkPlacement = adInfo.NetworkPlacement; // The placement ID from the network that showed the ad
        string ad_format = adInfo.AdFormat;
        Hashtable hashtable = new Hashtable();
        hashtable.Add("ad_platform", "AppLovin");
        hashtable.Add("ad_source", networkName);
        hashtable.Add("ad_unit_name", adUnitIdentifier);
        hashtable.Add("ad_format", ad_format);
        hashtable.Add("value", revenue);
        hashtable.Add("currency", "USD");
        FirebaseManager.instance.LogEvent("ad_impression", hashtable);
    }

    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

#endregion

#region inter
    private int retryAttempt;
    private void RequestInterstitial()
    {
        if (UserDatas.isBoughtRemoveAds) return;
#if UNITY_ANDROID
        string adUnitId = adInterUnitId;
#elif UNITY_IPHONE
                string adUnitId = adInterUnitId;
#else
                string adUnitId = "unexpected_platform";
#endif

        // Attach callback
        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
        MaxSdkCallbacks.Interstitial.OnAdRevenuePaidEvent += OnInterstitialAdRevenuePaidEvent;
        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
        LoadInterstitial();
    }

    private void LoadInterstitial()
    {
        MaxSdk.LoadInterstitial(adInterUnitId);
    }

    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Interstitial ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        Invoke("LoadInterstitial", (float)retryDelay);
    }

    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) 
	{ 
		isInterClosed = false; 
		Dictionary<string, string>
            eventValues = new Dictionary<string, string>();
        eventValues.Add("af_inters", "1");
        AppsFlyer.sendEvent("af_inters", eventValues);
	}

    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
        LoadInterstitial();
        _isShowingAd = false;
        OnInterDone?.Invoke();
        OnInterDone = null;
    }
    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Interstitial ad is hidden. Pre-load the next ad.
        LoadInterstitial();
        _isShowingAd = false;
#if UNITY_EDITOR
        ResetAdsCounter();
        OnInterDone?.Invoke();
        OnInterDone = null;
#else
                    isInterClosed = true;
#endif
    }

    private void OnInterstitialAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
        string networkPlacement = adInfo.NetworkPlacement; // The placement ID from the network that showed the ad
        string ad_format = adInfo.AdFormat;
        Hashtable hashtable = new Hashtable();
        hashtable.Add("ad_platform", "AppLovin");
        hashtable.Add("ad_source", networkName);
        hashtable.Add("ad_unit_name", adUnitIdentifier);
        hashtable.Add("ad_format", ad_format);
        hashtable.Add("value", revenue);
        hashtable.Add("currency", "USD");
        FirebaseManager.instance.LogEvent("ad_impression", hashtable);
    }
    #endregion

    #region reward
    private void RequestReward()
    {
#if UNITY_ANDROID
        string adUnitId = adRewardUnitId;
#elif UNITY_IPHONE
                string adUnitId = adRewardUnitId;
#else
                string adUnitId = "unexpected_platform";
#endif
        // Attach callback
        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

        // Load the first rewarded ad
        LoadRewardedAd();
    }

    private void LoadRewardedAd()
    {
        MaxSdk.LoadRewardedAd(adRewardUnitId);
    }

    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

        // Reset retry attempt
        retryAttempt = 0;
    }

    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
    {
        // Rewarded ad failed to load 
        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

        retryAttempt++;
        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

        Invoke("LoadRewardedAd", (float)retryDelay);
    }

    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        isRewardClosed = false;
        FirebaseManager.instance.LogEvent("ads_reward_success");
        Dictionary<string, string>
            eventValues = new Dictionary<string, string>();
        eventValues.Add("af_rewarded", "1");
        AppsFlyer.sendEvent("af_rewarded", eventValues);
        //MonoBehaviour.print("HandleRewardedAdOpening event received");
    }

    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
        LoadRewardedAd();
        _isShowingAd = false;
        OnRewardFailed?.Invoke();
        OnRewardFailed = null;
    }

    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Rewarded ad is hidden. Pre-load the next ad
        LoadRewardedAd();
        isRewardClosed = true;
        _isShowingAd = false;
        FirebaseManager.instance.LogEvent("ads_reward_finish");
        Dictionary<string, string>
            eventValues = new Dictionary<string, string>();
        eventValues.Add("af_AOA", "1");
        AppsFlyer.sendEvent("af_AOA", eventValues);

    }

    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
    {
        // The rewarded ad displayed and the user should receive the reward.
        StartCoroutine(IEWaitRewardClose());
    }

    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
    {
        // Ad revenue paid. Use this callback to track user revenue.
        double revenue = adInfo.Revenue;

        // Miscellaneous data
        string countryCode = MaxSdk.GetSdkConfiguration().CountryCode; // "US" for the United States, etc - Note: Do not confuse this with currency code which is "USD" in most cases!
        string networkName = adInfo.NetworkName; // Display name of the network that showed the ad (e.g. "AdColony")
        string adUnitIdentifier = adInfo.AdUnitIdentifier; // The MAX Ad Unit ID
        string placement = adInfo.Placement; // The placement this ad's postbacks are tied to
        string networkPlacement = adInfo.NetworkPlacement; // The placement ID from the network that showed the ad
        string ad_format = adInfo.AdFormat;
        Hashtable hashtable = new Hashtable();
        hashtable.Add("ad_platform", "AppLovin");
        hashtable.Add("ad_source", networkName);
        hashtable.Add("ad_unit_name", adUnitIdentifier);
        hashtable.Add("ad_format", ad_format);
        hashtable.Add("value", revenue);
        hashtable.Add("currency", "USD");
        FirebaseManager.instance.LogEvent("ad_impression", hashtable);
    }

    private bool isRewardClosed = false;
    private IEnumerator IEWaitRewardClose()
    {
        yield return new WaitUntil(() => isRewardClosed);
        OnRewardDone?.Invoke();
        OnRewardDone = null;
    }
    #endregion


    public void ShowBanner(bool is_show)
    {
        //#if UNITY_EDITOR
        //        return;
        //#endif
        SceneType current_scene = UserDatas.current_scene_type;
        if (isInit == false || current_scene != SceneType.MainMenu && current_scene != SceneType.Gameplay) return;
        if (is_show)
        {
            if (UserDatas.isBoughtRemoveAds || UserDatas.isEnableCheatMode) return;
            if (currentBanner != null)
                Destroy(currentBanner);
            MaxSdk.ShowBanner(adBannerUnitId);
            currentBanner = GameObject.Find("BANNER(Clone)");
        }
        else
            MaxSdk.HideBanner(adBannerUnitId);
    }

    public void ShowInter(UnityAction action)
    {
        if (UserDatas.isBoughtRemoveAds || isInit == false || _isShowingAd || UserDatas.isEnableCheatMode|| !adsInterPlay)
        {
            action?.Invoke();
            return;
        }

        _isShowingAd = true;
        if (MaxSdk.IsInterstitialReady(adInterUnitId))
        {
            OnInterDone = action;
            isInterClosed = false;
            MaxSdk.ShowInterstitial(adInterUnitId);

        }
        else
        {
            LoadInterstitial();
            _isShowingAd = false;
            action?.Invoke();
        }
        #region Firebase
        UserDatas.user_Data.countInterAds++;
        int countInter = UserDatas.user_Data.countInterAds;
        Hashtable hashtable = new Hashtable();
        hashtable.Add("countInter", countInter);
        FirebaseManager.instance.LogEvent("inter_attempt", hashtable);
#endregion
    }

    public void ShowReward(UnityAction on_done, UnityAction on_failed)
    {
        if (UserDatas.isEnableCheatMode)
        {
            on_done?.Invoke();
            return;
        }
        if (isInit == false || _isShowingAd)
        {
            on_failed?.Invoke();
            return;
        }
        _isShowingAd = true;
        isRewardClosed = false;
        if (MaxSdk.IsRewardedAdReady(adRewardUnitId))
        {
            OnRewardDone = on_done;
            OnRewardFailed = on_failed;
            MaxSdk.ShowRewardedAd(adRewardUnitId);
        }
        else
        {
            RequestReward();
            _isShowingAd = false;
            on_failed?.Invoke();
        }
        FirebaseManager.instance.LogEvent("reward_attempt");
    }

    private void OnApplicationPause(bool pause)
    {
        if (!pause)
        {
            if (OnInterDone != null)
            {
                if (corWaitInterClose != null)
                    StopCoroutine(corWaitInterClose);
                corWaitInterClose = StartCoroutine(IEWaitInterClose());
            }
        }
    }

    private Coroutine corWaitInterClose = null;
    private bool isInterClosed = false;
    private IEnumerator IEWaitInterClose()
    {
        yield return new WaitUntil(() => isInterClosed);
        ResetAdsCounter();
        OnInterDone?.Invoke();
        OnInterDone = null;
    }
    private ObscuredBool adsInterPlay = true;
    public void ResetAdsCounter()
    {
        if(corWaitToShowAdsInter!=null)
            StopCoroutine(corWaitToShowAdsInter);
        adsInterPlay = false;
        corWaitToShowAdsInter = StartCoroutine(WaitToShowAdsInter());
    }
    private Coroutine corWaitToShowAdsInter = null;
    private IEnumerator WaitToShowAdsInter()
    {
        yield return new WaitForSeconds(35f);
        adsInterPlay = true;
    }

    private void EnableCheatMode(object data)
    {
        if (data == null) return;
        bool active = (bool)data;
        ShowBanner(!active);
    }
}
