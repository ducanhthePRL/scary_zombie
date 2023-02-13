using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Camera mainCamera;
    [SerializeField] private PathFinding pathFinding;
    public List<RecordLevel> recordLevels = new List<RecordLevel>();
    [SerializeField] private bool isManualSetLevel = false;
    [SerializeField] private int levelPlay;

    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.PassLevel, PassLevel);
        Observer.Instance.AddObserver(ObserverKey.FallOut, FallOut);
        Observer.Instance.AddObserver(ObserverKey.Boom, Boom);
        Observer.Instance.AddObserver(ObserverKey.Fire, Fire);
        Observer.Instance.AddObserver(ObserverKey.StartGame, StartGame);
        Observer.Instance.AddObserver(ObserverKey.EnemyDead, EnemyDead);
        Observer.Instance.AddObserver(ObserverKey.PlayerStopMove, PlayerStopMoveCheck);
        Observer.Instance.AddObserver(ObserverKey.ShowHint, ShowHint);
        Observer.Instance.AddObserver(ObserverKey.PlayerCreated, SetPlayerPathFinding);
        Observer.Instance.AddObserver(ObserverKey.Replay, ResetLevel);
        Observer.Instance.AddObserver(ObserverKey.PopupShowed, PopupShowed);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PassLevel, PassLevel);
        Observer.Instance.RemoveObserver(ObserverKey.FallOut, FallOut);
        Observer.Instance.RemoveObserver(ObserverKey.Boom, Boom);
        Observer.Instance.RemoveObserver(ObserverKey.Fire, Fire);
        Observer.Instance.RemoveObserver(ObserverKey.StartGame, StartGame);
        Observer.Instance.RemoveObserver(ObserverKey.EnemyDead, EnemyDead);
        Observer.Instance.RemoveObserver(ObserverKey.PlayerStopMove, PlayerStopMoveCheck);
        Observer.Instance.RemoveObserver(ObserverKey.ShowHint, ShowHint);
        Observer.Instance.RemoveObserver(ObserverKey.PlayerCreated, SetPlayerPathFinding);
        Observer.Instance.RemoveObserver(ObserverKey.Replay, ResetLevel);
        Observer.Instance.RemoveObserver(ObserverKey.PopupShowed, PopupShowed);
    }

    void Start()
    {
        UserDatas.current_scene_type = SceneType.Gameplay;
        AdsManager.instance.ShowBanner(false);
        StartCoroutine(IEInit(null));
    }
    public int currentLevel
    {
        get
        {
#if UNITY_EDITOR
            if (isManualSetLevel)
            {
                return levelPlay;
            }
            else
            {
                return UserDatas.user_Data.current_progress.level;
            }
#else
            return UserDatas.user_Data.current_progress.level;
#endif
        }
        set
        {
            UserDatas.user_Data.current_progress.level = value;
            UserDatas.Save();
        }
    }
    public int currentChapter
    {
        get
        {
            return UserDatas.user_Data.current_progress.chapter;
        }
        set
        {
            UserDatas.user_Data.current_progress.chapter = value;
            UserDatas.Save();
        }
    }
    GameObject currentLevelGO;
    Level crLevel;
    RecordLevel crRecordLevel;
    PopupTimer popupTimer;
    PopupStepCount popupStepCount;
    private bool isPlayBySelectLevel;
    public void StartGame(object data)
    {
       
        if (currentLevelGO != null) Destroy(currentLevelGO);
        if (popupTimer != null) PanelManager.Hide<PopupTimer>();
        if (popupStepCount != null) PanelManager.Hide<PopupStepCount>();
        if (LevelSelect.level <= 0 || LevelSelect.level == currentLevel)
        {
            isPlayBySelectLevel = false;
            crRecordLevel = recordLevels[currentChapter];
            if (currentLevel >= crRecordLevel.level.Count)
            {
                currentLevel = 1;
            }
            currentLevelGO = Instantiate(crRecordLevel.level[currentLevel]);
            crLevel = currentLevelGO.GetComponent<Level>();
            crLevel.level = currentLevel;
            crLevel.Init();

            SetTextLevel(currentLevel);
        }
        else
        {
            isPlayBySelectLevel = true;
            crRecordLevel = recordLevels[LevelSelect.chapter];
            currentLevelGO = Instantiate(crRecordLevel.level[LevelSelect.level]);
            crLevel = currentLevelGO.GetComponent<Level>();
            crLevel.level = currentLevel;
            crLevel.Init();
            SetTextLevel(LevelSelect.level);
        }
        pathFinding.SaveBlockPositions(crLevel.MapSizeWidth, crLevel.MapSizeHeight, crLevel.tf);
        if (crLevel.gameMode == GameMode.TIME_LIMIT)
        {
            popupTimer = PanelManager.Show<PopupTimer>();
            popupTimer.Init(crLevel.timeLimit);
            popupTimer.OnTimeout = Timeout;
        }
        else if (crLevel.gameMode == GameMode.STEP_COUNT)
        {
            popupStepCount = PanelManager.Show<PopupStepCount>();
            popupStepCount.Init(crLevel.numberStep);
        }
        else
        {
            PanelManager.Hide<PopupTimer>();
            PanelManager.Hide<PopupStepCount>();
        }
        ResetMainCamera();
        TPRLSoundManager.Instance.PlayMusic("Ingame");
        LogFirebaseChapterLevel("play");
        //MeshCombiner.Instance.CombineMesh(transform);
        Ultis.ShowFade(false, null);

        if (currentLevel == 0)
        {
            StartCoroutine(OpenPopupTutorial());
        }
    }
    private IEnumerator OpenPopupTutorial()
    {
        yield return new WaitForSeconds(2);
        PopupTutorial popupTutorial = PanelManager.Show<PopupTutorial>();
        popupTutorial.PlayTutorial();
    }
    private void SetTextLevel(int level)
    {
        string text = level > 0 ? $"Level {level}" : "Tutorial";
        CanvasManager.instance.SetTextLevel(text);
    }
    private void ResetMainCamera()
    {
        mainCamera.transform.position = new Vector3(8 + crLevel.MapSizeWidth / 2, 14.5f, -11);
        mainCamera.transform.rotation = Quaternion.Euler(new Vector3(40, -32, -10));

        mainCamera.fieldOfView = 27 + crLevel.MapSizeWidth / 2;
    }
    PopupRate popupRate = null;
    public void PassLevel(object data)
    {
        if (crLevel.currentNumberEnemy <= 0)
        {
            if (isPlayBySelectLevel)
            {
                DoBeforePassLevel();
                PopupReward popupReward = PanelManager.Show<PopupReward>();
                RecordReward recordReward = new RecordReward();
                recordReward.amount = crLevel.GetReward();
                popupReward.recordRewardLevel = recordReward;
                popupReward.StartReward();
                popupReward.SetMode(false);
                
            }
            else
            {
                DoBeforePassLevel();
                if (currentLevel == 10 && !UserDatas.user_Data.is_rate || currentLevel == 5)
                {
                    popupRate = PanelManager.Show<PopupRate>();
                    popupRate.actionRate = () =>
                    {
                        PopupReward popupReward = PanelManager.Show<PopupReward>();
                        RecordReward recordReward = new RecordReward();
                        recordReward.amount = crLevel.GetReward();
                        popupReward.recordRewardLevel = recordReward;
                        popupReward.StartReward();
                        popupReward.SetMode(true);
                    };
                }
                else
                {
                    PopupReward popupReward = PanelManager.Show<PopupReward>();
                    RecordReward recordReward = new RecordReward();
                    recordReward.amount = crLevel.GetReward();
                    popupReward.recordRewardLevel = recordReward;
                    popupReward.StartReward();
                    if (currentLevel < 1)
                    {
                        popupReward.SetMode(false);
                    }
                    else
                    {
                        popupReward.SetMode(true);
                    }
                }
                LogFirebaseChapterLevel("win");
                if (currentLevel == crRecordLevel.level.Count - 1)
                {
                    if (currentChapter <= recordLevels.Count - 1)
                    {
                        if (currentChapter <= recordLevels.Count - 1)
                        {
                            /*currentChapter++;*/
                            currentLevel = 1; //Chapter 1 level 1, level 0 is tutorial
                        }
                    }
                    //not complete all level
                    else
                    {
                        currentLevel++;
                        LogFirebaseClearChapter();
                    }
                }
                else
                {
                    currentLevel++;
                }
            }
        }
    }
    private void DoBeforePassLevel()
    {
        TPRLSoundManager.Instance.StopMusic();
        if (popupTimer != null)
        {
            popupTimer.StopCountdown();
            crLevel.currentTime = popupTimer.currentTime;
        }
        TPRLSoundManager.Instance.PlaySoundFx("Win");
    }
    private void EnemyDead(object data)
    {
        DataEffect dataEffect = (DataEffect)data;
        crLevel.listEnemyObject.Remove(dataEffect.enemy);
        crLevel.currentNumberEnemy--;
        if (crLevel.currentNumberEnemy <= 0)
        {
            Observer.Instance.Notify(ObserverKey.AllEnemyDead, null);
        }
    }
    private void PlayerStopMoveCheck(object data)
    {
        if (data == null)
        {
            if (currentLevel == 0)
            {
                PopupTutorial popupTutorial = PanelManager.Show<PopupTutorial>();
                popupTutorial.currentStep++;
                popupTutorial.PlayTutorial();
            }
            pathFinding.ResetPlayerPosition();
            crLevel.currentStep++;
            if (popupStepCount != null && popupStepCount.gameObject.activeSelf && popupStepCount.isOutOfStep)
            {
                ShowPopupOutOfStep();
            }
        }
    }
    private void ShowPopupOutOfStep()
    {
        PopUpFailed popUpNotice = PanelManager.Show<PopUpFailed>();
        popUpNotice.OnSetTextTwoButton("LOSE", "Out Of Step !", ResetLevel, BackToMainMenu);
        TPRLSoundManager.Instance.PlaySoundFx("Fail");
        LogFirebaseChapterLevel("fail");
    }
    public void Timeout()
    {
        PopUpFailed popUpNotice = PanelManager.Show<PopUpFailed>();
        popUpNotice.OnSetTextTwoButton("LOSE", "Time out !", ResetLevel, BackToMainMenu);
        TPRLSoundManager.Instance.PlaySoundFx("Fail");
        LogFirebaseChapterLevel("fail");
    }
    public void FallOut(object data)
    {
        if (popupTimer != null)
        {
            popupTimer.StopCountdown();
        }
        PopUpFailed popUpNotice = PanelManager.Show<PopUpFailed>();
        popUpNotice.OnSetTextTwoButton("LOSE", "You fell off the map !", ResetLevel, BackToMainMenu);
        TPRLSoundManager.Instance.PlaySoundFx("Fail");
        LogFirebaseChapterLevel("fail");
    }
    public void Boom(object data)
    {
        if (popupTimer != null)
        {
            popupTimer.StopCountdown();
        }
        PopUpFailed popUpNotice = PanelManager.Show<PopUpFailed>();
        popUpNotice.OnSetTextTwoButton("LOSE", "You explode to die !", ResetLevel, BackToMainMenu);
        TPRLSoundManager.Instance.PlaySoundFx("Fail");
    }
    private void BackToMainMenu()
    {
        AdsManager.instance.ShowInter(delegate
        {
            ScenesManager.Instance.GetScene(AllSceneName.MainMenu, false);
        });
    }
    private void ResetLevel(object data)
    {
        PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        popUpNotice.OnSetTextTwoButton("Replay", "Do you want to replay this level?", ResetLevel, ClosePopupNotice);
        LogFirebaseChapterLevel("replay");
    }

    private void ClosePopupNotice()
    {
        PanelManager.Hide<PopUpNotice>();
    }
    public void ResetLevel()
    {
        pathFinding.playerMoveComponent.virtualCamera.SetActive(false);
        ResetMainCamera();
        AdsManager.instance.ShowInter(delegate {
           
            Destroy(currentLevelGO);
            TPRLSoundManager.Instance.PlayMusic("Ingame");
            SetTextLevel(currentLevel);
            if (isPlayBySelectLevel)
            {
                currentLevelGO = Instantiate(crRecordLevel.level[LevelSelect.level]);
                crLevel = currentLevelGO.GetComponent<Level>();
                crLevel.level = LevelSelect.level;
                crLevel.Init();
            }
            else
            {
                currentLevelGO = Instantiate(crRecordLevel.level[currentLevel]);
                crLevel = currentLevelGO.GetComponent<Level>();
                crLevel.level = currentLevel;
                crLevel.Init();
            }
            pathFinding.SaveBlockPositions(crLevel.MapSizeWidth, crLevel.MapSizeHeight, crLevel.tf);
            if (crLevel.gameMode == GameMode.TIME_LIMIT)
            {
                popupTimer = PanelManager.Show<PopupTimer>();
                popupTimer.Init(crLevel.timeLimit);
                popupTimer.OnTimeout = Timeout;
            }
            else if (crLevel.gameMode == GameMode.STEP_COUNT)
            {
                popupStepCount = PanelManager.Show<PopupStepCount>();
                popupStepCount.Init(crLevel.numberStep);
            }
            else
            {
                PanelManager.Hide<PopupTimer>();
                PanelManager.Hide<PopupStepCount>();
            }
            
        });
        
    }
    public void Fire(object data)
    {
        if (popupTimer != null)
        {
            popupTimer.StopCountdown();
        }
        PopUpFailed popUpNotice = PanelManager.Show<PopUpFailed>();
        popUpNotice.OnSetTextTwoButton("LOSE", "You burned to die !", ResetLevel, BackToMainMenu);
        TPRLSoundManager.Instance.PlaySoundFx("Fail");
        LogFirebaseChapterLevel("fail");
    }
    private IEnumerator IEInit(UnityAction action)
    {
        yield return new WaitUntil(() => CanvasManager.instance != null);
        StartGame(null);
        //if (mainCamera != null)
        //{
        //    var cam = mainCamera.GetUniversalAdditionalCameraData();
        //    cam.cameraStack.Add(CanvasManager.instance.canvasCamera);
        //}
        action?.Invoke();
    }
    private void ShowHint(object data)
    {
        crLevel.ShowHint();
        /*if (crLevel.listEnemyObject.Count > 0)
        {
            int randomEnemyIndex = Random.Range(0, crLevel.listEnemyObject.Count);
            pathFinding.target = crLevel.listEnemyObject[randomEnemyIndex];
        }
        else
        {
            pathFinding.target = crLevel.dest;
        }
        pathFinding.ResetTargetPosition();
        pathFinding.Run();*/
    }
    private void SetPlayerPathFinding(object data)
    {
        GameObject player = (GameObject)data;
        ObjectIndex playerOI = player.GetComponent<ObjectIndex>();
        pathFinding.player = playerOI;
        pathFinding.ResetPlayerPosition();
    }
    private void LogFirebaseChapterLevel(string status)
    {
        StringBuilder sbFirebase = new StringBuilder();
        sbFirebase.Append(status);
        sbFirebase.Append("_chap");
        sbFirebase.Append(currentChapter);
        sbFirebase.Append("_level_");
        sbFirebase.Append(currentLevel);
        FirebaseManager.instance.LogEvent(sbFirebase.ToString());
    }
    private void LogFirebaseClearChapter()
    {
        StringBuilder sbFirebase = new StringBuilder();
        sbFirebase.Append("clear_chap_");
        sbFirebase.Append(currentChapter);
        FirebaseManager.instance.LogEvent(sbFirebase.ToString());
    }

    private void PopupShowed(object data)
    {
        if (data == null) return;
        bool active = (bool)data;
        if (!active)
            AdsManager.instance.ShowBanner(active);
    }

}


[System.Serializable]
public class RecordLevel
{
    public int chapter;
    public List<GameObject> level = new List<GameObject>();
}
public static class LevelSelect
{
    public static int chapter;
    public static int level;
}