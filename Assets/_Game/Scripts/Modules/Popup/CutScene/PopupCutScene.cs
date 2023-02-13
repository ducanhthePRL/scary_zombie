using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

[UIPanelPrefabAttr("Popup/CutScene/PopupCutScene", "PriorityCanvas")]
public class PopupCutScene : BasePanel
{
    [SerializeField] private VideoPlayer videoPlayer;
    public void PlayCutScene()
    {
        videoPlayer.Play();
        StartCoroutine(checkOnDone());
    }
    public Action OnDone;
    private IEnumerator checkOnDone()
    {
        yield return new WaitUntil(() => { return videoPlayer.isPlaying; });
        while (videoPlayer.isPlaying)
        {
            yield return null;
        }
        OnDone?.Invoke();
        HidePanel();
    }
}
