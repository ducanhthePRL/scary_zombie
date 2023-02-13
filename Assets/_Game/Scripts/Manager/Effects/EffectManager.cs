using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public List<ParticleSystem> Effects = new List<ParticleSystem>();
    private void Start()
    {
        Observer.Instance.AddObserver(ObserverKey.PlayerMove, PlayDashEffect);
        Observer.Instance.AddObserver(ObserverKey.Bleeding, PlayBloodEffect);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PlayerMove, PlayDashEffect);
        Observer.Instance.RemoveObserver(ObserverKey.Bleeding, PlayBloodEffect);
    }
    private void PlayDashEffect(object data)
    {
        DataEffect dataMove = (DataEffect)data;
        Effects[0].transform.position = dataMove.startPosition + 0.75f*Vector3.up;
        if (dataMove.direction.x==-1)
        {
            Effects[0].transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
        }
        else if (dataMove.direction.x == 1)
        {
            Effects[0].transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
        }
        else if (dataMove.direction.z == 1)
        {
            Effects[0].transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if (dataMove.direction.z == -1)
        {
            Effects[0].transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
        }
        Effects[0].Play();
    }
    private void PlayBloodEffect(object data)
    {
        if (data == null) return;
        DataEffect dataMove = (DataEffect)data;
        Effects[1].transform.position = dataMove.startPosition;
        Effects[1].Play();
    }
}
public class DataEffect
{
    public Vector3 startPosition;
    public Vector3 direction;
    public ObjectIndex enemy;
}
