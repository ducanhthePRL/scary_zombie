using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : Trap
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
    private ObjectIndex _objectIndex;
    private ObjectIndex objectIndex
    {
        get
        {
            if (_objectIndex == null) _objectIndex = GetComponent<ObjectIndex>();
            return _objectIndex;
        }
    }
    private void Start()
    {
        objectIndex.row = (int)transform.position.x;
        objectIndex.column = (int)transform.position.z;
    }
    public override void OnCollide(Collider other)
    {
        other.enabled = false;
        MoveComponent moveComponent = other.GetComponent<MoveComponent>();
        if (moveComponent != null)
        {
            Vector2 index = new Vector2(objectIndex.row, objectIndex.column);
            moveComponent.VerticalFall(index);
        }
        else
        {
            EntityManager entityManager = other.GetComponent<EntityManager>();
            if (entityManager != null)
            {
                entityManager.Fall(transform);
            }
        }
    }
}
