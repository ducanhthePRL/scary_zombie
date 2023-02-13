using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Trap
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
    private MeshRenderer _meshRenderer;
    private MeshRenderer meshRenderer
    {
        get
        {
            if (_meshRenderer == null) _meshRenderer = GetComponent<MeshRenderer>();
            return _meshRenderer;
        }
    }
    [SerializeField] private ParticleSystem boomEffect;
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
            StartCoroutine(boom());
            moveComponent.Booooooooooooooooooooooooooooooooom(objectIndex);
        }
        else
        {
            EntityManager entityManager = other.GetComponent<EntityManager>();
            if (entityManager != null)
            {
                StartCoroutine(boom());
                entityManager.Booooooooooooooooooooooooooooooooom(objectIndex);
            }
        }
    }
    private IEnumerator boom()
    {
        boomEffect.Play();
        meshRenderer.enabled = false;
        yield return new WaitForSeconds(0.5f);
        ResetBlock();
        Destroy(gameObject);
    }
    private void ResetBlock()
    {
        Collider[] colliders = Physics.OverlapSphere(new Vector3(objectIndex.row,0,objectIndex.column), 0.2f);
        for (int i = 0; i < colliders.Length; i++)
        {
            Block block = colliders[i].GetComponent<Block>();
            if (block != null)
            {
                block.isTrap = false;
            }
        }
    }
}
