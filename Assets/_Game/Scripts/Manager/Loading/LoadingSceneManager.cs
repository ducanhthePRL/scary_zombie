using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class LoadingSceneManager : MonoBehaviour
{
    [SerializeField]
    private GameObject obTapToStart;
    [SerializeField]
    private Button btTapToStart;
    [SerializeField]
    private VideoPlayer vdPlay;
    [SerializeField]
    private Image logo;

    private void Awake()
    {
        if (btTapToStart != null)
            btTapToStart.onClick.AddListener(TapToStart);
    }

    // Start is called before the first frame update
    void Start()
    {
        UserDatas.current_scene_type = SceneType.Loading;
        Ultis.ShowFade(false, ShowTapToStart);
        bool is_first_open = UserDatas.user_Data.current_progress.level < 1 ? true : false;
        if (logo != null)
            logo.gameObject.SetActive(!is_first_open);
        if (vdPlay != null)
        {
            vdPlay.gameObject.SetActive(is_first_open);
            if (is_first_open)
                vdPlay.Play();
        }
    }
    private void ShowTapToStart()
    {
        if (CheckUpdateVersion())
        {
            if (obTapToStart != null)
                obTapToStart.SetActive(true);
        }
    }

    private void TapToStart()
    {
        bool is_first_open = UserDatas.user_Data.current_progress.level < 1 ? true : false;
        if (is_first_open)
        {
            Ultis.ShowFade(true, LoadSceneGame);

        }
        else
        {
            Ultis.ShowFade(true, LoadScene);
        }
        FirebaseManager.instance.LogEvent("open_app");
    }

    private void LoadScene()
    {
        ScenesManager.Instance.GetSceneAsync(AllSceneName.MainMenu, false, null, true);
    }

    private void LoadSceneGame()
    {
        ScenesManager.Instance.GetScene(AllSceneName.GamePlayMain, false);
        ScenesManager.Instance.GetScene(AllSceneName.GamePlayCanvas, true);
    }
    private bool CheckUpdateVersion()
    {
        string last_update_version = EnvironmentConfig.version;
        if (string.IsNullOrEmpty(last_update_version)) return true;
        string[] stringLastUpdate = last_update_version.Split('.');
        int[] valueLastUpdate = Array.ConvertAll<string, int>(stringLastUpdate, int.Parse);
        string versionNow = Application.version;
        string[] stringVersionNow = versionNow.Split('.');
        int[] valueVersionNow = Array.ConvertAll<string, int>(stringVersionNow, int.Parse);
        int length = valueLastUpdate.Length;
        for (int i = 0; i < length; i++)
        {
            if (valueVersionNow[i] < valueLastUpdate[i])
            {
                PopUpNotice notice = PanelManager.Show<PopUpNotice>();
                notice.OnSetTextOneButton("NOTICE", "New version is available\nPlease download it from App store !", OpenAppStoreToUpdate, "GO TO APP STORE");
                return false;
            }
        }
        return true;
    }
    private void OpenAppStoreToUpdate()
    {
        Application.OpenURL(EnvironmentConfig.linkGameStore);
        Application.Quit();
    }
}
