using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerFake : MonoBehaviour
{
    private Dictionary<string, GameObject> dictCurrentItemEquiped = new Dictionary<string, GameObject>();
    [SerializeField]
    private Transform weaponTransform;
    private Animator _animator;
    public Animator animator
    {
        get
        {
            if (_animator == null) _animator = GetComponent<Animator>();
            return _animator;
        }
        set
        {
            _animator = value;
        }
    }
    public UnityAction actionAppear;

    private void OnEnable()
    {
        if (UserDatas.user_Data.inventory_weapons != null)
        {
            foreach (RecordItem recordItem in UserDatas.user_Data.inventory_weapons)
            {
                if (recordItem.is_equiped)
                {
                    EquipItem(recordItem);
                }
            }
        }
    }
    private void AppearAnim()
    {
        actionAppear?.Invoke();
    }

    public void CheckEquipItem(RecordItemEquip record)
    {
        RecordItem[] recordItems = UserDatas.GetRecordItemInventoriesByType(record.type);
        if (recordItems == null) return;
        int length = recordItems.Length;
        for (int i = 0; i < length; i++)
        {
            if (record.id == recordItems[i].id)
            {
                RecordItem recordItem = new RecordItem();
                recordItem = recordItems[i];
                if (record.is_equiped == false)
                {
                    UnEquipItem(recordItem);
                    return;
                }
                EquipItem(recordItem);
                break;
            }
        }
    }
    private void EquipItem(RecordItem record)
    {
        GameObject item = null;
        if (record.type.Equals("weapon"))
        {
             item = PrefabsManager.Instance.GetAsset<GameObject>($"Weapon/{record.name}");
        }
        if (item == null) return;
        GameObject weapon = CreateManager.instance.CreateObject(item,Vector3.zero,weaponTransform);
        if (weapon == null) return;
        if (!dictCurrentItemEquiped.ContainsKey(record.type))
        {
            dictCurrentItemEquiped.Add(record.type, weapon);
        }
    }
    private void UnEquipItem(RecordItem record)
    {
        if (dictCurrentItemEquiped.ContainsKey(record.type))
        {
            GameObject remove = dictCurrentItemEquiped[record.type];
            if (remove != null)
            {
                Destroy(remove);
            }
            dictCurrentItemEquiped.Remove(record.type);
        }
    }
}
