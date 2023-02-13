using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[UIPanelPrefabAttr("Popup/SuccessfulBuy/PopUpSuccessBuy", "PriorityCanvas")]
public class PopupSucessfulBuy : BasePanel
{
    [SerializeField]
    private Image iconItem;
    [SerializeField]
    private TextMeshProUGUI txContent;
    [SerializeField]
    private TextMeshProUGUI txTitle;
    [SerializeField]
    private ButtonCustom btOk;
    protected override void Start()
    {
        base.Start();
        if (btOk != null)
            btOk.onClick = ClickExit;
    }
    private void ClickExit()
    {
        HidePanel();
    }
    public void Init(RecordItem item, string title = "UNLOCKER",string content = "")
    {
        SetImage(iconItem, item.icon);
        SetText(txTitle, title);
        SetText(txContent, content);
    }
    private void SetImage(Image img, string icon)
    {
        if (img != null)
        {
            img.sprite = TexturesManager.Instance.GetSprites(icon);
            img.color = new Color(1, 1, 1, 1);
            img.preserveAspect = true;
        }
    }
    private void SetText(TextMeshProUGUI text, string content)
    {
        if (text != null)
            text.text = content;
    }
}
