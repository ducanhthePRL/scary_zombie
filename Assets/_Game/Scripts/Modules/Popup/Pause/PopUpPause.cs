using CodeStage.AntiCheat.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[UIPanelPrefabAttr("Popup/Setting/PopupPause", "PriorityCanvas")]
public class PopUpPause : BasePanel
{
    [SerializeField]
    private ButtonCustom btMusicOn, btMusicOff, btFxOn, btFxOff, btThemeOn, btThemeOff;
    [SerializeField]
    private ButtonCustom btExit, btClose;
    [SerializeField]
    private GameObject obMusicOn, obMusicOff;
    [SerializeField]
    private GameObject obThemeOn, obThemeOff;
    [SerializeField]
    private GameObject obFxOn, obFxOff;

    protected override void Awake()
    {
        base.Awake();
        SetBtEvents(btMusicOn, ClickMusic);
        SetBtEvents(btMusicOff, ClickMusic);
        SetBtEvents(btFxOn, ClickFx);
        SetBtEvents(btFxOff, ClickFx);
        SetBtEvents(btThemeOn, ClickTheme);
        SetBtEvents(btThemeOff, ClickTheme);
        SetBtEvents(btExit, ClickBackMainMenu);
        SetBtEvents(btClose, HidePanel);
        bool theme_open = ObscuredPrefs.Get<bool>("ThemeOpen", false);
        SetActive(obThemeOn, theme_open);
        SetActive(obThemeOff, !theme_open);
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
        SetActive(obThemeOn, active);
        SetActive(obThemeOff, !active);
        Observer.Instance.Notify(ObserverKey.ThemeClick, active);
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

    private void ClickBackMainMenu()
    {
        PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        popUpNotice.OnSetTextTwoButtonCustom("", "Back to Main menu ?", delegate {
            Ultis.ShowFade(true, delegate {
                AdsManager.instance.ShowInter(delegate
                {
                    ScenesManager.Instance.GetScene(AllSceneName.MainMenu, false);
                });
            });
        });
        HidePanel();
    }

    private void ClickExit()
    {
        PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        popUpNotice.OnSetTextTwoButtonCustom("", "Are you sure want to exit game ?", delegate { Application.Quit(); });
        HidePanel();
    }
}
