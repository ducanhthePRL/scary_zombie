using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTrail : MonoBehaviour
{
    public float meshRefreshRate = 0.1f;
    public float meshDestroyDelay = 0.075f;
    private SkinnedMeshRenderer _skinnedMeshRender;
    private SkinnedMeshRenderer skinnedMeshRenderer
    {
        get
        {
            if (_skinnedMeshRender == null) _skinnedMeshRender = GetComponentInChildren<SkinnedMeshRenderer>();
            return _skinnedMeshRender;
        }
    }
    public Material material;
    private void Start()
    {
        mesh = new Mesh();
    }
    GameObject[] gO = new GameObject[2];
    MeshRenderer[] mr = new MeshRenderer[2];
    MeshFilter[] mf = new MeshFilter[2];
    private int gOIndex=0;
    Mesh mesh;
    public void ActiveMeshTrail()
    {
        //InvokeRepeating("ActiveTrail", 0, 1f);
    }
    private void ActiveTrail()
    {
        gOIndex++;
        if (gOIndex >= 2) gOIndex = 0;
        if (gO[gOIndex] == null)
        {
            gO[gOIndex] = new GameObject();
            mr[gOIndex] = gO[gOIndex].AddComponent<MeshRenderer>();
            mf[gOIndex] = gO[gOIndex].AddComponent<MeshFilter>();
        }
        gO[gOIndex].transform.localScale = new Vector3(0.74f, 0.99f, 0.74f);
        gO[gOIndex].SetActive(true);
        gO[gOIndex].transform.SetPositionAndRotation(transform.position, transform.rotation);
        mesh = new Mesh();
        skinnedMeshRenderer.BakeMesh(mesh);
        mf[gOIndex].mesh = mesh;
        mr[gOIndex].material = material;
        StartCoroutine(DisableGO(gO[gOIndex], meshDestroyDelay));
    }
    public void DeactiveTrail()
    {
        /*CancelInvoke();
        for(int i = 0; i < gO.Length; i++)
        {
            gO[i].SetActive(false);
        }*/
    }
    private void OnDestroy()
    {
        foreach(GameObject gameObject in gO)
        {
            Destroy(gameObject);
        }
    }
    private IEnumerator DisableGO(GameObject go,float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        go.SetActive(false);
    }
}
