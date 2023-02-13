using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveComponent : EntityManager
{
    public bool isMoving = false;
    public bool moveable = true;
    private int mapSizeWidth = 10;
    private int mapSizeHeight = 10;

    [SerializeField] private GameObject virtualCam;
    public GameObject virtualCamera => virtualCam;
    [SerializeField] private Transform followTransform;

    [SerializeField] private Transform weaponParent;
    [HideInInspector] public GameObject weapon;

    private ObjectIndex _objectIndex;
    private ObjectIndex objectIndex
    {
        get
        {
            if (_objectIndex == null) _objectIndex = GetComponent<ObjectIndex>();
            return _objectIndex;
        }
    }

    private BoxCollider _boxCollider;
    private BoxCollider boxCollider
    {
        get
        {
            if (_boxCollider == null) _boxCollider = GetComponent<BoxCollider>();
            return _boxCollider;
        }
    }
    private PlayerAnimationEvent _playerAnimationEvent;
    private PlayerAnimationEvent playerAnimationEvent
    {
        get
        {
            if (_playerAnimationEvent == null) _playerAnimationEvent = GetComponentInChildren<PlayerAnimationEvent>();
            return _playerAnimationEvent;
        }
    }

    [SerializeField]
    private Animator animator;
    [SerializeField]
    private MeshDestroy meshDestroy;

    public void SetMapSize(int map_size_width,int map_size_height)
    {
        mapSizeWidth = map_size_width;
        mapSizeHeight = map_size_height;
    }
    private void CollideEnemy(object data)
    {
        Enemy enemy = (Enemy)data;
        OnCollide(enemy.transform);
        enemy.StopAllCoroutines();
        if (enemy.dodgeTime < enemy.dodgeDirection.Count - 1)
        {
            if((Mathf.Round(transform.position.x) - Mathf.Round(enemy.transform.position.x) == 1 && enemy.dodgeDirection[enemy.dodgeTime+1] == Direction.RIGHT)
                || (Mathf.Round(transform.position.x) - Mathf.Round(enemy.transform.position.x) == -1 && enemy.dodgeDirection[enemy.dodgeTime + 1] == Direction.LEFT)
                || (Mathf.Round(transform.position.z) - Mathf.Round(enemy.transform.position.z) == 1 && enemy.dodgeDirection[enemy.dodgeTime + 1] == Direction.UP)
                || (Mathf.Round(transform.position.z) - Mathf.Round(enemy.transform.position.z) == -1 && enemy.dodgeDirection[enemy.dodgeTime + 1] == Direction.DOWN))
            {
                enemy.enemyEmotion.StopEmotion();
                Rigidbody rigidbody = enemy.gameObject.GetComponent<Rigidbody>();
                rigidbody.isKinematic = true;
                rigidbody.constraints = RigidbodyConstraints.FreezePosition;
                Attack(enemy);
            }
            else
            {
                enemy.Dodge(transform);
            }
        }
        else
        {
            enemy.enemyEmotion.StopEmotion();
            Rigidbody rigidbody = enemy.gameObject.GetComponent<Rigidbody>();
            rigidbody.isKinematic = true;
            rigidbody.constraints = RigidbodyConstraints.FreezePosition;
            Attack(enemy);
        }
    }
    private void CollideDest(object data)
    {
        Dest dest = (Dest)data;
        OnCollide(dest.transform);
        if (dest.canFinish)
        {
            boxCollider.enabled = false;
            JumpToFinish();
        }
    }
    public override void StopAction()
    {
        base.StopAction();
        OnCollide(null);
        moveable = false;
    }
    public void Spawn()
    {
        StartCoroutine(IESpawn());
    }
    private IEnumerator IESpawn()
    {
        moveable = false;
        animator.SetTrigger("Appear");
        yield return new WaitForSeconds(2f);
        if (weapon == null)
        {
            animator.SetTrigger("Idle");
        }
        else
        {
            animator.SetTrigger("Sword Idle");
        }
        yield return new WaitForSeconds(0.5f);
        moveable = true;
    }
    private void JumpToFinish()
    {
        StartCoroutine(IEJumpToFinish());
    }
    private IEnumerator IEJumpToFinish()
    {
        moveable = false;
        animator.ResetTrigger("Slide");
        animator.SetTrigger("Jumping");
        LeanTween.move(gameObject, transform.position + moveDirection, 1f).setEaseInCirc();
        yield return new WaitForSeconds(1.5f);
        Observer.Instance.Notify(ObserverKey.PassLevel);
    }
    private Vector3 camPosition;
    private Quaternion camRotation;
    private Quaternion followTransformRotation;
    protected override void Start()
    {
        base.Start();
        virtualCam.SetActive(false);
        EquipItem();
        if (weapon != null)
        {
            weapon.SetActive(true);
        }
        camPosition = Camera.main.transform.position;
        camRotation = Camera.main.transform.rotation;
        followTransformRotation = followTransform.transform.rotation;
        Observer.Instance.AddObserver(ObserverKey.EnemyDead, OnEnemyDead);
        Observer.Instance.AddObserver(ObserverKey.OutOfStep, OutOfStep);
        Observer.Instance.AddObserver(ObserverKey.ArrowButtonClick, ArrowButtonClick);
        Observer.Instance.AddObserver(ObserverKey.CollideEnemy, CollideEnemy);
        Observer.Instance.AddObserver(ObserverKey.CollideFinish, CollideDest);
    }
    private void EquipItem()
    {
        if (UserDatas.user_Data.inventory_weapons != null)
        {
            foreach (RecordItem recordItem in UserDatas.user_Data.inventory_weapons)
            {
                if (recordItem.is_equiped)
                {
                    weapon = Instantiate(Resources.Load<GameObject>($"Prefabs/Weapon/{recordItem.name}"), weaponParent);
                    playerAnimationEvent.slashSound = recordItem.name;
                }
            }
        }
        else
        {
            playerAnimationEvent.slashSound = "kick";
        }
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.EnemyDead, OnEnemyDead);
        Observer.Instance.RemoveObserver(ObserverKey.OutOfStep, OutOfStep);
        Observer.Instance.RemoveObserver(ObserverKey.ArrowButtonClick, ArrowButtonClick);
        Observer.Instance.RemoveObserver(ObserverKey.CollideEnemy, CollideEnemy);
        Observer.Instance.RemoveObserver(ObserverKey.CollideFinish, CollideDest);
    }
    private void OutOfStep(object data)
    {
        moveable = false;
    }
    private void OnEnemyDead(object data)
    {
        DataEffect dataEffect = (DataEffect)data;
        virtualCam.SetActive(false);
        Camera.main.transform.position = new Vector3(8 + mapSizeWidth / 2, 14.5f, -11);
        Camera.main.transform.rotation = Quaternion.Euler(new Vector3(40, -32, -10));
        Camera.main.fieldOfView = 27 + mapSizeWidth;
        if (!dataEffect.startPosition.Equals(Vector3.one*-1f)) { 
            LeanTween.rotate(followTransform.gameObject, followTransformRotation.eulerAngles, 0.1f);
            moveable = true;
            boxCollider.enabled = true;
            if (weapon != null)
            {
                weapon.SetActive(true);
            }
        }
    }
    private void PrepareToKillEnemy()
    {
        moveable = false;
        virtualCam.SetActive(true);
        boxCollider.enabled = false;
    }
    private void RotateCameraToKillEnemy()
    {
        Vector3 targetDirection = Vector3.zero;
        if (moveDirection == Vector3.left || moveDirection == Vector3.right)
        {
            followTransform.rotation = Quaternion.Euler(new Vector3(30, -45, 0));
            targetDirection = new Vector3(30, 0, 0);
        }
        else if (moveDirection == Vector3.back || moveDirection == Vector3.forward)
        {
            followTransform.rotation = Quaternion.Euler(new Vector3(30, -45, 0));
            targetDirection = new Vector3(30, -90, 0);
        }
        LeanTween.rotate(followTransform.gameObject, targetDirection, 1.7f);
    }
    private IEnumerator Slash(Enemy enemy)
    {
        yield return new WaitForSeconds(0.1f);
        PrepareToKillEnemy();
        RotateCameraToKillEnemy();
        enemy.enemyEmotion.StopEmotion();
        int animPlay = Random.Range(0, 4);
        EnemyDeadType enemyDeadType = EnemyDeadType.None;
        Vector3 offSet;
        firstPosition = transform.position;
        animator.ResetTrigger("Slide");
        if (animPlay == 0)
        {
            offSet = 0f * moveDirection;
            transform.position -= offSet;
            enemyDeadType = EnemyDeadType.Strike;
            animator.SetTrigger("Jump Attack 1");
            yield return new WaitForSeconds(0.7f);
        }
        else if(animPlay == 1)
        {
            offSet = 0f * moveDirection;
            transform.position -= offSet;
            enemyDeadType = EnemyDeadType.Strike;
            animator.SetTrigger("Strike");
            yield return new WaitForSeconds(0.7f);

        }
        else if (animPlay == 2)
        {
            offSet = -0.24f * moveDirection;
            transform.position -= offSet;
            enemyDeadType = EnemyDeadType.Kick;
            animator.SetTrigger("Kick by Kick");
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            offSet = 0.55f * moveDirection;
            transform.position -= offSet;
            enemyDeadType = EnemyDeadType.Kick;
            animator.SetTrigger("Kick");
            yield return new WaitForSeconds(0.7f);
        }
        enemy.enemyDeadReaction.PlayDeadRect(EnemyDeadType.None);
        yield return new WaitForSeconds(0.4f);
        if (weapon == null)
            animator.SetTrigger("Idle");
        else {
            animator.SetTrigger("Sword Idle"); 
        }
        enemy.DecreaseHealth(enemyDeadType,transform.position, ReturnPlayerPosition);
    }
    Vector3 firstPosition;
    private void ReturnPlayerPosition()
    {
        transform.position = new Vector3(Mathf.RoundToInt(firstPosition.x), firstPosition.y, Mathf.RoundToInt(firstPosition.z));
    }
        
    void Attack(Enemy enemy)
    {
        StartCoroutine(Slash(enemy));
    }
    public void Idle()
    {
        animator.SetTrigger("Idle");
    }
    public void Slide()
    {
        animator.SetTrigger("Slide");
    }
    public void OnCollide(Transform collision)
    {
        LeanTween.cancel(gameObject);
        isMoving = false; 
        if (collision != null)
        {
            ObjectIndex objectIndex = collision.gameObject.GetComponent<ObjectIndex>();
            transform.position = new Vector3(objectIndex.row-moveDirectionV2.x, transform.position.y, objectIndex.column - moveDirectionV2.y);
        }
        else
        {
            transform.position = new Vector3(Mathf.RoundToInt(transform.position.x), transform.position.y, Mathf.RoundToInt(transform.position.z));
        }
    }
    void ArrowButtonClick(object data)
    {
        string direction = data.ToString();
        if (moveable)
        {
            switch (direction)
            {
                case "up":
                    {
                        MoveUp();
                        break;
                    }
                case "down":
                    {
                        MoveDown();
                        break;
                    }
                case "right":
                    {
                        MoveRight();
                        break;
                    }
                case "left":
                    {
                        MoveLeft();
                        break;
                    }
            }
        }
    }
    /*void Update()
    {
        if (!_gameBlockInput)
        {
            if (moveable)
            {
#if UNITY_ANDROID || UNITY_IOS
            Swipe();
#else
                if (Input.GetKeyDown(KeyCode.A))
                {
                    MoveLeft();
                }
                if (Input.GetKeyDown(KeyCode.D))
                {
                    MoveRight();
                }
                if (Input.GetKeyDown(KeyCode.S))
                {
                    MoveDown();
                }
                if (Input.GetKeyDown(KeyCode.W))
                {
                    MoveUp();
                }
#endif
            }
        }
    }*/
    private Vector2 startTouchPosition;
    private Vector2 currentPosition;
    private Vector2 endTouchPosition;
    private bool stopTouch = false;
    private float swipeRange = 50f;
    private float tapRange =10f;

    private void Swipe()
    {
        if(Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPosition = Input.GetTouch(0).position;
        }
        if(Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            currentPosition = Input.GetTouch(0).position;
            Vector2 distance = currentPosition - startTouchPosition;
            if (!stopTouch)
            {
                if (distance.x < -swipeRange)
                {
                    MoveLeft();
                    stopTouch = true;
                }
                else if (distance.x > swipeRange)
                {
                    MoveRight();
                    stopTouch = true;
                }
                else if (distance.y > swipeRange)
                {
                    MoveUp();
                    stopTouch = true;
                }
                else if (distance.y < -swipeRange)
                {
                    MoveDown();
                    stopTouch = true;
                }
            }
        }
        if(Input.touchCount>0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        {
            stopTouch = false;
            endTouchPosition = Input.GetTouch(0).position;
            Vector2 distance = endTouchPosition - startTouchPosition;
            if(Mathf.Abs(distance.x)<tapRange && Mathf.Abs(distance.y) < tapRange)
            {
            }
        }
    }
    private Vector3 moveDirection;
    public Vector2 moveDirectionV2;
    private void MoveLeft()
    {
        if (!isMoving)
        {
            isMoving = true;
            transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));
            moveDirection = Vector3.left;

            moveDirectionV2 = Vector2.left;
            Observer.Instance.Notify(ObserverKey.UserSwipe, moveDirectionV2);
        }
    }
    private void MoveRight()
    {
        if (!isMoving)
        {
            isMoving = true;
            transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));
            moveDirection = Vector3.right;

            moveDirectionV2 = Vector2.right;
            Observer.Instance.Notify(ObserverKey.UserSwipe, moveDirectionV2);
        }
    }
    private void MoveDown()
    {
        if (!isMoving)
        {
            isMoving = true;
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));
            moveDirection = Vector3.back;

            moveDirectionV2 = Vector2.down; 
            Observer.Instance.Notify(ObserverKey.UserSwipe, moveDirectionV2);
        }
    }
    private void MoveUp()
    {
        if (!isMoving)
        {
            isMoving = true;
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            moveDirection = Vector3.forward;

            moveDirectionV2 = Vector2.up; 
            Observer.Instance.Notify(ObserverKey.UserSwipe, moveDirectionV2);
        }
    }
    private Vector3 CamDirection()
    {
        Vector3 targetDirection = Vector3.zero;
        if (moveDirection == Vector3.left  || moveDirection == Vector3.right )
        {
            followTransform.rotation = Quaternion.Euler(new Vector3(30, -45, 0));
            targetDirection = new Vector3(30, 0, 0);
        }
        else if (moveDirection == Vector3.back||moveDirection == Vector3.forward)
        {
            followTransform.rotation = Quaternion.Euler(new Vector3(30, -45, 0));
            targetDirection = new Vector3(30, -90, 0);
        }
        return targetDirection;
    }
    public void VerticalFall(Vector2 index)
    {
        Vector3 targetDirection = new Vector3(40, -45, 0);
        StopAction();
        StopAllCoroutines();
        StartCoroutine(IEFall(targetDirection, index));
    }
    private IEnumerator IEFall(Vector3 targetDirection, Vector2 index)
    {
        transform.position = new Vector3(index.x,0,index.y) - moveDirection;
        moveable = false;
        virtualCam.SetActive(true);
        followTransform.rotation = Quaternion.Euler(targetDirection);
        Vector3 vectorFallStart = new Vector3(index.x, 0, index.y);
        Vector3 vectorFallFinish = transform.position + 30 * Vector3.down;
        LeanTween.move(gameObject, vectorFallStart, 0.5f);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Fall");
        yield return new WaitForSeconds(1f);
        TPRLSoundManager.Instance.PlaySoundFx("fall");
        LeanTween.move(gameObject, vectorFallFinish, 1.5f);
        yield return new WaitForSeconds(1.5f);
        Observer.Instance.Notify(ObserverKey.FallOut);
    }
    protected override IEnumerator IEFallIntoWater()
    {
        moveable = false;
        virtualCam.SetActive(true); 
        followTransform.rotation = Quaternion.Euler(CamDirection());
        Vector3 vectorFallStart = transform.position;
        LeanTween.move(gameObject, vectorFallStart, 0.5f);
        yield return new WaitForSeconds(1f);
        animator.SetTrigger("Fall Into Water");
        yield return new WaitForSeconds(1f);
        Observer.Instance.Notify(ObserverKey.FallOut);
    }

    protected override IEnumerator IEFire()
    {
        moveable = false;
        virtualCam.SetActive(true);
        followTransform.rotation = Quaternion.Euler(CamDirection());
        Vector3 vectorFallStart = transform.position;
        LeanTween.move(gameObject, vectorFallStart, 0.5f);
        yield return new WaitForSeconds(0.5f);
        animator.SetTrigger("Fall");
        yield return new WaitForSeconds(1f);
        float valueSet = 0f;
        while (valueSet < 1f)
        {
            valueSet += 0.05f;
            material.SetFloat("_SliceAmount", valueSet);
            yield return new WaitForSeconds(0.1f);
        };
        if (weapon != null)
        {
            weapon.transform.parent = null;
            weapon.layer = LayerMask.NameToLayer("NotCollideWithEntity");
            MeshCollider meshCollider = weapon.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            weapon.AddComponent<Rigidbody>();
        }
        yield return new WaitForSeconds(1f);
        OnFire.Invoke();
    }
    protected override IEnumerator IEBeMagicAttacked()
    {
        moveable = false;
        virtualCam.SetActive(true);
        followTransform.localRotation = Quaternion.Euler(CamDirection());
        animator.SetTrigger("Fall");
        yield return new WaitForSeconds(1f);
        float valueSet = 0f;
        while (valueSet < 1f)
        {
            valueSet += 0.05f;
            material.SetFloat("_SliceAmount", valueSet);
            yield return new WaitForSeconds(0.1f);
        };
        if (weapon != null)
        {
            weapon.transform.parent = null;
            weapon.layer = LayerMask.NameToLayer("NotCollideWithEntity");
            MeshCollider meshCollider = weapon.AddComponent<MeshCollider>();
            meshCollider.convex = true;
            weapon.AddComponent<Rigidbody>();
        }
        yield return new WaitForSeconds(1f);
        OnFire.Invoke();
    }
    public override void Booooooooooooooooooooooooooooooooom(ObjectIndex objectIndex)
    {
        StopAction();
        StopAllCoroutines();
        StartCoroutine(IEBooooooooooooooooooooooooooooooooom(objectIndex));
    }
    private IEnumerator IEBooooooooooooooooooooooooooooooooom(ObjectIndex objectIndex)
    {
        PrepareToDie();
        DestroyMesh(objectIndex);
        yield return new WaitForSeconds(1);
        Observer.Instance.Notify(ObserverKey.Boom);
    }
    private void PrepareToDie()
    {
        LeanTween.cancel(gameObject);
        isMoving = false;
        skinnedMeshRenderer.enabled = false;
        boxCollider.enabled = false;
    }
    private void DestroyMesh(ObjectIndex objectIndex)
    {
        meshDestroy.CutCascades = 5;
        meshDestroy.ExplodeForce = 200;
        Vector3 startPosition = new Vector3(objectIndex.row, 0, objectIndex.column);
        meshDestroy.DestroyMesh(startPosition);
    }
}
