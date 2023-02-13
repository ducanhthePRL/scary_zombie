using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[UIPanelPrefabAttr("Popup/Tutorial/PopUpTutorial", "PriorityCanvas")]
public class PopupTutorial : BasePanel
{
    [SerializeField] private RectTransform Tut1,Tut2,Tut3;
    [SerializeField] private TMP_Text text;
    [SerializeField] private Button nextButton;
    [SerializeField] private RectTransform buttonRight, buttonUp;

    public int currentStep = 1;
    private string ClickButtonString = "Click button to move player";
    private string[] DesString = { "Here is destination","Move to this to complete level!" };

    [SerializeField] private RectTransform circleTransform;
    [SerializeField] private RectTransform arrow;

    public int startSize = 2400;
    public int stopSize = 200;

    protected override void OnEnable()
    {
        base.OnEnable();
        AdsManager.instance.ShowBanner(false);
    }

    protected override void Start()
    {
        nextButton.onClick.AddListener(()=> {
            currentStep++;
            PlayTutorial(); 
        });
    }
    public void PlayTutorial()
    {
        StopAllTutorial();
        if (currentStep == 1 || currentStep == 4)
        {
            text.text = ClickButtonString;
            circleTransform.sizeDelta = 2400 * Vector2.one;
            Vector2 targetPosition = CanvasManager.instance.buttonRight.anchoredPosition;
            Tut1.anchoredPosition = targetPosition;
            arrow.anchoredPosition = targetPosition + 200*Vector2.one;
            LeanTween.moveLocal(circleTransform.gameObject, targetPosition, 1);
            LeanTween.size(circleTransform, new Vector2(200, 200), 1f).setOnComplete(() => {
                Tut1.gameObject.SetActive(true);
                arrow.gameObject.SetActive(true);
            });
        }
        else if(currentStep == 2 || currentStep ==3)
        {
            text.text = DesString[currentStep - 2];
            circleTransform.sizeDelta = 2400 * Vector2.one;
            Dest dest = FindObjectOfType<Dest>();
            Vector3 destScreenPoint = Camera.main.WorldToScreenPoint(dest.transform.position);
            Vector2 targetPosition = new Vector2(destScreenPoint.x - Screen.width / 2, destScreenPoint.y - Screen.height / 2);
            Tut3.anchoredPosition = new Vector2(Screen.width / 2, -Screen.height / 2);
            arrow.anchoredPosition = targetPosition + 200 * Vector2.one;
            LeanTween.moveLocal(circleTransform.gameObject, targetPosition, 1);
            LeanTween.size(circleTransform, new Vector2(200, 200), 1f).setOnComplete(() => {
                Tut3.gameObject.SetActive(true);
                arrow.gameObject.SetActive(true);
            });
        }
        else if (currentStep == 5)
        {
            text.text = ClickButtonString;
            circleTransform.sizeDelta = 2400 * Vector2.one;
            Vector2 targetPosition = CanvasManager.instance.buttonUp.anchoredPosition;
            Tut2.anchoredPosition = targetPosition;
            arrow.anchoredPosition = targetPosition + 200 * Vector2.one;
            LeanTween.moveLocal(circleTransform.gameObject, targetPosition, 1);
            LeanTween.size(circleTransform, new Vector2(200, 200), 1f).setOnComplete(() => {
                Tut2.gameObject.SetActive(true);
                arrow.gameObject.SetActive(true);
            });
        }
    }
    public void StopAllTutorial()
    {
        Tut1.gameObject.SetActive(false);
        Tut2.gameObject.SetActive(false);
        Tut3.gameObject.SetActive(false);
        arrow.gameObject.SetActive(false);
    }
    public void ArrowClick(string key)
    {
        Observer.Instance.Notify(ObserverKey.ArrowButtonClick, key);
        HidePanel();
    }
}
