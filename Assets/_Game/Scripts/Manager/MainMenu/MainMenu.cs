using System.Collections;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private ItemLevelMainMenu[] itemLevelMainMenus;
    [SerializeField]
    private TextMeshProUGUI txChapter, txLevel;
    [SerializeField]
    private ButtonCustom btNextChapter, btBackChapter;
    [SerializeField]
    private ButtonCustom btPlay;
    private ObscuredInt currentChapter = 0;
    private ObscuredInt maxChapter = 5;
    private List<ItemLevelMainMenu> queueOrderItemLevels = new List<ItemLevelMainMenu>();
    private float[] originItemLevelPosX = new float[3];
    private ObscuredBool isClickingChangeChapter = false;
    private WorldmapMainMenu worldmapMainMenu = null;
    [SerializeField]
    private PlayerFakeManager playerFakeManager;
    [SerializeField]
    private GameObject hideClickLevelOb;
    [SerializeField]
    private GameObject btCheatMode;
    [SerializeField]
    private ButtonCustom btShowHideUI;
    [SerializeField]
    private GameObject obTop, obMiddle, obBot;

    private void Awake()
    {
        SetEventBt(btNextChapter, ClickNextChapter);
        SetEventBt(btBackChapter, ClickBackChapter);
        SetEventBt(btPlay, ClickPlay);
        Observer.Instance.AddObserver(ObserverKey.PopupShowed, SetStatusHideClickLevel);
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        if (environment == EnvironmentType.dev)
        {
            Observer.Instance.AddObserver(ObserverKey.EnableCheatMode, EnableCheatMode);
            SetEventBt(btShowHideUI, ClickShowHideUI);
        }
    }

    private void OnDestroy()
    {
        ClearItems();
        Observer.Instance.RemoveObserver(ObserverKey.PopupShowed, SetStatusHideClickLevel);
        EnvironmentType environment = EnvironmentConfig.currentEnvironmentEnum;
        if (environment == EnvironmentType.dev)
            Observer.Instance.RemoveObserver(ObserverKey.EnableCheatMode, EnableCheatMode);
    }

    private void ClearItems()
    {
        StopAllCoroutines();
    }

    private void SetActive(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }

    private void SetEventBt(ButtonCustom bt, UnityAction action)
    {
        if (bt != null)
            bt.onClick = action;
    }

    private void SetText(TextMeshProUGUI text, string content)
    {
        if (text != null)
            text.text = content;
    }

    private void SetImage(Image img, string path)
    {
        if (img != null)
            img.sprite = TexturesManager.Instance.GetSprites(path); ;
    }
    private void SetStatusHideClickLevel(object data)
    {
        if (data == null) return;
        bool status = (bool)data;
        SetActive(hideClickLevelOb, status);
    }
    // Start is called before the first frame update
    void Start()
    {
        UserDatas.current_scene_type = SceneType.MainMenu;
        SetActive(hideClickLevelOb, false);
        TPRLSoundManager.Instance.StopMusic();
        TPRLSoundManager.Instance.PlayMusic("mainmenu");
        int length = 0;
        if (itemLevelMainMenus != null && itemLevelMainMenus.Length > 0)
        {
            length = itemLevelMainMenus.Length;
            for (int i = 0; i < length; i++)
            {
                if (itemLevelMainMenus[i] != null)
                    originItemLevelPosX[i] = itemLevelMainMenus[i].transform.localPosition.x;
            }
        }
        currentChapter = UserDatas.user_Data.current_progress.chapter;
        EnqueueOrder(itemLevelMainMenus[0], itemLevelMainMenus[1], itemLevelMainMenus[2]);
        Ultis.ShowFade(false, null);
        AdsManager.instance.ShowBanner(true);
        CreateLevels();
    }

    private void EnqueueOrder(ItemLevelMainMenu item0, ItemLevelMainMenu item1, ItemLevelMainMenu item2)
    {
        queueOrderItemLevels.Add(item0);
        queueOrderItemLevels.Add(item1);
        queueOrderItemLevels.Add(item2);
    }
    [SerializeField] private LightQuality lightQuality;
    private void CreateLevels()
    {
        ClearItems();
        SetText(txChapter, $"Chapter {currentChapter + 1}");
        RecordLevelMainMenu[] recordLevelMainMenus = DataController.Instance.levelVO.GetDatasByName<RecordLevelMainMenu>($"Chapter_{currentChapter}");
        GameObject worldMapPf = PrefabsManager.Instance.GetAsset<GameObject>($"Worldmap/Worldmap_{currentChapter}");
        if (worldMapPf != null)
        {
            float z = GetWorldMapZPos();
            Vector3 vtWorldMap = new Vector3(-58.8042f, 0.3720666f, z);
            worldmapMainMenu = CreateManager.instance.CreateObjectGetComponent<WorldmapMainMenu>(worldMapPf, vtWorldMap);
        }
        lightQuality.SetPostProcessor(worldmapMainMenu.GetComponent<PostProcessor>());
        int level = GetIndexLevel(UserDatas.user_Data.current_progress.level);
        int previous_level = GetIndexLevel(UserDatas.user_Data.current_progress.previous_level);
        SetPlayerTransform(previous_level);
        if (playerFakeManager != null && previous_level != level)
        {
            playerFakeManager.PlayerDisappearAnim();
            playerFakeManager.actionAppear = () => { SetPlayerTransform(level); };
        }
        else
        {
            playerFakeManager.PlayerAppearAnim();
        }
        RecordLevelMainMenu recordLevel = recordLevelMainMenus[level + 1];
        string title = $"Level {recordLevel.level}";
        SetText(txLevel, title);
        //MeshCombiner.Instance.CombineMesh(transform);
    }

    private void ClickNextChapter()
    {
        if (currentChapter >= maxChapter - 1 || isClickingChangeChapter) return;
        isClickingChangeChapter = true;
        currentChapter++;
        CreateLevels();
        ItemLevelMainMenu item0 = queueOrderItemLevels[0];
        Transform item0Tf = item0.transform;
        item0.SetLock(false);
        item0Tf.localPosition = new Vector3(originItemLevelPosX[2], item0Tf.localPosition.y, 0);

        ItemLevelMainMenu item1 = queueOrderItemLevels[1];
        LeanTween.moveLocalX(item1.gameObject, originItemLevelPosX[0], 0.4f).setEase(LeanTweenType.linear).setOnComplete(() => { item1.SetLock(false); });

        ItemLevelMainMenu item2 = queueOrderItemLevels[2];
        item2.SetLock(UserDatas.user_Data.current_progress.chapter < currentChapter);
        LeanTween.moveLocalX(queueOrderItemLevels[2].gameObject, originItemLevelPosX[1], 0.4f).setEase(LeanTweenType.linear).setOnComplete(() => { isClickingChangeChapter = false; });

        queueOrderItemLevels.RemoveAt(0);
        queueOrderItemLevels.Add(item0);
    }

    private void ClickBackChapter()
    {
        if (currentChapter <= 0 || isClickingChangeChapter) return;
        isClickingChangeChapter = true;
        currentChapter--;
        CreateLevels();
        ItemLevelMainMenu item2 = queueOrderItemLevels[2];
        Transform item2Tf = item2.transform;
        item2.SetLock(false);
        item2Tf.localPosition = new Vector3(originItemLevelPosX[0], item2Tf.localPosition.y, 0);

        ItemLevelMainMenu item1 = queueOrderItemLevels[1];
        LeanTween.moveLocalX(item1.gameObject, originItemLevelPosX[2], 0.4f).setEase(LeanTweenType.linear).setOnComplete(() => { item1.SetLock(false); });

        ItemLevelMainMenu item0 = queueOrderItemLevels[0];
        item0.SetLock(UserDatas.user_Data.current_progress.chapter < currentChapter);
        LeanTween.moveLocalX(item0.gameObject, originItemLevelPosX[1], 0.4f).setEase(LeanTweenType.linear).setOnComplete(() => { isClickingChangeChapter = false; });

        queueOrderItemLevels.RemoveAt(2);
        queueOrderItemLevels.Insert(0, item2);
    }

    private void ClickPlay()
    {
        LevelSelect.level = 0;
        LevelSelect.chapter = 0;
        if (UserDatas.user_Data.current_progress.chapter < currentChapter) return;
        //AdsManager.instance.ShowInter();
        UserDatas.user_Data.current_progress.previous_level = UserDatas.user_Data.current_progress.level;
        Ultis.ShowFade(true, LoadSceneGame);
    }

    private void LoadSceneGame()
    {
        ScenesManager.Instance.GetScene(AllSceneName.GamePlayMain, false);
        ScenesManager.Instance.GetScene(AllSceneName.GamePlayCanvas, true);
    }
    private void SetPlayerTransform(int level)
    {
        if (worldmapMainMenu == null || playerFakeManager == null) return;
        worldmapMainMenu.FlashPosition(level);
        playerFakeManager.transform.SetParent(worldmapMainMenu.levelTransforms[level]);
        playerFakeManager.transform.localPosition = new Vector3(0, 0.2f, 0);
        playerFakeManager.transform.localRotation = Quaternion.Euler(Vector3.zero);
        playerFakeManager.transform.localScale = Vector3.one;
        ResetRotation(level);
        playerFakeManager.actionRotation = () => ResetRotation(level);

    }
    private void ResetRotation(int level)
    {
        if (playerFakeManager.playerFake == null) return;
        playerFakeManager.transform.localEulerAngles = worldmapMainMenu.SetRotationPlayer(level);
    }
    private int GetIndexLevel(int level)
    {
        RecordLevelMainMenu[] recordLevelMainMenus = DataController.Instance.levelVO.GetDatasByName<RecordLevelMainMenu>($"Chapter_{currentChapter}");
        int length = recordLevelMainMenus.Length;
        if (level >= length)
        {
            level = 1;
        }
        if (level < 1)
            level = 1;
        return level - 1;
    }

    private float GetWorldMapZPos()
    {
        float z = 46;
        float screen_rate = Ultis.GetScreenRate();
        if (screen_rate >= 2.6f)
            z = 28;
        else if (screen_rate >= 2.0f)
            z = 37;
        else if (screen_rate >= 1.7f)
            z = 46;
        else if (screen_rate >= 1.6f)
            z = 54;
        else
            z = 82;
        return z;
    }

    private void EnableCheatMode(object data)
    {
        if (data == null) return;
        bool active = (bool)data;
        SetActive(btCheatMode, active);
    }

    private void ClickShowHideUI()
    {
        bool active = obTop.activeSelf;
        active = !active;
        SetActive(obTop, active);
        SetActive(obMiddle, active);
        SetActive(obBot, active);
    }
}
