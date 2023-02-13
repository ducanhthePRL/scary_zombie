using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dest : MonoBehaviour
{
    public bool canFinish = false;
    [SerializeField] private List<ParticleSystem> canFinishEffect;
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
        boxCollider.enabled = false;
        Observer.Instance.AddObserver(ObserverKey.AllEnemyDead, ChangeState);
        SetIndex();
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.AllEnemyDead, ChangeState);
    }
    private void ChangeState(object data)
    {
        canFinish = true;
        boxCollider.enabled = true;
        foreach (ParticleSystem ps in canFinishEffect)
        {
            ps.Play();
        }
        TPRLSoundManager.Instance.PlaySoundFx("finish",0.5f);
    }
    private void SetIndex()
    {
        objectIndex.column = (int)transform.position.z;
        objectIndex.row = (int)transform.position.x;
    }
}
