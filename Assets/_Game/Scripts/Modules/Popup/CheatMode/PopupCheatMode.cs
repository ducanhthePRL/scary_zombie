using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

[UIPanelPrefabAttr("Popup/CheatMode/PopupCheatMode", "PriorityCanvas")]
public class PopupCheatMode : BasePanel
{
    [SerializeField]
    private ButtonCustom btClose, btApply;
   [SerializeField]
    private ButtonCustom btUIOn, btUIOff, btHintOn, btHintOff;
    [SerializeField]
    private GameObject obUIOn, obUIOff;
    [SerializeField]
    private GameObject obHintOn, obHintOff;
    private bool isEnableUI = false;
    private bool isEnableHint = false;
    [SerializeField]
    private TMP_InputField inputLevel, inputGold;

    protected override void Awake()
    {
        base.Awake();
        SetBtEvents(btUIOn, ClickUI);
        SetBtEvents(btUIOff, ClickUI);
        SetBtEvents(btHintOn, ClickHint);
        SetBtEvents(btHintOff, ClickHint);
        SetBtEvents(btApply, ClickApply);
        SetBtEvents(btClose, HidePanel);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetActive(obUIOn, isEnableUI);
        SetActive(obUIOff, !isEnableUI);
        SetActive(obHintOn, isEnableHint);
        SetActive(obHintOff, !isEnableHint);
        SetText(inputLevel, "");
        SetText(inputGold, "");
    }

    private void SetText(TMP_InputField input, string text)
    {
        if (input != null)
            input.text = text;
    }

    private void SetActive(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }

    private void SetBtEvents(ButtonCustom bt, UnityAction action)
    {
        if (bt != null)
            bt.onClick = action;
    }

    private void ClickUI()
    {
        bool active = obUIOn.activeSelf;
        active = !active;
        Observer.Instance.Notify(ObserverKey.ThemeClick, active);
        SetActive(obUIOn, active);
        SetActive(obUIOff, !active);
    }

    private void ClickHint()
    {
        bool active = obHintOn.activeSelf;
        active = !active;
        Observer.Instance.Notify(ObserverKey.ThemeClick, active);
        SetActive(obHintOn, active);
        SetActive(obHintOff, !active);
    }

    private void ClickApply()
    {
        string level = inputLevel.text;
        if (!string.IsNullOrEmpty(level))
        {
            UserDatas.user_Data.current_progress.level = int.Parse(level);
        }

        string gold = inputGold.text;
        if (!string.IsNullOrEmpty(gold))
        {
            UserDatas.UpdateGold(int.Parse(gold));
        }
    }
}
