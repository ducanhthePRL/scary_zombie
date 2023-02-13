using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplashManager : MonoBehaviour
{
    [SerializeField]
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (canvasGroup != null)
            canvasGroup.alpha = 0;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    // Start is called before the first frame update
    void Start()
    {
        UserDatas.current_scene_type = SceneType.Splash;
        if (canvasGroup != null)
        {
            canvasGroup.LeanAlpha(1,1f);
        }
        UserDatas.Load();
        IAPManager.instance.Init(delegate { AdsManager.instance.Init(LoadSceneLoading); });

    }

    private void LoadSceneLoading()
    {
        StartCoroutine(IELoadSceneLoading());
    }

    private IEnumerator IELoadSceneLoading()
    {
        yield return new WaitForSeconds(1);
        Ultis.ShowFade(true, delegate {
            ScenesManager.Instance.GetSceneAsync(AllSceneName.LoadingScene, false, null, true);
        });
    }
}
