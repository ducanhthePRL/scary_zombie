using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.IO;

#if UNITY_EDITOR
public class MapGenerator : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject BlackCube;
    public GameObject WhiteCube;
    public GameObject Spawn;
    public GameObject Dest;
    public List<GameObject> Enemy;
    public List<GameObject> Fence;
    public List<GameObject> Obstacle;
    public GameObject Flame;
    public GameObject Hole;
    public List<GameObject> Effects;

    [Space]
    [Header("Editable Properties")]
    public int MapSizeWidth;
    public int MapSizeHeight;
    public int numberEnemy;

    [Space]
    [Header("Number Level Of Each Chapter")]
    public List<int> numberLevelEachChapter;
    public void RandomGenerateLevel()
    {
        GameObject level = new GameObject("Random Level");
        Level levelScript = level.AddComponent<Level>();
        levelScript.MapSizeWidth = MapSizeWidth;
        levelScript.MapSizeHeight = MapSizeHeight;
        GameObject[,] baseMapList = new GameObject[MapSizeWidth, MapSizeHeight];
        GenerateBaseMap(level, baseMapList);
        RandomGenerateObstacles(level,baseMapList);
    }
    public void GenerateBaseMapAndEnemy()
    {
        GameObject level = new GameObject("Random Level");
        Level levelScript = level.AddComponent<Level>();
        levelScript.MapSizeWidth = MapSizeWidth;
        levelScript.MapSizeHeight = MapSizeHeight;
        GameObject[,] baseMapList = new GameObject[MapSizeWidth, MapSizeHeight];
        GenerateBaseMap(level, baseMapList);

        //Generate ListPosition
        List<Vector3> rdListObstaclesPosition = new List<Vector3>();
        float scale = 0.1f * Random.Range(1, 10000);
        Vector2 offSet = new Vector2(Random.Range(1, 10000), Random.Range(1, 10000));

        float[,] noiseMap = new float[MapSizeWidth, MapSizeHeight];
        for (int x = 0; x < MapSizeWidth; x++)
        {
            for (int y = 0; y < MapSizeHeight; y++)
            {
                float xCor = (float)x * scale + offSet.x;
                float yCor = (float)y * scale + offSet.y;
                noiseMap[x, y] = Mathf.PerlinNoise(xCor, yCor);
            }
        }
        RandomGenerateEnemy(level, noiseMap);
        GameObject obstacles = new GameObject("Obstacles");
        obstacles.transform.parent = level.transform;
        GameObject fences = new GameObject("Fences");
        fences.transform.parent = level.transform;
        GameObject flames = new GameObject("Flames");
        flames.transform.parent = level.transform;
        GameObject holes = new GameObject("Holes");
        holes.transform.parent = level.transform;
        GameObject environment = new GameObject("Environment");
        environment.transform.parent = level.transform;
        RandomGenerateSpawnPoint(level, noiseMap);
        AddEffect(level);
    }
    private void GenerateBaseMap(GameObject level, GameObject[,] baseMapList)
    {
        level.transform.position = Vector3.zero;
        GameObject baseMap = new GameObject("BaseMap");
        Level levelScript = level.GetComponent<Level>();
        baseMap.transform.parent = level.transform;
        levelScript.tf = new Block[MapSizeHeight * MapSizeWidth];
        for (int i = 0; i < MapSizeWidth; i++)
        {
            GameObject line = new GameObject("Line "+(i+1).ToString());
            line.transform.parent = baseMap.transform;
            for(int j = 0; j < MapSizeHeight; j++)
            {
                if ((i % 2==0 &&j % 2 == 0) || (i%2 == 1 && j%2 == 1))
                {
                    GameObject go =(GameObject)PrefabUtility.InstantiatePrefab(WhiteCube);
                    go.transform.parent = line.transform;
                    go.transform.position = new Vector3(i, 0, j); 
                    baseMapList[i, j] = go;
                    levelScript.tf[MapSizeHeight*i+j]=go.GetComponent<Block>();
                }
                else
                {
                    GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(BlackCube);
                    go.transform.parent = line.transform;
                    go.transform.position = new Vector3(i, 0, j);
                    baseMapList[i, j] = go;
                    levelScript.tf[MapSizeHeight * i + j] = go.GetComponent<Block>();
                }
            }
        }
    }
    private void RandomGenerateObstacles(GameObject level, GameObject[,] baseMapList)
    {
        GameObject obstacles = new GameObject("Obstacles");
        obstacles.transform.parent = level.transform;

        //Generate ListPosition
        List<Vector3> rdListObstaclesPosition = new List<Vector3>();
        float scale = 0.1f *Random.Range(1,10000);
        Vector2 offSet = new Vector2(Random.Range(1,10000),Random.Range(1,10000));

        float[,] noiseMap = new float[MapSizeWidth, MapSizeHeight];
        for (int x = 0; x < MapSizeWidth; x++)
        {
            for(int y = 0; y < MapSizeHeight; y++)
            {
                float xCor = (float)x * scale + offSet.x;
                float yCor = (float)y * scale + offSet.y;
                noiseMap[x, y] = Mathf.PerlinNoise(xCor, yCor);
                if (noiseMap[x,y] > 0.65f) rdListObstaclesPosition.Add(new Vector3(x,0,y));
            }
        }
        foreach (Vector3 position in rdListObstaclesPosition)
        {
            int randomInt = Random.Range(0, Obstacle.Count);
            if (randomInt == 0)
            {
                GameObject obstacle = (GameObject)PrefabUtility.InstantiatePrefab(Obstacle[randomInt]);
                obstacle.transform.parent = obstacles.transform;
                obstacle.transform.position = new Vector3(position.x, 0.75f, position.z);
            }
            else
            {
                GameObject obstacle = (GameObject)PrefabUtility.InstantiatePrefab(Obstacle[randomInt]);
                obstacle.transform.parent = obstacles.transform;
                obstacle.transform.position = new Vector3(position.x, 0.25f, position.z);
            }
        }
        RandomGenerateFence(level, noiseMap);
        RandomGenerateFlame(level, noiseMap);
        RandomGenerateHole(level, noiseMap, baseMapList);
        RandomGenerateSpawnPoint(level, noiseMap);
        RandomGenerateEnemy(level, noiseMap);
        AddEffect(level);
    }
    private void AddEffect(GameObject level)
    {
        GameObject effects = new GameObject("Effects");
        effects.transform.parent = level.transform;
        EffectManager effectManager = effects.AddComponent<EffectManager>();
        GameObject dash = (GameObject)PrefabUtility.InstantiatePrefab(Effects[0]);
        dash.transform.parent = effects.transform;
        dash.transform.position = Vector3.zero;
        GameObject blood = (GameObject)PrefabUtility.InstantiatePrefab(Effects[1]);
        blood.transform.parent = effects.transform;
        blood.transform.position = Vector3.zero;
        effectManager.Effects.Add(dash.GetComponent<ParticleSystem>());
        effectManager.Effects.Add(blood.GetComponent<ParticleSystem>());
    }
    private void RandomGenerateFlame(GameObject level, float[,] noiseMap)
    {
        GameObject flames = new GameObject("Flames");
        flames.transform.parent = level.transform;
        for (int x = 0; x < MapSizeWidth; x++)
        {
            for (int y = 0; y < MapSizeHeight; y++)
            {
                if (noiseMap[x, y] > 0f && noiseMap[x, y] <= 0.1f)
                {
                    GameObject flame = (GameObject)PrefabUtility.InstantiatePrefab(Flame);
                    flame.transform.parent = flames.transform;
                    flame.transform.position = new Vector3(x, 0.75f, y);
                }
            }
        }
    }
    private void RandomGenerateHole(GameObject level, float[,] noiseMap, GameObject[,] baseMapList)
    {
        GameObject holes = new GameObject("Holes");
        holes.transform.parent = level.transform;
        for (int x = 0; x < MapSizeWidth; x++)
        {
            for (int y = 0; y < MapSizeHeight; y++)
            {
                if (noiseMap[x, y] > 0.1f && noiseMap[x, y] <= 0.2f)
                {
                    GameObject hole = (GameObject)PrefabUtility.InstantiatePrefab(Hole);
                    hole.transform.parent = holes.transform;
                    hole.transform.position = new Vector3(x, 0.5f, y);
                    DestroyImmediate(baseMapList[x, y]);
                }
            }
        }
    }

    private void RandomGenerateFence(GameObject level, float[,] noiseMap)
    {
        GameObject fences = new GameObject("Fences");
        fences.transform.parent = level.transform;
        for (int x = 0; x < MapSizeWidth; x++)
        {
            for (int y = 0; y < MapSizeHeight; y++)
            {
                if(noiseMap[x,y]>0.45f && noiseMap[x, y] <= 0.5f)
                {
                    GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(Fence[0]);
                    fence.transform.parent = fences.transform;
                    fence.transform.position = new Vector3(x, 0.4f, y - 0.5f);
                }
                else if(noiseMap[x,y]<= 0.45f && noiseMap[x, y] > 0.4f)
                {
                    GameObject fence = (GameObject)PrefabUtility.InstantiatePrefab(Fence[1]);
                    fence.transform.parent = fences.transform;
                    fence.transform.position = new Vector3(x - 0.5f, 0.4f, y);
                }
            }
        }
    }
    private void RandomGenerateSpawnPoint(GameObject level,float[,] noiseMap)
    {
        GameObject spawnAndDest = new GameObject("Spawn & Dest");
        spawnAndDest.transform.parent = level.transform;
        Level levelScript = level.GetComponent<Level>();
        int x, y;
        do
        {
            x = Random.Range(0, MapSizeWidth);
            y = Random.Range(0, MapSizeHeight);
        }
        while (noiseMap[x, y] > 0.65f);
        GameObject spawnPoint = (GameObject)PrefabUtility.InstantiatePrefab(Spawn);
        spawnPoint.transform.parent = spawnAndDest.transform;
        spawnPoint.GetComponent<PlayerSpawner>().SetMapSize(MapSizeWidth, MapSizeHeight);
        spawnPoint.transform.position = new Vector3(x, 0, y);
        do
        {
            x = Random.Range(0, MapSizeWidth);
            y = Random.Range(0, MapSizeHeight);
        }
        while (noiseMap[x, y] > 0.65f);
        GameObject destPoint = (GameObject)PrefabUtility.InstantiatePrefab(Dest);
        levelScript.dest = destPoint.GetComponent<ObjectIndex>();
        destPoint.transform.parent = spawnAndDest.transform;
        destPoint.transform.position = new Vector3(x, 0.255f, y);
    }
    private void RandomGenerateEnemy(GameObject level, float[,] noiseMap)
    {
        int numberEnemyRef = numberEnemy;
        Level levelScript = level.GetComponent<Level>();
        levelScript.numberEnemy = numberEnemyRef;
        GameObject enemies = new GameObject("Enemies");
        enemies.transform.parent = level.transform;

        while (numberEnemyRef > 0)
        {
            int x, y;
            do
            {
                x = Random.Range(0, MapSizeWidth);
                y = Random.Range(0, MapSizeHeight);
            }
            while (noiseMap[x, y] > 0.65f);
            var rdenemy = Random.Range(0, Enemy.Count);
            GameObject enemy = (GameObject)PrefabUtility.InstantiatePrefab(Enemy[rdenemy]);
            enemy.transform.parent = enemies.transform;
            enemy.transform.position = new Vector3(x, 0.75f, y);
            levelScript.listEnemyObject.Add(enemy.GetComponent<ObjectIndex>());
            numberEnemyRef--;
        }
    }
    public void DestroyGeneratedLevel()
    {
        GameObject generatedLevel = GameObject.Find("Random Level");
        DestroyImmediate(generatedLevel);
        GameObject manualGeneratedLevel = GameObject.Find("Level Generated");
        DestroyImmediate(manualGeneratedLevel);
    }

    public void AddGeneratedLevelToPrefab()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        bool canAddLevelToChapter;
        if (gameManager.recordLevels.Count == 0)
        {
            canAddLevelToChapter = false;
        }
        else
        {
            canAddLevelToChapter = gameManager.recordLevels[gameManager.recordLevels.Count - 1].level.Count < numberLevelEachChapter[gameManager.recordLevels.Count - 1];
        }
        if (!canAddLevelToChapter)
        {
            if (numberLevelEachChapter.Count == gameManager.recordLevels.Count)
            {
                DestroyGeneratedLevel();
                Debug.LogError("Please Increase numberLevelEachChapter in MapGenerator index and fill number to new field");
                return;
            }
        }
        GameObject generatedLevel = GameObject.Find("Random Level");
        string localPath;
        if (canAddLevelToChapter) {
            localPath = "Assets/_Game/Prefabs/Level/Chapter "+(gameManager.recordLevels.Count - 1 )+ "/Level " + (gameManager.recordLevels[gameManager.recordLevels.Count - 1].level.Count) + ".prefab";
        }
        else
        {
            AssetDatabase.CreateFolder("Assets/_Game/Prefabs/Level", "Chapter "+ (gameManager.recordLevels.Count));
            localPath = "Assets/_Game/Prefabs/Level/Chapter "+(gameManager.recordLevels.Count)+"/Level 0.prefab";
        }

        localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

        bool prefabSuccess;
        GameObject levelPrefab = PrefabUtility.SaveAsPrefabAssetAndConnect(generatedLevel, localPath, InteractionMode.UserAction, out prefabSuccess);
        if (prefabSuccess == true)
        {
            if (canAddLevelToChapter)
            {
                gameManager.recordLevels[gameManager.recordLevels.Count - 1].level.Add(levelPrefab);
            }
            else
            {
                RecordLevel recordLevel = new RecordLevel();
                recordLevel.chapter = gameManager.recordLevels.Count;
                recordLevel.level.Add(levelPrefab);
                gameManager.recordLevels.Add(recordLevel);
            }
            DestroyImmediate(generatedLevel);
            Debug.Log("Prefab was saved successfully");
        }
        else
            Debug.Log("Prefab failed to save" + prefabSuccess);
    }
    public void DeleteGeneratedPrefab()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (gameManager.recordLevels.Count == 1 && gameManager.recordLevels[0].level.Count ==2)
        {
            Debug.LogError("Can't destroy 2 first chapters, Becareful with your hand !");
            return;
        }
        string filePath = Application.dataPath + "/_Game/Prefabs/Level/Chapter "+ (gameManager.recordLevels.Count - 1) + "/Level " + (gameManager.recordLevels[gameManager.recordLevels.Count - 1].level.Count-1) + ".prefab";

        // check if file exists
        if (!File.Exists(filePath))
        {
            Debug.Log("no "+ filePath + " file exists");
        }
        else
        {

            Debug.Log(filePath + " file exists, deleting...");

            gameManager.recordLevels[gameManager.recordLevels.Count - 1].level.Remove(gameManager.recordLevels[gameManager.recordLevels.Count - 1].level[gameManager.recordLevels[gameManager.recordLevels.Count - 1].level.Count - 1]);
            if(gameManager.recordLevels[gameManager.recordLevels.Count - 1].level.Count == 0)
            {
                gameManager.recordLevels.Remove(gameManager.recordLevels[gameManager.recordLevels.Count - 1]);
            }
            File.Delete(filePath);
            RefreshEditorProjectWindow();
        }
    }
    void RefreshEditorProjectWindow()
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
    }
}
#endif
#if UNITY_EDITOR
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var mapGenerator = (MapGenerator)target;
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if(GUILayout.Button("Generate Map & Enemy"))
        {
            mapGenerator.GenerateBaseMapAndEnemy();
        }
        if (GUILayout.Button("Generate Level"))
        {
            mapGenerator.RandomGenerateLevel();
        }
        if (GUILayout.Button("Add Generated Level to prefab"))
        {
            mapGenerator.AddGeneratedLevelToPrefab();
        }
        if (GUILayout.Button("Delete Generated Level"))
        {
            mapGenerator.DestroyGeneratedLevel();
        }
        GUILayout.Space(10f);
        if (GUILayout.Button("Generate Level (1 click option)"))
        {
            mapGenerator.RandomGenerateLevel();
            mapGenerator.AddGeneratedLevelToPrefab();
        }
        if (GUILayout.Button("Delete Generated Prefab"))
        {
            mapGenerator.DeleteGeneratedPrefab();
        }
    }
}
#endif
