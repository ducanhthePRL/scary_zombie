using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[UIPanelPrefabAttr("Popup/Timer/PopupTimer", "Popup")]
public class PopupTimer : BasePanel
{
    [SerializeField] private TMP_Text timeText;
    private int maxTime;
    [HideInInspector] public int currentTime;
    public Action OnTimeout;
    private Coroutine countdownCor;

    protected override void OnEnable()
    {
        base.OnEnable();
        AdsManager.instance.ShowBanner(false);
    }

    public void Init(int time)
    {
        maxTime = time;
        StartCountdown();
    }
    public void StartCountdown()
    {
        currentTime = maxTime;
        timeText.text = secondToTime(currentTime);
        countdownCor = StartCoroutine(Countdown());
    }
    public void StopCountdown()
    {
        StopCoroutine(countdownCor);
    }
    private string secondToTime(int time)
    {
        int mm = (int)(time / 60);
        int ss = time - 60 * mm;
        return (mm >= 10 ? mm.ToString() : ("0" + mm)) + ":" + (ss >= 10 ? ss.ToString() : ("0" + ss));
    }
    private IEnumerator Countdown()
    {
        while (currentTime > 0)
        {
            yield return new WaitForSeconds(1);
            currentTime--;
            timeText.text = secondToTime(currentTime);
            if (currentTime <= 0)
            {
                OnTimeout?.Invoke();
            }
        }
    }
}
