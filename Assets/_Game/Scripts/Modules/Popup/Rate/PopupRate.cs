using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[UIPanelPrefabAttr("Popup/Rate/PopupRate", "PriorityCanvas")]
public class PopupRate : BasePanel
{
    [SerializeField]
    private ButtonCustom btRate, btLater;
    [Header("Star")]
    [SerializeField]
    private Image [] btStars;
    [SerializeField]
    private Sprite spriteShadow,spriteLight;
    [SerializeField]
    private GameObject fiveStars;
    [SerializeField]
    private TextMeshProUGUI textThanks;
    private int indexStar = 0;
    

    protected override void Start()
    {
        base.Start();
        ClickButton(btRate,ClickRate);
        ClickButton(btLater, ClickLater);
        if (btStars != null)
        {
            int lenght = btStars.Length;
            for (int i = 0; i < lenght; i++)
            {
                int i_copy = i;
                ClickButton(TransButton(btStars[i_copy]), () => { ClickStar(i_copy); }) ;
            }
        }
        if (btRate != null)
        {
            btRate.Interactable(false);
        }
        SetActiveOb(fiveStars, true);
        SetActiveOb(textThanks.gameObject, false);
    }

    private void SetActiveOb(GameObject obj,bool status)
    {
        if (obj != null)
            obj.SetActive(status);
    }
    private void SetText(TextMeshProUGUI text, string str)
    {
        if (text != null)
        {
            text.text = str;
        }
    }
    private void ClickButton(ButtonCustom bt, UnityAction action)
    {
        if (bt != null)
        {
            bt.onClick = action;
        }
    }
    private ButtonCustom TransButton(Image image)
    {
        ButtonCustom bt = image.GetComponent<ButtonCustom>();
        return bt;
    }
    public UnityAction actionRate = null;
    private void ClickRate()
    {
        if (indexStar >= 5)
        {
            Application.OpenURL(EnvironmentConfig.linkGameStore);
        }
        else
        {
            SetActiveOb(fiveStars, false);
            SetActiveOb(textThanks.gameObject, true);
            string detail = null;
            detail = "Thanks for the rating";
            SetText(textThanks, detail);
        }
        UserDatas.user_Data.is_rate = true;
        btRate.Interactable(false);
        StartCoroutine(WaitToClosePanel(1));

    }
    IEnumerator WaitToClosePanel(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        if (actionRate != null)
            actionRate?.Invoke();
        HidePanel();
    }
    private void ClickLater()
    {
        HidePanel(); 
        if (actionRate != null)
            actionRate?.Invoke();
    }
    private void ClickStar(int index)
    {
        if (spriteLight == null || spriteShadow == null) return;
        btRate.Interactable(true);
        indexStar = index+1;
        int lenght = btStars.Length;
        if (index >= lenght) return;
        for (int i = 0; i < lenght; i++)
        {
            btStars[i].sprite = spriteShadow;
            if (i <= index)
            {
                btStars[i].sprite = spriteLight;
            }
        }
    }
}
