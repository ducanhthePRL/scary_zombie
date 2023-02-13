using CodeStage.AntiCheat.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[UIPanelPrefabAttr("Popup/Setting/PopupSetting", "PriorityCanvas")]
public class PopupSetting : BasePanel
{
    [SerializeField]
    private ButtonCustom btMusicOn, btMusicOff, btFxOn, btFxOff, btThemeOn, btThemeOff, btCheatOn, btCheatOff;
    [SerializeField]
    private ButtonCustom btRestore, btClose;
    [SerializeField]
    private GameObject obMusicOn, obMusicOff;
    [SerializeField]
    private GameObject obThemeOn, obThemeOff;
    [SerializeField]
    private GameObject obFxOn, obFxOff;
    [SerializeField]
    private GameObject obCheat, obCheatOn, obCheatOff;

    protected override void Awake()
    {
        base.Awake();
        SetBtEvents(btMusicOn, ClickMusic);
        SetBtEvents(btMusicOff, ClickMusic);
        SetBtEvents(btFxOn, ClickFx);
        SetBtEvents(btFxOff, ClickFx);
        SetBtEvents(btThemeOn, ClickTheme);
        SetBtEvents(btThemeOff, ClickTheme);
#if UNITY_IPHONE
        btRestore.gameObject.SetActive(true);
#else
        btRestore.gameObject.SetActive(false);
#endif
        SetBtEvents(btRestore, ClickRestore);
        SetBtEvents(btClose, HidePanel);
       

        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        bool is_dev = environment == EnvironmentType.dev;
        if (is_dev)
        {
            SetBtEvents(btCheatOn, ClickCheat);
            SetBtEvents(btCheatOff, ClickCheat);
        }

        SetActive(obCheat, is_dev);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        bool music_mute = TPRLSoundManager.Instance.isMusicMute;
        SetActive(obMusicOn, !music_mute);
        SetActive(obMusicOff, music_mute);
        bool fx_mute = TPRLSoundManager.Instance.isFxMuted;
        SetActive(obFxOn, !fx_mute);
        SetActive(obFxOff, fx_mute);
        bool theme_open = ObscuredPrefs.Get<bool>("ThemeOpen", false);
        SetActive(obThemeOn, theme_open);
        SetActive(obThemeOff, !theme_open);
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        bool is_dev = environment == EnvironmentType.dev;
        if (is_dev)
        {
            bool is_enable_cheat_mode = UserDatas.isEnableCheatMode;
            SetActive(obCheatOn, is_enable_cheat_mode);
            SetActive(obCheatOff, !is_enable_cheat_mode);
        }
    }

    private void SetBtEvents(ButtonCustom bt, UnityAction action)
    {
        if (bt != null)
            bt.onClick = action;
    }

    private void ClickTheme()
    {
        bool active = obThemeOn.activeSelf;
        active = !active;
        Observer.Instance.Notify(ObserverKey.ThemeClick, active);
        SetActive(obThemeOn, active);
        SetActive(obThemeOff, !active);
    }

    private void ClickMusic()
    {
        bool active = obMusicOn.activeSelf;
        active = !active;
        SetActive(obMusicOn, active);
        SetActive(obMusicOff, !active);
        TPRLSoundManager.Instance.SetMusicMute(!active);
    }

    private void ClickFx()
    {
        bool active = obFxOn.activeSelf;
        active = !active;
        SetActive(obFxOn, active);
        SetActive(obFxOff, !active);
        TPRLSoundManager.Instance.SetSoundMute(!active);
    }

    private void SetActive(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }

    private void ClickRestore()
    {
        PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        popUpNotice.OnSetTextTwoButtonCustom("", "Restore ?", delegate { IAPManager.instance.RestorePurchases(); });
        HidePanel();
    }

    private void ClickCheat()
    {
        bool active = obCheatOn.activeSelf;
        active = !active;
        SetActive(obCheatOn, active);
        SetActive(obCheatOff, !active);
        UserDatas.isEnableCheatMode = active;
    }
 }
