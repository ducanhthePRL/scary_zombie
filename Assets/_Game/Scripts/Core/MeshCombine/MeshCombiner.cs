using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MeshCombiner : TPRLSingleton<MeshCombiner>
{
    [SerializeField]
    private Material[] mats;
    private Dictionary<string, Material> lstMats = new Dictionary<string, Material>();

    protected override void Awake()
    {
        dontDestroyOnLoad = true;
        base.Awake();
    }

    private void Start()
    {
        AddMatToDic();
    }

    private void AddMatToDic()
    {
        if (mats == null || mats.Length == 0) return;
        int length = mats.Length;
        for (int i = 0; i < length; i++)
        {
            if (mats[i] == null) continue;
            Material mat = mats[i];
            if (!lstMats.ContainsKey(mat.name))
                lstMats.Add(mat.name, mat);
        }
    }

    public void Clear()
    {
        int childcount = transform.childCount;
        if (childcount == 0) return;
        for (int i = childcount - 1; i >= 0; i--)
        {
            MeshFilter meshFilter = transform.GetChild(i).gameObject.GetComponent<MeshFilter>();
            if (meshFilter != null)
                Destroy(meshFilter.mesh);
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }

    public void CombineMesh(Transform tf)
    {
        Clear();
        MeshFilter[] meshFilters = FindObjectsOfType<MeshFilter>();
        if (meshFilters != null && meshFilters.Length == 0) return;
        transform.SetParent(tf);
        transform.SetParent(null);
        transform.position = Vector3.zero;
        List<Material> materials = new List<Material>();
        int mesh_filter_length = meshFilters.Length;
        for (int i = 0; i < mesh_filter_length; i++)
        {
            MeshRenderer mesh_ren = meshFilters[i].GetComponent<MeshRenderer>();
            if (mesh_ren == null) continue;
            int layer = mesh_ren.gameObject.layer;
            if (layer != (int)LayerType.PostProcessing && layer != (int)LayerType.Level_worldmap) continue;
            Material[] materials1 = mesh_ren.sharedMaterials;
            if (materials1 == null) continue;
            int mat_length1 = materials1.Length;
            for (int j = 0; j < mat_length1; j++)
            {
                if (!materials.Contains(materials1[j]))
                {
                    materials.Add(materials1[j]);
                }
            }
        }

        int mat_length = materials.Count;
        for (int i = 0; i < mat_length; i++)
        {
            Material mat = materials[i];
            if (mat == null) continue;
            List<CombineInstance> combines = new List<CombineInstance>();
            int layer = 0;
            int vertex_count = 0;
            for (int j = 0; j < mesh_filter_length; j++)
            {
                MeshRenderer meshRenderer = meshFilters[j].GetComponent<MeshRenderer>();
                if (meshRenderer == null) continue;
                layer = meshRenderer.gameObject.layer;
                if (layer != (int)LayerType.PostProcessing && layer != (int)LayerType.Level_worldmap) continue;
                Material[] materials1 = meshRenderer.sharedMaterials;
                if (materials1 == null) continue;
                Transform mesh_tf = meshRenderer.transform;
                CreateMesh(mesh_tf, mesh_tf.rotation, mesh_tf.localScale, meshRenderer.GetComponents<MeshCollider>(), meshRenderer.GetComponents<BoxCollider>());
                int mat_length1 = materials1.Length;
                for (int k = 0; k < mat_length1; k++)
                {
                    if (mat != materials1[k]) continue;
                    vertex_count += meshFilters[j].sharedMesh.vertexCount;
                    if (vertex_count >= 65000)
                    {
                        CreateNewOb(combines.ToArray(), mat.name, layer);
                        combines = new List<CombineInstance>();
                        vertex_count = meshFilters[j].sharedMesh.vertexCount;
                    }
                    CombineInstance combineInstance = new CombineInstance();
                    combineInstance.mesh = meshFilters[j].sharedMesh;
                    combineInstance.subMeshIndex = k;
                    combineInstance.transform = meshFilters[j].transform.localToWorldMatrix;
                    combines.Add(combineInstance);
                }
                //Destroy(meshFilters[j].gameObject);
                meshFilters[j].gameObject.SetActive(false);
            }
            CreateNewOb(combines.ToArray(), mat.name, layer);
        }
        transform.SetParent(TPRLSoundManager.Instance.transform);
        transform.SetParent(null);
    }

    private void CreateNewOb(CombineInstance[] combines, string mat_name, int layer)
    {
        Mesh mesh = new Mesh();
        //mesh.indexFormat = IndexFormat.UInt32;
        mesh.CombineMeshes(combines, true);
        GameObject newOb = new GameObject();
        newOb.layer = layer;
        newOb.name = $"Mesh_{mat_name}";
        newOb.transform.SetParent(transform);
        newOb.transform.localPosition = Vector3.zero;
        MeshFilter mesh_filter = newOb.AddComponent<MeshFilter>();
        mesh_filter.mesh = mesh;
        MeshRenderer mesh_renderer = newOb.AddComponent<MeshRenderer>();
        mesh_renderer.material = GetMaterial(mat_name);
        //MeshCollider mesh_collider = newOb.AddComponent<MeshCollider>();
        //mesh_collider.convex = true;
        //Mesh m = null;
        //if (mesh_colliders.ContainsKey(mat_name))
        //    m = mesh_colliders[mat_name];
        //mesh_collider.sharedMesh = m;
        newOb.SetActive(true);
    }

    private void CreateMesh(Transform tf, Quaternion rotation, Vector3 scale, MeshCollider[] mesh, BoxCollider[] box)
    {
        if ((mesh == null && box == null) || (mesh.Length == 0 && box.Length == 0)) return;
        GameObject newOb = new GameObject();
        newOb.name = $"Collider_{tf.name}";
        newOb.transform.SetParent(transform);
        newOb.transform.position = tf.position;
        newOb.transform.rotation = rotation;
        newOb.transform.localScale = scale;
        int length = mesh.Length;
        for (int i = 0; i < length; i++)
        {
            MeshCollider mesh_collider = newOb.AddComponent<MeshCollider>();
            mesh_collider.convex = mesh[i].convex;
            mesh_collider.sharedMesh = mesh[i].sharedMesh;
        }

        length = box.Length;
        for (int i = 0; i < length; i++)
        {
            BoxCollider box_collider = newOb.AddComponent<BoxCollider>();
            box_collider.isTrigger = box[i].isTrigger;
            box_collider.center = box[i].center;
            box_collider.size = box[i].size;
        }
    }

    private Material GetMaterial(string mat_name)
    {
        return lstMats.ContainsKey(mat_name) ? lstMats[mat_name] : null;
    }
}
