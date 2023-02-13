using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;

public class CanvasManager : SingletonMono<CanvasManager>
{
    [SerializeField]
    private Camera _canvasCamera;
    public Camera canvasCamera => _canvasCamera;
    [SerializeField]
    private TextMeshProUGUI txLevel;
    [SerializeField]
    private Animator textLevelAnimator;
    [SerializeField]
    private GameObject activeAfterObject;
    public RectTransform buttonLeft, buttonRight, buttonUp, buttonDown;
    [SerializeField]
    private ButtonCustom btShowHideUI;
    [SerializeField]
    private GameObject obTop, obMiddle, obBot;

    protected override void Awake()
    {
        base.Awake();
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        if (environment == EnvironmentType.dev && UserDatas.isEnableCheatMode)
        {
            SetActive(btShowHideUI.gameObject, true);
            SetEventBt(btShowHideUI, ClickShowHideUI);
        }
        else
            SetActive(btShowHideUI.gameObject, false);
    }

    protected override void OnDestroy()
    {
        StopAllCoroutines();
        base.OnDestroy();
    }

    public void SetTextLevel(string text)
    {
        if (txLevel != null)
        {
            if (activeAfterObject != null)
            {
                activeAfterObject.SetActive(false);
            }
            txLevel.text = text;
            textLevelAnimator.Play("TextLevel");
        }
    }

    private void SetActive(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }

    private void SetEventBt(ButtonCustom bt, UnityAction action)
    {
        if (bt != null)
            bt.onClick = action;
    }

    private void ClickShowHideUI()
    {
        bool active = obTop.activeSelf;
        active = !active;
        SetActive(obTop, active);
        SetActive(obMiddle, active);
        SetActive(obBot, active);
    }
}
