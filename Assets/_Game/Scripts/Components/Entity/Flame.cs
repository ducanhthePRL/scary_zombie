using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flame : Trap
{
    [SerializeField] private ParticleSystem flame;
    private void Start()
    {
        TPRLSoundManager.Instance.StopAllSoundFx(null);
        TPRLSoundManager.Instance.PlaySoundFx("flame",true);
        flame.Play();
    }
    public override void OnCollide(Collider other)
    {
        base.OnCollide(other);
        MoveComponent moveComponent = other.GetComponent<MoveComponent>();
        if (moveComponent != null)
        {
            other.enabled = false;
            moveComponent.transform.position = new Vector3(transform.position.x, moveComponent.transform.position.y, transform.position.z);
            moveComponent.Fire();
        }
        else
        {
            EntityManager entityManager = other.GetComponent<EntityManager>();
            if (entityManager != null)
            {
                other.enabled = false;
                entityManager.transform.position = new Vector3(transform.position.x, entityManager.transform.position.y, transform.position.z);
                entityManager.Fire();
            }
        }
    }
}
