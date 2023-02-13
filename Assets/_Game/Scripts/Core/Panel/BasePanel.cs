using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    [SerializeField]
    protected Transform main;
    [SerializeField]
    protected LeanTweenType tweenType = LeanTweenType.easeSpring;

    protected virtual void Awake()
    {
        //PanelManager.Register(this);
        if (main != null)
            main.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }

    protected virtual void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.offsetMax = Vector2.zero;
            rectTransform.offsetMin = Vector2.zero;
        }
    }

    protected virtual void OnEnable()
    {
        if(main != null)
        {
            main.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            main.LeanScale(Vector3.one, 0.4f).setEase(tweenType);
        }
        AdsManager.instance.ShowBanner(true);
        Observer.Instance.Notify(ObserverKey.PopupShowed, true);
    }
    private void OnDisable()
    {
        Observer.Instance.Notify(ObserverKey.PopupShowed, false);
    }

    public virtual void HidePanel()
    {
        Observer.Instance.Notify(ObserverKey.PopupShowed, false);
        PanelManager.Hide(this);
    }
}
