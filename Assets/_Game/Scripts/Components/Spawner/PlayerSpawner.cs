using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private int mapSizeWidth = 10;
    [SerializeField] private int mapSizeHeight = 10;
    [SerializeField] private ParticleSystem spawnEffect;
    public void SetMapSize(int map_size_width,int map_size_height)
    {
        mapSizeWidth = map_size_width;
        mapSizeHeight = map_size_height;
    }
    void Start()
    {
        AddDefault();
        SpawnPlayer();
    }
    private void SpawnPlayer()
    {
        GameObject player = null;
        if (UserDatas.user_Data.inventory_characters != null)
        {
            int length = UserDatas.user_Data.inventory_characters.Length;
            for (int i = 0; i < length; i++)
            {
                if (UserDatas.user_Data.inventory_characters[i].is_equiped)
                {
                    RecordItem recordItem = DataController.Instance.itemVO.GetDataByName<RecordItem>("Item", $"{UserDatas.user_Data.inventory_characters[i].id}");
                    GameObject prefab = PrefabsManager.Instance.GetAsset<GameObject>($"Players/{recordItem.prefab}");
                    if (prefab == null) return;
                    player = CreateManager.instance.CreateObject(prefab, Vector3.zero, transform);
                    break;
                }
            }
        }
        if (player == null) return;
        MoveComponent movement = player.GetComponent<MoveComponent>();
        movement.SetMapSize(mapSizeWidth, mapSizeHeight);
        movement.Spawn();
        spawnEffect.Play();
        Observer.Instance.Notify(ObserverKey.PlayerCreated, player);
    }
    private void AddDefault()
    {
        if (UserDatas.user_Data.inventory_characters == null|| UserDatas.user_Data.inventory_characters.Length ==0)
        {
            AddItemDefaultToInventory(InventoryItemType.character);
        }
        
        if (UserDatas.user_Data.inventory_weapons == null|| UserDatas.user_Data.inventory_weapons.Length ==0)
        {
            AddItemDefaultToInventory(InventoryItemType.weapon);
        }
    }
    private void AddItemDefaultToInventory(InventoryItemType type)
    {
        RecordItemEquip recordItemEquip = new RecordItemEquip();
        if (type == InventoryItemType.character)
        {
            recordItemEquip.id = "100";
        }
        else if (type == InventoryItemType.weapon)
        {
            recordItemEquip.id = "1000";
        }
        recordItemEquip.type = type;
        recordItemEquip.is_equiped = true;
        RecordItem recordItem = DataController.Instance.itemVO.GetDataByName<RecordItem>("Item", $"{recordItemEquip.id}");
        RecordItem[] recordItems = UserDatas.GetRecordItemInventoriesByType(type);
        if (!UserDatas.IsContainItem(recordItems, recordItemEquip.id))
        {
            UserDatas.AddItemToInventory(recordItem);
        }
        UserDatas.SetEquipItem(recordItemEquip);

    }
}
