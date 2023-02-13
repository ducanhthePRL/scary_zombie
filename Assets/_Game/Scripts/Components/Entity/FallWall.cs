using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallWall : MonoBehaviour
{
    private BoxCollider _boxCollider;
    private BoxCollider boxCollider
    {
        get
        {
            if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();
            return _boxCollider;
        }
    }
    [SerializeField]private Animator animator;
    private void OnTriggerEnter(Collider other)
    {
        MoveComponent moveComponent = other.GetComponent<MoveComponent>();
        if (moveComponent != null)
        {
            moveComponent.StopAction();
            if (moveComponent.moveDirectionV2.x == 1 || moveComponent.moveDirectionV2.y == 1)
            {
                animator.Play("FallAhead");
            }
            else if (moveComponent.moveDirectionV2.x == -1 || moveComponent.moveDirectionV2.y == -1)
            {
                animator.Play("FallBackward");
            }
            ResetBlock(moveComponent);
            boxCollider.isTrigger = false;
        }
    }
    private void ResetBlock(MoveComponent moveComponent)
    {
        StartCoroutine(IEResetBlock(moveComponent));
    }
    private IEnumerator IEResetBlock(MoveComponent moveComponent)
    {
        yield return new WaitForSeconds(1);
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
        for (int i = 0; i < colliders.Length; i++)
        {
            Block block = colliders[i].GetComponent<Block>();
            if (block != null)
            {
                block.isLock = true;
            }
        }
        moveComponent.moveable = true;
    }
}
