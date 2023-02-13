using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private Material _material;
    protected Material material
    {
        get
        {
            if (_material == null) _material = skinnedMeshRenderer.materials[0];
                return _material;
        }
    }
    [SerializeField]protected SkinnedMeshRenderer skinnedMeshRenderer;
    protected virtual void Start()
    {
        OnFire = OnFireCallback;
    }
    public Action OnFire;
    public void ClearEffects()
    {

    }
    public void Fire()
    {
        StopAction();
        StopAllCoroutines();
        StartCoroutine(IEFire());
    }
    public void BeMagicAttacked()
    {
        StopAction();
        StopAllCoroutines(); 
        LeanTween.moveY(gameObject, transform.position.y + 0.5f, 1);
        StartCoroutine(IEBeMagicAttacked());
    }
    public virtual void StopAction()
    {

    }
    protected virtual IEnumerator IEBeMagicAttacked()
    {
        float valueSet = 0f;
        while (valueSet < 1)
        {
            valueSet += 0.05f;
            material.SetFloat("_SliceAmount", valueSet);
            yield return new WaitForSeconds(0.1f);
        };
        OnFire.Invoke();
    }
    protected virtual IEnumerator IEFire()
    {
        float valueSet = 0f;
        while (valueSet<1)
        {
            valueSet += 0.05f;
            material.SetFloat("_SliceAmount", valueSet);
            yield return new WaitForSeconds(0.1f);
        };
        OnFire.Invoke();
    }
    public virtual void OnFireCallback()
    {
        Observer.Instance.Notify(ObserverKey.Fire);
    }
    public virtual void Fall(Transform hole)
    {
        StopAction();
        Observer.Instance.Notify(ObserverKey.FallOut);
    }
    public virtual void Booooooooooooooooooooooooooooooooom(ObjectIndex objectIndex)
    {

    }
    public void FallIntoWater()
    {
        StopAction();
        StopAllCoroutines();
        StartCoroutine(IEFallIntoWater());
    }
    protected virtual IEnumerator IEFallIntoWater()
    {
        yield return null;
    }
}
