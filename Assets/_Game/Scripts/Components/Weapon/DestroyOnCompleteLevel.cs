using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnCompleteLevel : MonoBehaviour
{
    void Start()
    {
        Observer.Instance.AddObserver(ObserverKey.PassLevel, DestroyThis);        
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PassLevel, DestroyThis);
    }
    void DestroyThis(object data)
    {
        if (transform.parent == null)
        {
            Destroy(gameObject);
        }
    }
}
