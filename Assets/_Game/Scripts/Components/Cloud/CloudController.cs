using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudController : MonoBehaviour
{
    [SerializeField] private List<GameObject> clouds;

    private Dictionary<int, List<GameObject>> cloudPool = new Dictionary<int, List<GameObject>>();

    private float SpawnX = -2f;
    private float SpawnY = 6f;
    private float minSpawnZ = -7f;
    private float maxSpawnZ = 2f;
    private float maxX = 15f;
    private int maxCloudSpawnOnce = 7;
    private int currentActiveCloud = 0;

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private float moveTime = 20;
    private void Start()
    {
        Spawn();
    }
    private void Spawn()
    {
        StartCoroutine(IESpawn());
    }
    private IEnumerator IESpawn()
    {
        int length = clouds.Count;
        while (true)
        {
            if (currentActiveCloud < maxCloudSpawnOnce)
            {
                int randomCloud = Random.Range(0, length);
                List<GameObject> randomCloudList;
                if (!cloudPool.ContainsKey(randomCloud))
                {
                    randomCloudList = new List<GameObject>();
                    SpawnNewCloud(randomCloud, randomCloudList);
                    cloudPool.Add(randomCloud, randomCloudList);
                }
                else
                {
                    randomCloudList = cloudPool[randomCloud];
                    int count = randomCloudList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (!randomCloudList[i].activeSelf)
                        {
                            ActiveSpawnedCloud(i, randomCloudList);
                            break;
                        }
                        if(i== randomCloudList.Count - 1)
                        {
                            SpawnNewCloud(randomCloud, randomCloudList);
                        }
                    }
                }
            }
            float randomTime = Random.Range(0, 10f);
            yield return new WaitForSeconds(randomTime);
        }
    }
    private void SpawnNewCloud(int randomCloud, List<GameObject> randomCloudList)
    {
        GameObject cloud = Instantiate(clouds[randomCloud], new Vector3(SpawnX, SpawnY, Random.Range(minSpawnZ, maxSpawnZ)), Quaternion.Euler(Vector3.zero), transform);
        if (cloud == null) return;
        cloud.transform.localScale = Random.Range(0.5f, 1) * Vector3.one;
        currentActiveCloud++;
        LeanTween.moveX(cloud, maxX, moveTime).setOnComplete(() => {
            cloud.SetActive(false);
            currentActiveCloud--;
        });
        randomCloudList.Add(cloud);
    }
    private void ActiveSpawnedCloud(int i, List<GameObject> randomCloudList)
    {
        if (i >= randomCloudList.Count) return;
        GameObject cloud = randomCloudList[i];
        cloud.SetActive(true);
        cloud.transform.position = new Vector3(SpawnX, SpawnY, Random.Range(minSpawnZ, maxSpawnZ));
        cloud.transform.localScale = Random.Range(0.5f, 1) * Vector3.one;
        currentActiveCloud++;
        int i_copy = i;
        LeanTween.moveX(randomCloudList[i_copy], maxX, moveTime).setOnComplete(() => {
            randomCloudList[i_copy].SetActive(false);
            currentActiveCloud--; 
        });
    }
}
