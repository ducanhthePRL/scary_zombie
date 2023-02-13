using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

[UIPanelPrefabAttr("Popup/Reward/PopupReward", "PriorityCanvas")]
public class PopupReward : BasePanel
{
    [SerializeField]
    private ButtonCustom btX2Reward, btContinue, btExit, btContinue0;
    [SerializeField]
    private GameObject AdsZone;
    [SerializeField]
    private TextMeshProUGUI txAdsX;
    private RecordReward recordReward;
    [SerializeField]
    private TextMeshProUGUI txReward, txGold;
    [Header("GoldUI")]
    [SerializeField]
    private GameObject goldUI;
    [SerializeField]
    private TextMeshProUGUI txXGold;
    [Header("Slider Reward")]
    [SerializeField]
    private RectTransform rtMove;
    [SerializeField]
    private RectTransform rtLine;
    private Vector2 pointLeft;
    private Vector2 pointRight;
    private float timeRun = 0.5f;
    private int levelReward = 1;
    public RecordReward recordRewardLevel;
    private int amountReward = 0;
    protected override void Awake()
    {
        base.Awake();
        SetBtEvents(btX2Reward, ClickAds);
        SetBtEvents(btContinue, ClickContinue);
        SetBtEvents(btContinue0, ClickContinue0);
        SetBtEvents(btExit, ClickExit);
        float widthLine = rtLine.rect.width;
        pointLeft = new Vector2((-widthLine) / 2, 30);
        pointRight = new Vector2((widthLine) / 2, 30);
    }
    public void SetMode(bool hasAd)
    {
        SetActiveOb(btContinue0.gameObject, !hasAd);
        SetActiveOb(AdsZone, hasAd);
        SetActiveOb(btExit.gameObject, hasAd);
    }
    public void StartReward()
    {
        SetActiveOb(goldUI, false);
        SetText(txAdsX, "");
        if (rtMove != null)
            rtMove.localPosition = pointRight;
        StartMove();
        SetActiveOb(btX2Reward.gameObject, true);
        SetActiveOb(btContinue.gameObject, true);
        SetText(txGold, $"{UserDatas.user_Data.currency.gold}");
        SetInfo(recordRewardLevel);
    }

    private void Reset()
    {
        SetScale(btX2Reward.transform, Vector3.zero);
        SetScale(btContinue.transform, Vector3.zero);
        SetText(txReward, "0");
    }

    public void SetInfo(RecordReward reward)
    {
        StopAllCoroutines();
        Reset();
        StartCoroutine(IESetInfo(reward));
    }

    private IEnumerator IESetInfo(RecordReward reward)
    {
        recordReward = reward;
        LeanTween.scale(btX2Reward.gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.5f).setEase(LeanTweenType.easeSpring);
        yield return new WaitForSeconds(3f);
        btContinue.gameObject.SetActive(true);
        SetText(txReward, $"{recordReward.amount} ");
        LeanTween.scale(btContinue.gameObject, new Vector3(0.8f, 0.8f, 0.8f), 0.5f).setEase(LeanTweenType.easeSpring);
    }
    private void ClickAds()
    {
        rtMove.LeanCancel();
        AdsManager.instance.ShowReward(OnRewardSuccess, OnRewardFailed);
    }
    private void ClickContinue()
    {
        SetActiveOb(AdsZone, false);
        int currentGold = UserDatas.user_Data.currency.gold;
        int reward = recordReward.amount;
        UserDatas.UpdateGold(reward);
        StartCoroutine(GoldUpdate(currentGold, UserDatas.user_Data.currency.gold, false));
    }
    private void ClickContinue0()
    {
        Ultis.ShowFade(true, delegate { ScenesManager.Instance.GetScene(AllSceneName.MainMenu, false); });
        HidePanel();
    }
    private void ClickExit()
    {
        int reward = recordReward.amount;
        UserDatas.UpdateGold(reward);
        Ultis.ShowFade(true, delegate
        {
            AdsManager.instance.ShowInter(delegate
            {
                ScenesManager.Instance.GetScene(AllSceneName.MainMenu, false);
            });

        });
        HidePanel();
    }

