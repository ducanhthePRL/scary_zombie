using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalAnimationEvent : MonoBehaviour
{
    private void Start()
    {
        Observer.Instance.AddObserver(ObserverKey.PassLevel, PassLevel);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PassLevel, PassLevel);
    }
    private bool isPassLevel = false;
    private void PassLevel(object data)
    {
        isPassLevel = true;   
    }
    private void PlaySoundOnObject(string sound_name)
    {
        if (isPassLevel) return;
        int randomValue = Random.Range(0, 2);
        if (randomValue == 0)
        {
            TPRLSoundManager.Instance.PlaySoundFx(sound_name,0.5f);
        }
    }
}
