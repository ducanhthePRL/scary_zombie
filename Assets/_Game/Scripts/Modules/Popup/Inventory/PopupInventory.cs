using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UIPanelPrefabAttr("Popup/Inventory/PopupInventory", "PriorityCanvas")]
public class PopupInventory : BasePanel
{
    [SerializeField]
    private Transform parentItem;
    [SerializeField]
    private ItemTapInvetory[] itemTapInvetories;
    private ItemTapInvetory currentTapInventory = null;
    private ItemInventory currentItemInventory = null;
    private ItemInventory previousItemInventory = null;
    private List<ItemPool> lstCurrentItems = new List<ItemPool>();
    [SerializeField]
    private ButtonCustom btClose;

    ItemPoolManager _item_pool;
    protected ItemPoolManager itemPool
    {
        get
        {
            if (_item_pool == null)
            {
                try
                {
                    if (!TryGetComponent<ItemPoolManager>(out _item_pool))
                    {
                        _item_pool = gameObject.AddComponent<ItemPoolManager>();
                    }
                }
                catch (System.Exception ex) { }
            }
            return _item_pool;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        if (btClose != null)
            btClose.onClick = HidePanel;
    }

    private void OnDestroy()
    {
        ClearItems();
    }

    protected override void Start()
    {
        base.Start();
        if (itemTapInvetories != null && itemTapInvetories.Length > 0)
        {
            int length = itemTapInvetories.Length;
            for (int i = 0; i < length; i++)
            {
                itemTapInvetories[i].onClick = (tap) => { ItemTapClick(tap, false); };
            }
            currentTapInventory = itemTapInvetories[0];
        }
    }

    private void CreateItems()
    {
        StopAllCoroutines();
        ClearItems();
        RecordItem[] _recordItemInventories = currentTapInventory.recordItemInventories;
        if (_recordItemInventories != null && _recordItemInventories.Length > 0)
        {
            int item_length = _recordItemInventories.Length;
            for (int i = 0; i < item_length; i++)
            {
                ItemPool item_manager = itemPool.GetInstance();
                item_manager.transform.SetParent(parentItem);
                item_manager.transform.localPosition = Vector3.zero;
                item_manager.transform.localScale = Vector3.one;
                item_manager.gameObject.SetActive(true);
                ItemInventory itemInventory = item_manager.GetComponent<ItemInventory>();
                itemInventory.onEquipItem = EquipItem;
                RecordItem record_item = _recordItemInventories[i];
                if (record_item.is_equiped)
                    previousItemInventory = itemInventory;
                itemInventory.Init(record_item, ClickItemInventory);
                lstCurrentItems.Add(item_manager);
            }
        }
    }


    protected override void OnEnable()
    {
        base.OnEnable();
        if (itemTapInvetories != null && itemTapInvetories.Length > 0)
            ItemTapClick(itemTapInvetories[0], true);
    }

    private void ItemTapClick(ItemTapInvetory tap, bool is_force)
    {
        if (currentTapInventory != null)
        {
            if (currentTapInventory == tap && is_force == false) return;
            currentTapInventory.SetActiveHighlight(false);
        }
        currentTapInventory = tap;
        currentTapInventory.SetActiveHighlight(true);
        currentItemInventory = null;
        CreateItems();
    }

    private void EquipItem()
    {
        if (previousItemInventory != null)
        {
            if (previousItemInventory == currentItemInventory) return;
            previousItemInventory.SetEquiped(false);
            RecordItem record = previousItemInventory.recordItem;
            RecordItemEquip recordItem = new RecordItemEquip();
            recordItem.id = record.id;
            recordItem.type = previousItemInventory.inventoryItemType;
            recordItem.is_equiped = false;
            UserDatas.SetEquipItem(recordItem);
            Observer.Instance.Notify(ObserverKey.EquipItem, recordItem);
        }
        previousItemInventory = currentItemInventory;
    }

    private void ClearItems()
    {
        if (lstCurrentItems.Count == 0) return;
        int length = lstCurrentItems.Count;
        for (int i = 0; i < length; i++)
        {
            itemPool.ReturnToPool(lstCurrentItems[i]);
        }
        lstCurrentItems.Clear();
    }

    private void ClickItemInventory(ItemInventory item)
    {
        if (currentItemInventory != null)
        {
            //if (currentItemInventory == item) return;
            currentItemInventory.SetSelected(false);
            currentItemInventory.SetActiveUseWhenSelected(false, true);
        }
        currentItemInventory = item;
        currentItemInventory.SetSelected(true);
        currentItemInventory.SetActiveUseWhenSelected(!currentItemInventory.recordItem.is_equiped);
    }
}