    private void OnRewardSuccess()
    {
        int currentGold = UserDatas.user_Data.currency.gold;
        int reward = recordReward.amount;
        reward *= levelReward;
        UserDatas.UpdateGold(reward);
        StartCoroutine(GoldUpdate(currentGold, UserDatas.user_Data.currency.gold, true));
    }
    private void OnRewardFailed()
    {
        int reward = recordReward.amount;
        UserDatas.UpdateGold(reward);
        BackToMainMenu();
    }

    private void BackToMainMenu()
    {
        Ultis.ShowFade(true, delegate
        {
            ScenesManager.Instance.GetScene(AllSceneName.MainMenu, false);
        });
        HidePanel();
    }
    private IEnumerator GoldUpdate(int currentGold, int nextGold, bool is_watchAds)
    {
        SetActiveOb(txXGold.gameObject, false);
        SetActiveOb(AdsZone, false);
        SetActiveOb(goldUI, true);
        int goldStep = (nextGold - currentGold) / 20;
        if (is_watchAds)
        {
            SetText(txXGold, $"x{levelReward}");
            SetActiveOb(txXGold.gameObject, true);
            if (txXGold != null)
            {
                txXGold.gameObject.transform.localScale = new Vector3(8, 8, 8);
                txXGold.gameObject.LeanScale(Vector3.one, 0.5f);
            }
            yield return new WaitForSeconds(0.5f);
        }
        if (txGold != null)
        {
            txGold.gameObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            txGold.transform.LeanScale(new Vector3(1.5f, 1.5f, 1.5f), 0.3f).setEaseSpring().setOnComplete(() =>
            {
                txGold.transform.LeanScale(new Vector3(1, 1, 1), 0.3f);
            });
        }
        while (currentGold < nextGold)
        {
            TPRLSoundManager.Instance.PlaySoundFx("Coin");
            SetText(txGold, $"{currentGold}");
            currentGold += goldStep;
            yield return new WaitForSeconds(0.08f);
        }
        SetText(txGold, $"{nextGold}");
        yield return new WaitForSeconds(2f);
        Ultis.ShowFade(true, delegate
        {
            if (is_watchAds)
            {
                Observer.Instance.Notify(ObserverKey.StartGame, null);
                AdsManager.instance.ResetAdsCounter();
            }
            else
            {
                AdsManager.instance.ShowInter(delegate
                {
                    Observer.Instance.Notify(ObserverKey.StartGame, null);
                });
            }
            HidePanel();
        });
    }
    #region SliderRun
    private void StartMove()
    {
        float widthLine = rtLine.rect.width;
        float time_run_rate = widthLine / -(pointLeft.x - rtMove.localPosition.x);
        rtMove.LeanMoveLocal(pointLeft, timeRun / time_run_rate).setEaseLinear().setOnComplete(() =>
        {
            rtMove.LeanMoveLocal(pointRight, timeRun).setEaseLinear().setOnComplete(() =>
            {
                StartMove();
            }).setOnUpdate(SetTextReward); ;
        }).setOnUpdate(SetTextReward);
    }
    private void SetTextReward(float value)
    {
        if (value >= 0 && value < 0.25 || value >= 0.75 && value <= 1)
        {
            levelReward = 2;
        }
        else if (value >= 0.25 && value < 0.45 || value >= 0.55 && value < 0.75) { levelReward = 3; }
        else if (value >= 0.45 && value < 0.55) { levelReward = 4; }
        amountReward = recordReward.amount * levelReward;
        SetText(txAdsX, $"{amountReward}");
    }
    #endregion
    private void OnDestroy()
    {
        rtMove.LeanCancel();
        StopAllCoroutines();
    }

    private void OnDisable()
    {
        rtMove.LeanCancel();
        StopAllCoroutines();
    }
    #region CheckNull
    private void SetText(TextMeshProUGUI text, string content)
    {
        if (text != null)
            text.text = content;
    }
    private void SetActiveOb(GameObject ob, bool status)
    {
        if (ob != null)
            ob.SetActive(status);
    }
    private void SetBtEvents(ButtonCustom bt, UnityAction action)
    {
        if (bt != null)
            bt.onClick = action;
    }

    private void SetScale(Transform tf, Vector3 scale)
    {
        if (tf != null)
            tf.localScale = scale;
    }
    #endregion
}

