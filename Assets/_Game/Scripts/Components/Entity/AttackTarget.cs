using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTarget : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            moveComponent = other.gameObject.GetComponent<MoveComponent>();
            if (moveComponent.isMoving) return;
            other.enabled = false;
            moveComponent.BeMagicAttacked();
            Observer.Instance.Notify(ObserverKey.PlayerBeAttacked, transform.localPosition);
        }
    }
    MoveComponent moveComponent;
    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (moveComponent.isMoving) return;
            other.enabled = false;
            moveComponent.BeMagicAttacked();
            Observer.Instance.Notify(ObserverKey.PlayerBeAttacked, transform.localPosition);
        }
    }
}
