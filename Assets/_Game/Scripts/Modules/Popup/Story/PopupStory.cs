using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[UIPanelPrefabAttr("Popup/Story/PopupStory", "PriorityCanvas")]
public class PopupStory : BasePanel
{
    [SerializeField]
    private ButtonCustom btSkip;
    [SerializeField]
    private CanvasGroup canvasGroup;

    protected override void Awake()
    {
        base.Awake();
        if (btSkip != null)
            btSkip.onClick = SkipStory;
        if (canvasGroup != null)
            canvasGroup.alpha = 0;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Ultis.ShowFade(false, Show);
    }

    private void Show()
    {
        if (canvasGroup != null)
            canvasGroup.LeanAlpha(1, 1.5f);
    }

    private void SkipStory()
    {
        Ultis.ShowFade(true, delegate {
            ScenesManager.Instance.GetSceneAsync(AllSceneName.MainMenu, false, null, true);
            PanelManager.Hide(this, true);
        });
    }
}
