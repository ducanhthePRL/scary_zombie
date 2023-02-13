using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : MonoBehaviour
{
    [SerializeField] private ParticleSystem bloom;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform target;
    private void Start()
    {
        Observer.Instance.AddObserver(ObserverKey.PlayerBeAttacked, AttackPlayer);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PlayerBeAttacked, AttackPlayer);
    }
    private void AttackPlayer(object data)
    {
        Vector3 targetPosition = (Vector3)data;
        animator.Play("Magic Attack");
        bloom.Play();
    }
    public void ResetBlock(Vector3 enemy_position)
    {
        if (!target.gameObject.activeSelf) return;
        Collider[] colliders = Physics.OverlapSphere(target.position, 0.2f);
        for(int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].name == target.name) continue;
            Block block = colliders[i].GetComponent<Block>();
            if (block != null)
            {
                int distanceX = Mathf.RoundToInt(enemy_position.x - block.transform.position.x);
                int distanceZ = Mathf.RoundToInt(enemy_position.z - block.transform.position.z);
                if (distanceX == 1)
                {
                    foreach(Direction dir in block.trapLock)
                    {
                        if (dir == Direction.RIGHT)
                        {
                            block.trapLock.Remove(dir);
                            break;
                        }
                    }
                }
                else if (distanceX == -1) {
                    foreach (Direction dir in block.trapLock)
                    {
                        if (dir == Direction.LEFT)
                        {
                            block.trapLock.Remove(dir);
                            break;
                        }
                    }
                }
                else if (distanceZ == 1)
                {
                    foreach (Direction dir in block.trapLock)
                    {
                        if (dir == Direction.UP)
                        {
                            block.trapLock.Remove(dir);
                            break;
                        }
                    }
                }
                else if(distanceZ == -1)
                {
                    foreach (Direction dir in block.trapLock)
                    {
                        if (dir == Direction.DOWN)
                        {
                            block.trapLock.Remove(dir);
                            break;
                        }
                    }
                }
                if (block.trapLock.Count <= 0)
                {
                    block.isTrap = false;
                }
            }
        }
    }
}
