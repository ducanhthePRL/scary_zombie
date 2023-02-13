using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathFinding1 : MonoBehaviour
{
    [SerializeField]
    private Block[] tfs;
    private Dictionary<Vector2, Block> dicBlockPositions = new Dictionary<Vector2, Block>();
    [SerializeField]
    private ObjectIndex player, target;
    private List<RecordNode> lstChild = new List<RecordNode>();
    private Stack<RecordNode> solutions = new Stack<RecordNode>();
    private bool isReachGoal = false;
    private Vector2 maxRowColum = new Vector2();
    private Vector2[] validDirection = new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, -1) };
    private Stack<Vector2> visited = new Stack<Vector2>();
    [SerializeField]
    private Button btRun;

    private void Awake()
    {
        btRun.onClick.AddListener(Run);
    }

    private void Run()
    {
        Reset();
        RecordNode root = CreateRootNode();
        visited.Push(root.index);
        lstChild.Add(root);
        FindPath();
    }

    private void Reset()
    {
        isReachGoal = false;
        visited.Clear();
        lstChild.Clear();
        solutions.Clear();
        foreach (var item in dicBlockPositions)
        {
            item.Value.SetActiveStep(false,Vector3.zero);
            item.Value.SetActiveLock();
        }
    }

    private void Start()
    {
        SaveBlockPositions(5, 5, tfs);
        Reset();
        Run();
    }



    private bool SaveBlockPositions(int max_row, int max_column, Block[] transforms)
    {
        if (transforms == null || transforms.Length == 0)
        {
            Debug.LogError("Save failed");
            return false;
        }
        maxRowColum.x = max_row;
        maxRowColum.y = max_column;
        int row = 0;
        int column = 0;
        int length = transforms.Length;
        for (int i = 0; i < length; i++)
        {
            Block tf = transforms[i];
            Vector2 index = new Vector2(row, column);
            if (!dicBlockPositions.ContainsKey(index))
            {
                dicBlockPositions.Add(index, tf);
            }
            row++;
            if (row >= max_row)
            {
                row = 0;
                column++;
            }
        }
        return true;
    }

    private RecordNode CreateRootNode()
    {
        RecordNode root = new RecordNode();
        root.index = new Vector2(player.row, player.column);
        root.parent = null;
        return root;
    }

    private void FindPath()
    {
        while (lstChild.Count > 0 && isReachGoal == false)
        {
            RecordNode node = lstChild[0];
            lstChild.RemoveAt(0);
            GetChild(node);
        }
        ShowSolution();
    }

    private void GetChild(RecordNode parent)
    {
        for (int i = 0; i < 4; i++)
        {
            RecordNode p = parent;
            Vector2 next_index = p.index + validDirection[i];
            while (IsSafe(p.index, next_index, validDirection[i]))
            {
                RecordNode child = new RecordNode();
                child.index = next_index;
                child.parent = p;
                if (IsReachGoal(next_index))
                {
                    solutions.Push(child);
                    RecordNode node = child;
                    while (node != null)
                    {
                        RecordNode n = node.parent;
                        if (n != null)
                        {
                            solutions.Push(n);
                        }
                        node = n;
                    }
                    isReachGoal = true;
                    return;
                }
                next_index = child.index + validDirection[i];
                p = child;
            }
            if (!visited.Contains(p.index))
            {
                visited.Push(p.index);
                lstChild.Add(p);
            }
        }
    }

    //private void GetChild1(RecordNode parent)
    //{
    //    for (int i = 0; i < 4; i++)
    //    {
    //        Vector2 next_index = parent.index + validDirection[i];
    //        if (IsSafe(next_index) == false)
    //        {

    //            continue;
    //        }
    //        RecordNode child = new RecordNode();
    //        child.index = next_index;
    //        child.parent = parent;
    //        if (IsReachGoal(next_index))
    //        {
    //            solutions.Push(child);
    //            RecordNode node = child;
    //            while (node != null)
    //            {
    //                RecordNode n = node.parent;
    //                if (n != null)
    //                {
    //                    solutions.Push(n);
    //                }
    //                node = n;
    //            }
    //            isReachGoal = true;
    //            return;
    //        }
    //        lstChild.Add(child);
    //    }
    //}

    private void ShowSolution()
    {
        while (solutions.Count > 0)
        {
            RecordNode node = solutions.Pop();
            if (dicBlockPositions.ContainsKey(node.index))
            {
                Block block = dicBlockPositions[node.index];
                block.SetActiveStep(true, node.direction);
            }
        }
    }

    private bool IsSafe(Vector2 self, Vector2 index, Vector2 valid_index)
    {
        bool isLock = false;
        bool isLockSideSelf = false;
        bool isLockSide = false;
        string block_name = "";
        if (dicBlockPositions.ContainsKey(self))
            isLockSideSelf = dicBlockPositions[self].IsLockSideSelf(valid_index);

        if (dicBlockPositions.ContainsKey(index))
        {
            block_name = dicBlockPositions[index].name;
            isLock = dicBlockPositions[index].isLock;
            isLockSide = dicBlockPositions[index].IsLockSide(valid_index);
        }

        return index.x >= 0 && index.x < maxRowColum.x && index.y >= 0 && index.y < maxRowColum.y && isLock == false && isLockSide == false && isLockSideSelf == false;
    }

    private bool IsReachGoal(Vector2 index)
    {
        return index.x == target.row && index.y == target.column;
    }
}
