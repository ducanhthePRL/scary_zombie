using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class UserDatas
{
    public static RecordUserData user_Data;
    public static SceneType current_scene_type = SceneType.Splash;
    private static ObscuredBool _isEnableCheatMode = false;
    public static ObscuredBool isEnableCheatMode
    {
        get
        {
            return _isEnableCheatMode;
        }
        set
        {
            _isEnableCheatMode = value;
            Observer.Instance.Notify(ObserverKey.EnableCheatMode, (bool)value);
        }
    }


    private static bool _isBoughtRemoveAds = false;
    public static bool isBoughtRemoveAds => _isBoughtRemoveAds;
    public static void SetBoughtRemoveAds(bool is_bought, bool check_show_ads)
    {
        _isBoughtRemoveAds = is_bought;
        if (check_show_ads)
            AdsManager.instance.ShowBanner(!_isBoughtRemoveAds);
        RecordItem recordItem = DataController.Instance.itemVO.GetDataByName<RecordItem>("Item", "10");
        if (_isBoughtRemoveAds)
        {
            AddItemToInventory(recordItem);
        }
        else
            RemoveItemFromInventory(recordItem);
    }

    public static void UpdateGold(int gold = 0)
    {
        user_Data.currency.gold += gold;
        Observer.Instance.Notify(ObserverKey.UpdateGold);
        Save();
    }

    public static RecordItem[] GetRecordItemInventoriesByType(InventoryItemType type)
    {
        RecordItem[] _recordItemInventories = null;
        switch (type)
        {
            case InventoryItemType.character:
                _recordItemInventories = user_Data.inventory_characters;
                break;
            case InventoryItemType.weapon:
                _recordItemInventories = user_Data.inventory_weapons;
                break;
            case InventoryItemType.consumable:
                _recordItemInventories = user_Data.inventory_consumables;
                break;
            case InventoryItemType.nonconsumable:
                _recordItemInventories = user_Data.inventory_nonconsumables;
                break;
            default:
                break;
        }
        return _recordItemInventories;
    }

    public static void SetEquipItem(RecordItemEquip record)
    {
        RecordItem[] _recordItemInventories = GetRecordItemInventoriesByType(record.type);
        if (_recordItemInventories == null || _recordItemInventories.Length == 0) return;
        int length = _recordItemInventories.Length;
        for (int i = 0; i < length; i++)
        {
            if (!_recordItemInventories[i].id.Equals(record.id)) continue;
            _recordItemInventories[i].is_equiped = record.is_equiped;
            break;
        }
        SetRecordItemInventories(record.type, _recordItemInventories);
        Save();
    }

    public static void SetRecordItemInventories(InventoryItemType type, RecordItem[] recordItemInventories)
    {
        switch (type)
        {
            case InventoryItemType.character:
                user_Data.inventory_characters = recordItemInventories;
                break;
            case InventoryItemType.weapon:
                user_Data.inventory_weapons = recordItemInventories;
                break;
            case InventoryItemType.consumable:
                user_Data.inventory_consumables = recordItemInventories;
                break;
            case InventoryItemType.nonconsumable:
                user_Data.inventory_nonconsumables = recordItemInventories;
                break;
            default:
                break;
        }
    }

    public static bool AddItemToInventory(RecordItem record)
    {
        InventoryItemType type = Ultis.ParseEnum<InventoryItemType>(record.type);
        RecordItem[] recordItems = GetRecordItemInventoriesByType(type);
        //bool isContain = IsContainItem(recordItems, record.id);
        //if (isContain)
        //{

        //    PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
        //    popUpNotice.OnSetTextOneButton("", "You already owned this item, buy another !", null, "OK");
        //    return false;
        //}
        List<RecordItem> lst = new List<RecordItem>();
        if (recordItems != null)
            lst.AddRange(recordItems);
        lst.Add(record);
        SetRecordItemInventories(type, lst.ToArray());
        Save();
        return true;
    }

    public static bool RemoveItemFromInventory(RecordItem record)
    {
        InventoryItemType type = Ultis.ParseEnum<InventoryItemType>(record.type);
        RecordItem[] recordItems = GetRecordItemInventoriesByType(type);
        List<RecordItem> lst = new List<RecordItem>();
        if (recordItems != null)
            lst.AddRange(recordItems);
        bool is_remove_success = false;
        int length = lst.Count;
        for (int i = 0; i < length; i++)
        {
            if (lst[i].id.Equals(record.id))
            {
                lst.RemoveAt(i);
                is_remove_success = true;
                break;
            }
        }

        SetRecordItemInventories(type, lst.ToArray());
        Save();
        return is_remove_success;
    }


    public static bool IsContainItem(RecordItem[] records, string id)
    {
        if (records == null || records.Length == 0) return false;
        int length = records.Length;
        for (int i = 0; i < length; i++)
        {
            if (records[i].id.Equals(id)) return true;
        }
        return false;
    }

    public static void Save()
    {
        string data = JsonUtility.ToJson(user_Data);
        ObscuredPrefs.Set<string>("user_data", data);
        ObscuredPrefs.Save();
        string path = $"{Application.persistentDataPath}/DataSaved.txt";
        File.WriteAllText(path, data);
    }

    public static void Load()
    {
        string data = "";
        string path = $"{Application.persistentDataPath}/DataSaved.txt";
        if (File.Exists(path))
            data = File.ReadAllText(path);
        if (string.IsNullOrEmpty(data))
            data = ObscuredPrefs.Get<string>("user_data", "");
        if (!string.IsNullOrEmpty(data))
            user_Data = JsonUtility.FromJson<RecordUserData>(data);
        //user_Data.currency.gold = 10000000;
        //user_Data.current_progress.previous_level = 29;
        //user_Data.current_progress.level = 30;
    }
}