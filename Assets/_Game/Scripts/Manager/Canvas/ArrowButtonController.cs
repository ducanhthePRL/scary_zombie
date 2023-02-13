using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ArrowButtonController : MonoBehaviour
{
   public UnityEvent _event ;
   public void OnButtonArrowButtonClick(string key)
   {
        Observer.Instance.Notify(ObserverKey.ArrowButtonClick, key);
   }
}

