using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : Trap
{
    public override void OnCollide(Collider other)
    {
        MoveComponent moveComponent = other.GetComponent<MoveComponent>();
        if (moveComponent != null)
        {
            other.enabled = false;
            moveComponent.transform.position = new Vector3(transform.position.x, moveComponent.transform.position.y, transform.position.z);
            moveComponent.FallIntoWater() ;
        }
        else
        {
            EntityManager entityManager = other.GetComponent<EntityManager>();
            if (entityManager != null)
            {
                other.enabled = false;
                entityManager.transform.position = new Vector3(transform.position.x, entityManager.transform.position.y, transform.position.z);
                entityManager.FallIntoWater();
            }
        }
    }
}
