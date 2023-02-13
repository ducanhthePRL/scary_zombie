using CodeStage.AntiCheat.Storage;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class LightQuality : MonoBehaviour
{
     [SerializeField] private PostProcessVolume postProcessVolume;
     [SerializeField] private PostProcessor postProcesssor;

    private void Start()
    {
        bool theme_open = ObscuredPrefs.Get<bool>("ThemeOpen",false);
        if (postProcesssor != null)
        {
            Observer.Instance.AddObserver(ObserverKey.ThemeClick, ThemeClick);
            ThemeClick(theme_open);
        }
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.ThemeClick, ThemeClick);
    }
    private void ThemeClick(object data)
    {
        bool isActive = (bool)data;
        if (postProcesssor != null)
        {
            postProcesssor.enabled = isActive;
        }
        ObscuredPrefs.Set<bool>("ThemeOpen", isActive);
    }
    public void SetPostProcessor(PostProcessor post_rocessor)
    {
        postProcesssor = post_rocessor;
        Observer.Instance.AddObserver(ObserverKey.ThemeClick, ThemeClick);
    }
}
