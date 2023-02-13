using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Enemy : EntityManager
{
    protected int health = 1;
    private int mapSizeWidth = 10;
    private int mapSizeHeight = 10;
    private DataEffect dataEffect;
    private DataDodge dataDodge;

    [SerializeField] private Animator animator;
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField]
    public EnemyEmotion enemyEmotion;
    public EnemyDeadReaction enemyDeadReaction;
    public EnemyOrder enemyOrder;
    [SerializeField]
    private MeshDestroy meshDestroy;
    private ObjectIndex _objectIndex;
    private ObjectIndex objectIndex
    {
        get
        {
            if (_objectIndex == null) _objectIndex = GetComponent<ObjectIndex>();
            return _objectIndex;
        }
    }
    private Rigidbody _rigidBody;
    private Rigidbody rigidBody
    {
        get
        {
            if (_rigidBody == null) _rigidBody = GetComponent<Rigidbody>();
            return _rigidBody;
        }
    }
    [SerializeField] private EntityAttack entityAttack;
    [SerializeField] private bool canAttack;

    protected override void Start()
    {
        base.Start();
        dataEffect = new DataEffect();
        dataDodge = new DataDodge();
        if (isMovingEnemy)
        {
            if (isHorizontalPatrol)
                PatrolLeftRight(step);
            else
                PatrolUpDown(step);
            InvokeRepeating("SetIndex", 0, 0.1f);
        }
        entityAttack.gameObject.SetActive(canAttack);
        enemyEmotion.PlayEmotion(Emotion.ANY);
        Observer.Instance.AddObserver(ObserverKey.PlayerStopMove, BeScaredNearPlayer);
        SetIndex();
        RandomAnimation();
    }
    private void RandomAnimation()
    {
        int rdValue = UnityEngine.Random.Range(0, 2);
        if(rdValue == 0)
        {
            animator.Play("Idle");
        }
        else if(rdValue == 1)
        {
            animator.Play("Screem");
        }
        else if(rdValue == 2)
        {
            animator.Play("Turn");
        }
    }
    public void SetIndex()
    {
        objectIndex.column = Mathf.RoundToInt(transform.position.z);
        objectIndex.row = Mathf.RoundToInt(transform.position.x);
    }
    public void SetMapSize(int map_size_width,int map_size_height)
    {
        mapSizeWidth = map_size_width;
        mapSizeHeight = map_size_height;
    }
    public void DecreaseHealth(EnemyDeadType deadtype,Vector3 player_position,Action onDone)
    {
        health--;
        if (health <= 0)
        {
            Dead(deadtype, player_position, onDone);
        }
    }
    public override void OnFireCallback()
    {
        dataEffect.startPosition = Vector3.one * -1f;
        dataEffect.enemy = GetComponent<ObjectIndex>();
        Observer.Instance.Notify(ObserverKey.EnemyDead, dataEffect);
        Destroy(gameObject);
    }
    public override void Fall(Transform hole)
    {
        StartCoroutine(IEFall(hole));
    }
    public override void Booooooooooooooooooooooooooooooooom(ObjectIndex objectIndex)
    {
        StartCoroutine(IEBooooooooooooooooooooooooooooooooom(objectIndex));
    }
    private IEnumerator IEBooooooooooooooooooooooooooooooooom(ObjectIndex objectIndex)
    {
        PrepareToDie();
        DestroyMesh(objectIndex);
        yield return new WaitForSeconds(1);
        dataEffect.startPosition = Vector3.one * -1f;
        dataEffect.enemy = GetComponent<ObjectIndex>();
        Observer.Instance.Notify(ObserverKey.EnemyDead, dataEffect);
        Destroy(gameObject);
    }
    private void DestroyMesh(ObjectIndex objectIndex)
    {
        meshDestroy.CutCascades = 5;
        meshDestroy.ExplodeForce = 200;
        Vector3 startPosition = new Vector3(objectIndex.row, 0, objectIndex.column);
        meshDestroy.DestroyMesh(startPosition);
    }
    protected virtual void Dead(EnemyDeadType deadtype, Vector3 player_position,Action onDone)
    {
        StartCoroutine(IEDead(deadtype, player_position, onDone));
    }
    private IEnumerator IEDead(EnemyDeadType deadtype, Vector3 player_position,Action onDone)
    {
        PrepareToDie();
        DestroyMesh(deadtype, player_position);
        Observer.Instance.Notify(ObserverKey.Bleeding, dataEffect);
        yield return new WaitForSeconds(1.5f);
        Observer.Instance.Notify(ObserverKey.EnemyDead, dataEffect);
        ScareNearestEnemy();
        onDone?.Invoke();
        Observer.Instance.Notify(ObserverKey.PlayerStopMove, false);
        Destroy(gameObject);
    }
    private void DestroyMesh(EnemyDeadType deadtype, Vector3 player_position)
    {
        switch (deadtype)
        {
            case EnemyDeadType.JumpAttack:
                meshDestroy.CutCascades = 0;
                meshDestroy.DestroyMesh(player_position);
                break;
            case EnemyDeadType.Kick:
                meshDestroy.CutCascades = 0;
                meshDestroy.DestroyMesh(player_position);
                break;
            case EnemyDeadType.Strike:
                meshDestroy.CutCascades = 0;
                meshDestroy.ExplodeForce = 400;
                meshDestroy.DestroyMesh(player_position);
                break;
        }
    }
    private void PrepareToDie()
    {
        if (canAttack)
        {
            entityAttack.ResetBlock(transform.position); 
            entityAttack.gameObject.SetActive(false);
        }
        LeanTween.cancel(gameObject);
        enemyEmotion.StopEmotion();
        isMoving = false;
        dataEffect.startPosition = transform.position;
        dataEffect.enemy = GetComponent<ObjectIndex>();
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -90));
        skinnedMeshRenderer.enabled = false;
        boxCollider.enabled = false;
        enemyOrder?.gameObject.SetActive(false);
        enemyDeadReaction.gameObject.SetActive(false);
    }
    public void DeadWithoutAnyEffects()
    {
        PrepareToDie();
        Observer.Instance.Notify(ObserverKey.EnemyDead, dataEffect);
        Observer.Instance.Notify(ObserverKey.PlayerStopMove, null);
        Destroy(gameObject);
    }
    private void BeScaredNearPlayer(object data)
    {
        if (data != null)
        {
            bool check = (bool)data;
            if (!check) return;
        }
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.6f);
        Transform player = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                if (colliders[i].tag == "Player")
                {
                    player = colliders[i].transform;
                }
            }
        }
        if (player != null)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject != gameObject)
                {
                    if (colliders[i].tag != "Player")
                    {
                        Block block = colliders[i].GetComponent<Block>();
                        if (block != null)
                        {
                            if ((block.isLockBot && Mathf.Round(Mathf.Round(transform.position.x)- Mathf.Round(player.position.x)) == 1)
                                || (block.isLockTop && Mathf.Round(Mathf.Round(transform.position.x) - Mathf.Round(player.position.x)) == -1)
                                || (block.isLockLeft && Mathf.Round(Mathf.Round(transform.position.z) - Mathf.Round(player.position.z)) == -1)
                                || (block.isLockRight && Mathf.Round(Mathf.Round(transform.position.z) - Mathf.Round(player.position.z)) == 1))
                            {
                                return;
                            }
                        }
                    }
                }
                if (i == colliders.Length - 1)
                {
                    Dodge(player);
                }
            }
        }
    }
    public virtual void ScareNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 0.6f * Mathf.Sqrt(2));
        for(int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                Enemy enemy = colliders[i].GetComponent<Enemy>();
                if (enemy != null && enemy.dodgeDirection.Count>0 && enemy.dodgeTime + 1 <=enemy.dodgeDirection.Count-1&& enemy.dodgeDirection[enemy.dodgeTime+1] != Direction.NONE)
                {
                    enemy.Dodge(null);
                }
            }
        }
    }
    public void BeScared()
    {
        enemyEmotion.PlayEmotion(Emotion.SCARY);
        animator.Play("Terrified");
    }
    public void BeNormal()
    {
        animator.Play("Idle");
    }
    public virtual void Dodge(Transform player)
    {
        moveTimeUnit = 0.05f;
        TryDodge(player);
        BeScared();
    }
    private void TryDodge(Transform player)
    {
        int step = mapSizeHeight > mapSizeWidth ? mapSizeHeight : mapSizeWidth;
        objectIndex.column = (int)transform.position.z;
        objectIndex.row = (int)transform.position.x;
        dodgeTime++;
        if (dodgeTime <= dodgeDirection.Count - 1)
        {
            if (player != null)
            {
                if ((Mathf.Round(objectIndex.column - Mathf.Round(player.position.z)) == 1 && dodgeDirection[dodgeTime] == Direction.DOWN) ||
                    (Mathf.Round(objectIndex.column - Mathf.Round(player.position.z)) == -1 && dodgeDirection[dodgeTime] == Direction.UP) ||
                    (Mathf.Round(objectIndex.row - Mathf.Round(player.position.x)) == 1 && dodgeDirection[dodgeTime] == Direction.LEFT) ||
                    (Mathf.Round(objectIndex.row - Mathf.Round(player.position.x)) == -1 && dodgeDirection[dodgeTime] == Direction.RIGHT))
                {
                    dodgeTime--;
                    return;
                }
            }
            if (dodgeDirection[dodgeTime] == Direction.RIGHT) TryMoveRight(step, true);
            else if (dodgeDirection[dodgeTime] == Direction.LEFT) TryMoveLeft(step, true);
            else if (dodgeDirection[dodgeTime] == Direction.UP) TryMoveUp(step, true);
            else if (dodgeDirection[dodgeTime] == Direction.DOWN) TryMoveDown(step, true);
        }
        else
        {
            dodgeTime--;
        }
    }
    [HideInInspector]
    public bool isMoving = false;
    public float moveTimeUnit;
    protected void TryMoveLeft(int step, bool is_scared)
    {
        if (!isMoving)
        {
            if (transform.position.x == 0) return;
            else
            {
                isMoving = true;
                transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
                moveDirection = Vector3.left;
                moveDirectionV2 = Vector2.left;
                if (!is_scared)
                {
                    if (transform.position.x - step < 0) step = Mathf.RoundToInt(transform.position.x);
                    animator.Play("Walking");
                    LeanTween.moveX(gameObject, transform.position.x - step, step * moveTimeUnit).setOnComplete(() => {
                        isMoving = false;
                        BeNormal();
                    });
                }
                else
                {
                    dataDodge.direction = moveDirectionV2;
                    dataDodge.oi = objectIndex;
                    Observer.Instance.Notify(ObserverKey.EnemyDodge, dataDodge);
                }
            }
        }
    }
    protected void TryMoveRight(int step, bool is_scared)
    {
        if (!isMoving)
        {
            if (transform.position.x == mapSizeWidth - 1) return;
            else
            {
                isMoving = true;
                transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
                moveDirection = Vector3.right;
                moveDirectionV2 = Vector2.right;
                if (!is_scared)
                {
                    if (transform.position.x + step > mapSizeWidth - 1) step = mapSizeWidth - 1 - Mathf.RoundToInt(transform.position.x);
                    animator.Play("Walking");
                    LeanTween.moveX(gameObject, transform.position.x + step, step * moveTimeUnit).setOnComplete(() =>
                    {
                        isMoving = false;
                        BeNormal();
                    });
                }
                else
                {
                    dataDodge.direction = moveDirectionV2;
                    dataDodge.oi = objectIndex;
                    Observer.Instance.Notify(ObserverKey.EnemyDodge, dataDodge);
                }
            }
        }
    }
    protected void TryMoveDown(int step, bool is_scared)
    {
        if (!isMoving)
        {
            if (transform.position.z == 0) return;
            else
            {
                isMoving = true;
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
                moveDirection = Vector3.back;
                moveDirectionV2 = Vector2.down;
                if (!is_scared)
                {
                    if (transform.position.z - step < 0) step = Mathf.RoundToInt(transform.position.z);
                    animator.Play("Walking");
                    LeanTween.moveZ(gameObject, transform.position.z - step, step * moveTimeUnit).setOnComplete(() =>
                    {
                        isMoving = false;
                        BeNormal();
                    });
                }
                else
                {
                    dataDodge.direction = moveDirectionV2;
                    dataDodge.oi = objectIndex;
                    Observer.Instance.Notify(ObserverKey.EnemyDodge, dataDodge);
                }
            }
        }
    }
    protected void TryMoveUp(int step,bool is_scared)
    {
        if (!isMoving)
        {
            if (transform.position.z == mapSizeHeight-1) return;
            else
            {
                isMoving = true;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                moveDirection = Vector3.forward;
                moveDirectionV2 = Vector2.up;
                if (!is_scared)
                {
                    if (transform.position.z + step > mapSizeHeight - 1) step = mapSizeHeight - 1 - Mathf.RoundToInt(transform.position.z);
                    animator.Play("Walking");
                    LeanTween.moveZ(gameObject, transform.position.z + step, step * moveTimeUnit).setOnComplete(() =>
                    {
                        isMoving = false;
                        BeNormal();
                    });
                }
                else
                {
                    dataDodge.direction = moveDirectionV2;
                    dataDodge.oi = objectIndex;
                    Observer.Instance.Notify(ObserverKey.EnemyDodge, dataDodge);
                }
            }
        }
    }
    private Vector3 moveDirection;
    private Vector2 moveDirectionV2;
    private IEnumerator IEFall(Transform hole)
    {
        Vector3 vectorFallStart = hole.position;
        Vector3 vectorFallFinish = transform.position + 30 * Vector3.down;
        LeanTween.move(gameObject, vectorFallStart, 0.5f);
        yield return new WaitForSeconds(0.5f);
        animator.Play("Teeter");
        enemyDeadReaction.PlayDeadRect(EnemyDeadType.FALL);
        yield return new WaitForSeconds(0.5f);
        LeanTween.move(gameObject, vectorFallFinish, 2f);
        yield return new WaitForSeconds(2f);
        dataEffect.startPosition = Vector3.one * -1f;
        dataEffect.enemy = GetComponent<ObjectIndex>();
        Observer.Instance.Notify(ObserverKey.EnemyDead, dataEffect);
        Destroy(gameObject);
    }
    protected override IEnumerator IEFallIntoWater()
    {
        Vector3 vectorFallStart = transform.position;
        Vector3 vectorFallFinish = transform.position + 30 * Vector3.down;
        LeanTween.move(gameObject, vectorFallStart, 0.5f);
        yield return new WaitForSeconds(0.5f);
        animator.Play("Fall Into Water");
        enemyDeadReaction.PlayDeadRect(EnemyDeadType.FALL);
        yield return new WaitForSeconds(0.5f);
        LeanTween.move(gameObject, vectorFallFinish, 1f);
        yield return new WaitForSeconds(1f);
        dataEffect.startPosition = Vector3.one * -1f;
        dataEffect.enemy = GetComponent<ObjectIndex>();
        Observer.Instance.Notify(ObserverKey.EnemyDead, dataEffect);
        Destroy(gameObject);
    }
    protected override IEnumerator IEFire()
    {
        Vector3 vectorFallStart = transform.position;
        Vector3 vectorFallFinish = transform.position + 30 * Vector3.down;
        LeanTween.move(gameObject, vectorFallStart, 0.5f);
        yield return new WaitForSeconds(1f);
        animator.Play("Teeter");
        enemyDeadReaction.PlayDeadRect(EnemyDeadType.FIRE);
        yield return new WaitForSeconds(1f);
        float valueSet = 0f;
        while (valueSet < 1f)
        {
            valueSet += 0.05f;
            material.SetFloat("_SliceAmount", valueSet);
            yield return new WaitForSeconds(0.1f);
        };
        OnFire.Invoke();
    }
    public bool isMovingEnemy =false;
    public int step = 2;
    public bool isHorizontalPatrol;
    public int dodgeTime = -1;
    [SerializeField] public List<Direction> dodgeDirection;
    protected void PatrolLeftRight(int step)
    {
        StartCoroutine(IEPatrolLeftRight(step));
    }
    protected void PatrolUpDown(int step)
    {
        StartCoroutine(IEPatrolUpDown(step));
    }
    private IEnumerator IEPatrolLeftRight(int step)
    {
        while (true)
        {
            moveTimeUnit = 2f;
            TryMoveLeft(step, false);
            yield return new WaitUntil(() => !isMoving);
            yield return new WaitForSeconds(2f);
            TryMoveRight(step, false);
            yield return new WaitUntil(() => !isMoving);
            yield return new WaitForSeconds(2f);
        }
    }
    private IEnumerator IEPatrolUpDown(int step)
    {
        while (true)
        {
            moveTimeUnit = 2f;
            TryMoveUp(step, false);
            yield return new WaitUntil(() => !isMoving);
            yield return new WaitForSeconds(2f);
            TryMoveDown(step, false);
            yield return new WaitUntil(() => !isMoving);
            yield return new WaitForSeconds(2f);
        }
    }
    private void OnDestroy()
    {
        CancelInvoke();
        Observer.Instance.RemoveObserver(ObserverKey.PlayerStopMove, BeScaredNearPlayer);
    }

}
public class DataDodge
{
    public ObjectIndex oi;
    public Vector2 direction;
}
public enum EnemyDeadType
{
    None =0,
    JumpAttack,
    Strike,
    Kick,
    FIRE,
    FALL,
}
public enum Direction
{
    NONE=0,
    LEFT,
    RIGHT,
    UP,
    DOWN
}