using CodeStage.AntiCheat.ObscuredTypes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PathFinding : MonoBehaviour
{
    private Dictionary<Vector2, Block> dicBlockPositions = new Dictionary<Vector2, Block>();
    public ObjectIndex player, target;
    private List<RecordNode> lstChild = new List<RecordNode>();
    private Stack<RecordNode> solutions = new Stack<RecordNode>();
    private bool isReachGoal = false;
    private Vector2 maxRowColum = new Vector2();
    private Vector2[] validDirection = new Vector2[] { new Vector2(0, 1), new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, -1) };
    private Stack<Vector2> visited = new Stack<Vector2>();
    private ObscuredFloat moveTimeUnit = 7;

    public void ResetPlayerPosition()
    {
        player.column = (int)player.transform.position.z;
        player.row = (int)player.transform.position.x;
    }
    public void ResetTargetPosition()
    {
        target.column = (int)target.transform.position.z;
        target.row = (int)target.transform.position.x;
    }

    public void Run()
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
        }
    }

    public bool SaveBlockPositions(int max_row, int max_column, Block[] transforms)
    {
        if (transforms == null || transforms.Length == 0)
        {
            Debug.LogError("Save failed");
            return false;
        }
        dicBlockPositions.Clear();
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
            column++;
            if (column >= max_column)
            {
                column = 0;
                row++;
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
        while (lstChild.Count > 0 && !isReachGoal)
        {
            RecordNode node = lstChild[0];
            lstChild.RemoveAt(0);
            GetChild(node);
        }
        if (!isReachGoal)
        {
            PopUpNotice popupNotice = PanelManager.Show<PopUpNotice>();
            popupNotice.OnSetTextOneButton("Hint", "Cannot find path to destination");
        }
        else
        {
            ShowSolution();
        }
    }

    private void Start()
    {
        Observer.Instance.AddObserver(ObserverKey.UserSwipe, PlayerMoveOne);
        Observer.Instance.AddObserver(ObserverKey.EnemyDodge, EnemyMoveOne);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.UserSwipe, PlayerMoveOne);
        Observer.Instance.RemoveObserver(ObserverKey.EnemyDodge, EnemyMoveOne);
    }

   
    private MoveComponent _playerMoveComponent;
    public MoveComponent playerMoveComponent
    {
        get
        {
            if (_playerMoveComponent == null) _playerMoveComponent = player.GetComponent<MoveComponent>();
            return _playerMoveComponent;
        }
    }
    private void PlayerMoveOne(object data)
    {
        Vector2 direction = (Vector2)data;

        ResetPlayerPosition();
        MoveOne(player, direction, delegate () {
            //removed idle here
            playerMoveComponent.isMoving = false;
            Observer.Instance.Notify(ObserverKey.PlayerStopMove, null);
            ResetPlayerPosition();
        });
    }
    private void EnemyMoveOne(object data)
    {
        DataDodge dataDodge = (DataDodge)data;
        Enemy enemy = dataDodge.oi.GetComponent<Enemy>();
        MoveOne(dataDodge.oi, dataDodge.direction, delegate () {
            enemy.isMoving = false;
            enemy.transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            enemy.BeScared();
            enemy.SetIndex();
        }, false);
    }
    private void MoveOne(ObjectIndex oi, Vector2 direction, Action onDone, bool is_player = true)
    {
        Vector2 index = new Vector2(oi.row, oi.column);
        Vector2 next_index = index + direction;
        int count_safe = 0;
        Vector2 tmp_index = Vector2.zero;
        while (IsSafe(index, next_index, direction,false))
        {
            tmp_index = next_index;
            next_index = tmp_index + direction;
            index = tmp_index;
            count_safe++;
            if (isLockByTrap())
            {
                break;
            }
        }
        if (count_safe == 0)
        {
            onDone?.Invoke();
            return;
        }
        Vector3 targetDirection = new Vector3(direction.x, 0, direction.y);

        if (is_player)
        {
            TPRLSoundManager.Instance.PlaySoundFx("swish");

            DataEffect dataMove = new DataEffect();
            Vector3 moveDirection = new Vector3(direction.x, 0, direction.y);
            dataMove.direction = moveDirection;
            dataMove.startPosition = player.transform.position;
            Observer.Instance.Notify(ObserverKey.PlayerMove, dataMove);
            playerMoveComponent.Slide();
        };
        float ray_spacing_x = is_player ? 0.5f  : -0.25f;
        Ray ray = new Ray(oi.transform.position + ray_spacing_x * Vector3.up , targetDirection);
        RaycastHit hit;

        Vector3 dest = Vector3.zero;
        Enemy enemy;
        Dest destComp;
        Trap trap;
        if (Physics.Raycast(ray, out hit))
        {
            if ((direction.x > 0 && hit.transform.position.x <= tmp_index.x) || (direction.x < 0 && hit.transform.position.x >= tmp_index.x)
                || (direction.y > 0 && hit.transform.position.z <= tmp_index.y) || (direction.y < 0 && hit.transform.position.z >= tmp_index.y)
                )
            {
                enemy = hit.transform.GetComponent<Enemy>();
                destComp = hit.transform.GetComponent<Dest>();
                trap = hit.transform.GetComponent<Trap>();
                //Only Player check zone
                if (enemy != null && is_player)
                {
                    enemy.SetIndex();
                    dest = hit.transform.position - targetDirection - 0.75f * Vector3.up;
                    Move(oi.gameObject, dest, delegate {
                        playerMoveComponent.isMoving = false;
                        ResetPlayerPosition();
                        Observer.Instance.Notify(ObserverKey.CollideEnemy, enemy);
                    });
                }
                else if (destComp != null && is_player)
                {
                    dest = hit.transform.position - targetDirection - 0.25f * Vector3.up;
                    Move(oi.gameObject, dest, delegate { Observer.Instance.Notify(ObserverKey.CollideFinish, destComp); });
                }
                //Both Player & Enemy check zone
                else if (trap != null)
                {
                    Vector3 target = hit.transform.position - targetDirection;
                    dest = new Vector3(target.x, oi.transform.position.y, target.z);
                    Move(oi.gameObject, dest, delegate {
                        onDone?.Invoke();
                        trap.OnCollide(oi.GetComponent<Collider>());
                    });
                }
                else
                {
                    dest = new Vector3(tmp_index.x, oi.transform.position.y, tmp_index.y);
                    Move(oi.gameObject, dest, onDone);
                }
            }
            else
            {
                dest = new Vector3(tmp_index.x, oi.transform.position.y, tmp_index.y);
                Move(oi.gameObject, dest, onDone);
            }
        }
        else
        {
            dest = new Vector3(tmp_index.x, oi.transform.position.y, tmp_index.y);
            Move(oi.gameObject, dest, onDone);
        }
    }

    private void Move(GameObject source, Vector3 dest, Action on_done)
    {
        float fixedTime = Time.fixedDeltaTime;
        LeanTween.move(source, dest, fixedTime * moveTimeUnit).setOnComplete(on_done);
    }

    //private bool IsPlayer(ObjectIndex oi)
    //{
    //    return (oi.gameObject.layer.CompareTo(LayerMask.NameToLayer("Player")) == 0);
    //}

    private void GetChild(RecordNode parent)
    {
        for (int i = 0; i < 4; i++)
        {
            RecordNode p = parent;
            Vector2 next_index = p.index + validDirection[i];
            while (IsSafe(p.index, next_index, validDirection[i],true))
            {
                RecordNode child = new RecordNode();
                child.index = next_index;
                child.parent = p;
                if (IsReachGoal(next_index))
                {
                    child.direction = validDirection[i];
                    solutions.Push(child);
                    RecordNode node = child;
                    //Record Solution
                    while (node != null)
                    {
                        RecordNode n = node.parent;
                        if (n != null)
                        {
                            n.direction = node.index - n.index;
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
            if (!visited.Contains(p.index) && !isLockByTrap())
            {
                visited.Push(p.index);
                lstChild.Add(p);
            }
        }
    }
    private void ShowSolution()
    {
        while (solutions.Count > 0)
        {
            RecordNode node = solutions.Pop();
            if (dicBlockPositions.ContainsKey(node.index))
            {
                Block block = dicBlockPositions[node.index];
                block.SetActiveStep(true,node.direction);
            }
        }
    }
    bool isBlockSide = false;
    List<Direction> trapLock;
    bool isLockAnotherSide = false;
    private bool IsSafe(Vector2 self, Vector2 index, Vector2 valid_index,bool is_hint)
    {
        bool isLockSide = false;
        bool isLock = false;
        bool isLockSideSelf = false;
        string block_name = "";
        if (dicBlockPositions.ContainsKey(self)) {
            isLockSideSelf = dicBlockPositions[self].IsLockSideSelf(valid_index);
        }
        if (dicBlockPositions.ContainsKey(index))
        {
            block_name = dicBlockPositions[index].name;
            isLock = dicBlockPositions[index].isLock;
            isLockSide = dicBlockPositions[index].IsLockSide(valid_index);
            isLockAnotherSide = dicBlockPositions[index].IsLockAnotherSide(valid_index);
            isBlockSide = dicBlockPositions[index].isTrap;
            trapLock = dicBlockPositions[index].trapLock;
        }
        return index.x >= 0 && index.x < maxRowColum.x && index.y >= 0 && index.y < maxRowColum.y && isLock == false && isLockSide == false && isLockSideSelf == false
            && (!isLockByTrap()||!is_hint);
    }
    private bool isLockByTrap()
    {
        return ((isBlockSide && trapLock.Count==0) || (isBlockSide && trapLock.Count >0 && isLockAnotherSide));
    }

    private bool IsReachGoal(Vector2 index)
    {
        return index.x == target.row && index.y == target.column;
    }
}

public class RecordNode
{
    public Vector2 index;
    public RecordNode parent;
    public Vector2 direction;
}