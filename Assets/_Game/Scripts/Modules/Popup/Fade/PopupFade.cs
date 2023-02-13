using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UIPanelPrefabAttr("Popup/Fade/PopupFade", "PriorityCanvas")]
public class PopupFade : BasePanel
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    public void Show(Action on_done)
    {
        if(canvasGroup != null)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 0;
            canvasGroup.LeanAlpha(1, 1f).setOnComplete(on_done);
        }
    }

    public void Hide(Action on_done)
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1;
            canvasGroup.LeanAlpha(0, 1f).setOnComplete(()=> {
                canvasGroup.gameObject.SetActive(false);
                on_done?.Invoke();
            });
        }
    }
}
