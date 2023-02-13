using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[UIPanelPrefabAttr("Popup/Hint/PopUpHint", "PriorityCanvas")]
public class PopupHint : BasePanel
{
    [SerializeField] private ButtonCustom bt_ok;
    [SerializeField] private TMP_Text bt_ok_text;
    [SerializeField] private ButtonCustom bt_cancel;
    [SerializeField] private TMP_Text bt_cancel_text;
    public Action actionOk;
    public Action actionCancel;

    protected override void Start()
    {
        base.Start();

        if (bt_ok != null)
            bt_ok.onClick = OnOkButtonClick;

        if (bt_cancel != null)
            bt_cancel.onClick = OnCancelButtonClick;
    }
    public void OnOkButtonClick()
    {
        if (UserDatas.isEnableCheatMode)
        {
            ShowHint();
            HidePanel();
            return;
        }
#if UNITY_EDITOR
        ShowHint();
        HidePanel();
#else
        AdsManager.instance.ShowReward(OnRewardSuccess, OnRewardFailed);
#endif
        FirebaseManager.instance.LogEvent("click_ads_hint");
    }
    public void OnCancelButtonClick()
    {
        HidePanel();
    }
    private void ShowHint()
    {
        Observer.Instance.Notify(ObserverKey.ShowHint, null);
    }
    private void OnRewardSuccess()
    {
        ShowHint();
        AdsManager.instance.ResetAdsCounter();
        HidePanel();
    }
    private void OnRewardFailed()
    {
        PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        popUpNotice.OnSetTextOneButton("Failed", "Claim Reward Failure", OnCancelButtonClick, "OK");
    }
}
