using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Text;

public class VideoGoldManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text txTimeRemain;
    [SerializeField]
    private ItemVideoGold[] itemVideoGolds;
    private long timeRemain = 0;
    private DateTime dtNow;
    private void Awake()
    {
        SubmitButton();
    }
    private void SubmitButton()
    {
        if (itemVideoGolds == null) return;
        int length = itemVideoGolds.Length;
        for (int i = 0; i < length; i++)
        {
            if (itemVideoGolds[i] != null)
            {
                int index_copy = i;
                itemVideoGolds[index_copy].actionClick = () => { AdsManager.instance.ShowReward(OnDoneReward, OnFailedReward); };
                itemVideoGolds[index_copy].Interactable(false);
            }
        }
    }
    private void OnDoneReward()
    {
        if (itemVideoGolds[UserDatas.user_Data.levelVideoGold] == null) return;
        int reward = itemVideoGolds[UserDatas.user_Data.levelVideoGold].coinAdd;
        RecordItem record = new RecordItem();
        if(UserDatas.user_Data.levelVideoGold <= 2)
            record.icon = "icons/gold_1";
        else
            record.icon = "icons/gold_2";
        record.name = $"{reward} Gold";
        PopupSucessfulBuy popup = PanelManager.Show<PopupSucessfulBuy>();
        popup.Init(record,"REWARD",$"{record.name}");
        itemVideoGolds[UserDatas.user_Data.levelVideoGold].Interactable(false);
        UserDatas.user_Data.levelVideoGold++;
        UserDatas.UpdateGold(reward);
        if (UserDatas.user_Data.levelVideoGold < itemVideoGolds.Length && itemVideoGolds[UserDatas.user_Data.levelVideoGold] !=null)
        {
            itemVideoGolds[UserDatas.user_Data.levelVideoGold].Interactable(true);
        }
    }
    private void OnFailedReward()
    {
        PopUpFailed popUpNotice = PanelManager.Show<PopUpFailed>();
        popUpNotice.OnSetTextOneButton("FAILED", "Network problem !", () => { PanelManager.Hide<PopUpFailed>(); } );
    }
    
    private void SetSessionTimeText(int time)
    {
        int hh = (int)time / 3600;
        int mm = (int)(time - hh * 3600) / 60;
        int ss = time - 3600 * hh - 60 * mm;

        string sessionTime = (hh >= 10 ? hh.ToString() : "0" + hh) + ":" +
            (mm >= 10 ? mm.ToString() : "0" + mm) + ":" +
            (ss >= 10 ? ss.ToString() : "0" + ss);
        if (txTimeRemain)
            txTimeRemain.text = sessionTime;
    }
    private void SubTime()
    {
        dtNow = DateTime.Now;
        UserDatas.user_Data.time_current.year = dtNow.Year;
        UserDatas.user_Data.time_current.month = dtNow.Month;
        UserDatas.user_Data.time_current.day = dtNow.Day;
        DateTime dtHourTomorow = DateTime.Now.AddDays(1);
        DateTime dtTomorow = new DateTime(dtHourTomorow.Year, dtHourTomorow.Month, dtHourTomorow.Day,0,0,0);
        long currentTime = ((DateTimeOffset)dtNow).ToUnixTimeSeconds();
        long tomorrowTime = ((DateTimeOffset)dtTomorow).ToUnixTimeSeconds();
        timeRemain = tomorrowTime - currentTime;
    }
    private void OnEnable()
    {
        dtNow = DateTime.Now;
        if (dtNow.Year > UserDatas.user_Data.time_current.year)
        {
            ResetVideoGold();
        }
        else if(dtNow.Month > UserDatas.user_Data.time_current.month)
        {
            ResetVideoGold();
        }
        else if(dtNow.Day > UserDatas.user_Data.time_current.day)
        {
            ResetVideoGold();
        }
        SubTime();
        StartCoroutine(CountTimeVideoGold());
        if (UserDatas.user_Data.levelVideoGold < itemVideoGolds.Length && itemVideoGolds[UserDatas.user_Data.levelVideoGold] != null)
            itemVideoGolds[UserDatas.user_Data.levelVideoGold].Interactable(true);
    }
    private IEnumerator CountTimeVideoGold()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            SetSessionTimeText((int)timeRemain);
            timeRemain--;
            if (timeRemain <= 1)
            {
                SubTime();
                ResetVideoGold();
            }
        }
    }
    private void ResetVideoGold()
    {
        if (itemVideoGolds == null) return;
        int length = itemVideoGolds.Length;
        for (int i = 0; i < length; i++)
        {
            if (itemVideoGolds[i] != null)
                itemVideoGolds[i].Interactable(false);
        }
        UserDatas.user_Data.levelVideoGold = 0;
        if (itemVideoGolds[UserDatas.user_Data.levelVideoGold] != null)
            itemVideoGolds[UserDatas.user_Data.levelVideoGold].Interactable(true);
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
    private void OnDestroy()
    {
        StopAllCoroutines();
    }
}
