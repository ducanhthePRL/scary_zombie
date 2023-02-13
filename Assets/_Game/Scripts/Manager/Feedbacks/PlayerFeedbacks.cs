using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerFeedbacks : MonoBehaviour
{
    public UnityEvent cameraShakeAction;
    private void Start()
    {
        Observer.Instance.AddObserver(ObserverKey.PlayerStopMove, PlayFeedBack);
        Observer.Instance.AddObserver(ObserverKey.CollideEnemy, PlayFeedBackEnemyDead);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PlayerStopMove, PlayFeedBack);
        Observer.Instance.RemoveObserver(ObserverKey.CollideEnemy, PlayFeedBackEnemyDead);
    }
    private void PlayFeedBackEnemyDead(object data)
    {
        cameraShakeAction?.Invoke();
    }
    private void PlayFeedBack(object data)
    {
        if (data==null)
        {
            cameraShakeAction?.Invoke();
        }
    }
}
