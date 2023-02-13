using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockLevelClick : MonoBehaviour
{
    private int chapter = 0;
    [SerializeField] private int level;
    private void OnMouseDown()
    {
        if (UserDatas.user_Data.current_progress.level < level) return;
        PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        popUpNotice.OnSetTextTwoButton("Start Level", $"Do you want to play level {level}", StartGame, Cancel);
    }
    private void StartGame()
    {
        if(chapter == UserDatas.user_Data.current_progress.chapter && level == UserDatas.user_Data.current_progress.level)
        {
            LevelSelect.chapter = 0;
            LevelSelect.level = 0;
        }
        else
        {
            LevelSelect.chapter = chapter;
            LevelSelect.level = level;
        }
        Play();
    }
    private void Cancel()
    {
        PanelManager.Hide<PopUpNotice>();
    }
    private void Play()
    {
        Ultis.ShowFade(true, LoadSceneGame);
    }

    private void LoadSceneGame()
    {
        ScenesManager.Instance.GetScene(AllSceneName.GamePlayMain, false);
        ScenesManager.Instance.GetScene(AllSceneName.GamePlayCanvas, true);
    }
}
