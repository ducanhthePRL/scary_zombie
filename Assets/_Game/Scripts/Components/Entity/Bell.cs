using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Bell : MonoBehaviour
{
    public BellTarget bellTarget;

    public UnityAction<Bell> onTrigger;

    private void OnTriggerEnter(Collider other)
    {
        MoveComponent moveComponent = other.GetComponent<MoveComponent>();
        if (moveComponent != null)
        {
            moveComponent.StopAction();
            onTrigger.Invoke(this);
            moveComponent.moveable = true;
        }
    }
}
