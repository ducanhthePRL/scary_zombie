using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerFakeManager : MonoBehaviour
{
    [SerializeField]
    private Transform playerParent;
    private PlayerFake _playerFake;
    public PlayerFake playerFake
    {
        get
        {
            if (_playerFake == null) SpawnPlayerFake();
            return _playerFake;
        }
        set
        {
            _playerFake = value;
        }
    }   
    [SerializeField]
    private ParticleSystem ptcAppear, ptcDisappear;
    public UnityAction actionAppear=null;
    public UnityAction actionRotation = null;
    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.EquipItem, CheckEquip);
    }
    private void OnDestroy()
    {

        Observer.Instance.RemoveObserver(ObserverKey.EquipItem, CheckEquip);
    }
    public void PlayerDisappearAnim()
    {
        if ( ptcAppear == null || ptcDisappear == null) return;
        int level = UserDatas.user_Data.current_progress.level;
        playerFake.actionAppear = PlayerAppearAnim;
        if (UserDatas.user_Data.current_progress.previous_level != level)
        {
            playerFake.animator.Play("DisappearFakePlayer", 0);
            ptcAppear.gameObject.SetActive(true);
            ptcDisappear.Play();
            UserDatas.user_Data.current_progress.previous_level = level;
        }
    }
    public void PlayerAppearAnim()
    {
        actionAppear?.Invoke();
        playerFake.animator.Play("AppearFakePlayer", 0);
        if (ptcAppear != null)
        {
            ptcAppear.Play();
        }
    }
    private void CheckEquip(object data)
    {
        if (data == null) return;
        RecordItemEquip record = (RecordItemEquip)data;
       if(record.type== InventoryItemType.character)
            ChangePlayerFake();
        else
        {
            if(playerFake!=null)
                 playerFake.CheckEquipItem(record);
        }
    }

    private void ChangePlayerFake()
    {
        if (playerFake != null)
            Destroy(playerFake.gameObject);
        SpawnPlayerFake();
    }
    private void SpawnPlayerFake()
    {
        if (UserDatas.user_Data.inventory_characters != null)
        {
            int length = UserDatas.user_Data.inventory_characters.Length;
            for (int i = 0; i < length; i++)
            {
                if (UserDatas.user_Data.inventory_characters[i].is_equiped)
                {
                    RecordItem recordItem = DataController.Instance.itemVO.GetDataByName<RecordItem>("PlayerFake", $"{UserDatas.user_Data.inventory_characters[i].id}");
                    GameObject prefab = PrefabsManager.Instance.GetAsset<GameObject>($"Players/{recordItem.prefab}");
                    if (prefab == null) return;
                    _playerFake = CreateManager.instance.CreateObjectGetComponent<PlayerFake>(prefab, Vector3.zero, playerParent);
                    break;
                }
            }
        }
        if (_playerFake == null) return;
        _playerFake.transform.localScale = Vector3.one;
        actionRotation?.Invoke();
    }
}
