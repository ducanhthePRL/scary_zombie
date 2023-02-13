using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [HideInInspector] public int numberEnemy;
    [HideInInspector] public int currentNumberEnemy;
    public List<ObjectIndex> listEnemyObject = new List<ObjectIndex>();
    public ObjectIndex dest;
    public int MapSizeWidth;
    public int MapSizeHeight;
    public GameMode gameMode;
    public int timeLimit;
    public int numberStep;
    [HideInInspector] public int currentStep =0;
    [HideInInspector] public int currentTime = 0;
    [HideInInspector] public int level;
    public Block[] tf;
    private int maxReward;
    [SerializeField] private GameObject hintObject;
    private void Start()
    {
        hintObject?.SetActive(false);
    }
    public void Init()
    {
        if (level == 0)
        {
            maxReward = 0;
        }
        else {
            RewardLevel reward = DataController.Instance.RewardVO.GetDataByName<RewardLevel>("Reward", $"{level}");
            maxReward = reward.reward;
        }
        numberEnemy = listEnemyObject.Count;
        currentNumberEnemy = numberEnemy;
    }
    public void ShowHint()
    {
        hintObject?.SetActive(true);
    }
    public int GetReward()
    {
        if(gameMode == GameMode.NORMAL)
        {
            return maxReward - (currentStep>8?Mathf.RoundToInt(maxReward * 0.2f):0);
        }
        else if (gameMode == GameMode.TIME_LIMIT)
        {
            return maxReward;
        }
        else if( gameMode == GameMode.STEP_COUNT)
        {
            return maxReward;
        }
        return 0;
    }
}
public enum GameMode
{
    NORMAL,
    TIME_LIMIT,
    STEP_COUNT
}
public struct RewardLevel
{
    public int reward;
}