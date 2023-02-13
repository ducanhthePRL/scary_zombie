using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    [SerializeField] private ParticleSystem hintTrail;
    [SerializeField] private EnemyOrder enemyOrderPrefab;
    [SerializeField] private List<Block> edgeList;
    [SerializeField] private List<Enemy> enemyOrderList;
    private LineRenderer _lineRenderer;
    private float timeUnit = 0.25f;
    private int showTime;
    private void OnEnable()
    {
        showTime = 0;
        SetStartPosition();
        hintTrail.Play();
        ShowHint();
        RenderLine();
        SetEnemyOrderList();
    }
    private void SetEnemyOrderList()
    {
        for(int i = 0; i < enemyOrderList.Count; i++)
        {
            if (enemyOrderList[i].enemyOrder == null)
            {
                EnemyOrder enemyOrder = Instantiate(enemyOrderPrefab, enemyOrderList[i].transform);
                enemyOrderList[i].enemyOrder = enemyOrder;
                enemyOrder.SetOrder(i + 1);
            }
            else
            {
                if (enemyOrderList[i] != null)
                {
                    enemyOrderList[i].enemyOrder.gameObject.SetActive(true);
                }
            }
        }
    }
    private void CloseEnemyOrderList()
    {
        for (int i = 0; i < enemyOrderList.Count; i++)
        {
            if (enemyOrderList[i] != null)
            {
                enemyOrderList[i].enemyOrder.gameObject.SetActive(false);
            }
        }
    }
    private void RenderLine()
    {
        if (_lineRenderer == null) _lineRenderer = GetComponent<LineRenderer>();
        if (_lineRenderer == null) _lineRenderer = gameObject.AddComponent<LineRenderer>();
        _lineRenderer.positionCount = edgeList.Count;
        for(int i = 0; i<edgeList.Count; i++)
        {
            _lineRenderer.SetPosition(i, new Vector3(edgeList[i].transform.position.x, 0f, edgeList[i].transform.position.z));
        }
    }
    private void StopRenderLine()
    {
        _lineRenderer.positionCount = 0;
    }
    int i = -1;
    private void SetStartPosition()
    {
        hintTrail.transform.position = new Vector3(edgeList[0].transform.position.x, 0.6f, edgeList[0].transform.position.z);
    }
    private void ShowHint()
    {
        if (i < edgeList.Count-1)
        {
            i++;
        }
        else
        {
            hintTrail.Stop();
            showTime++;
            SetStartPosition();
            i = 0;
            if (showTime == 3)
            {
                StopRenderLine(); 
                CloseEnemyOrderList();
                gameObject.SetActive(false);
                return;
            }
            hintTrail.Play();
        }
        LeanTween.move(hintTrail.gameObject, new Vector3(edgeList[i].transform.position.x,0.6f, edgeList[i].transform.position.z), 
            timeUnit * Vector3.Distance(hintTrail.transform.position, edgeList[i].transform.position))
            .setOnComplete(ShowHint);
    }
}
