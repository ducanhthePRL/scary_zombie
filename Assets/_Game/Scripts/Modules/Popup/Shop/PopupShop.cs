using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[UIPanelPrefabAttr("Popup/Shop/PopupShop", "PriorityCanvas")]
public class PopupShop : BasePanel
{
    [SerializeField]
    private Transform itemParent;
    private bool isClickingBuy = false;
    [SerializeField]
    private ButtonCustom btClose;
    [SerializeField]
    private ItemTapInvetory[] itemTapShops;
    private ItemTapInvetory currentTapShop = null;
    private List<ItemPool> lstCurrentItems = new List<ItemPool>();
    ItemPoolManager _item_pool;
    [Header("Currency")]
    [SerializeField]
    private ItemShop item_removeAds;
    [SerializeField]
    private GameObject videoGold;
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
    protected override void Start()
    {
        base.Start();
        if (itemTapShops != null && itemTapShops.Length > 0)
        {
            int length = itemTapShops.Length;
            for (int i = 0; i < length; i++)
            {
                itemTapShops[i].onClick = (tap) => { ItemTapClick(tap); };
            }
            currentTapShop = itemTapShops[0];
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        if (itemTapShops != null && itemTapShops.Length > 0)
            ItemTapClick(itemTapShops[0]);
    }
    private void ItemTapClick(ItemTapInvetory tap)
    {
        if (currentTapShop != null)
        {
            if (currentTapShop == tap) return;
            currentTapShop.SetActiveHighlight(false);
        }
        currentTapShop = tap;
        currentTapShop.SetActiveHighlight(true);
        if (currentTapShop.gameObject.name.Equals("currency"))
        {
            ClearItems();
            SetActiveObject(videoGold, true);
            SetActiveObject(item_removeAds.gameObject, true);
            InstanceRemoveAds();
        }
        else
        {
            CreateItems();
        }
    }
    private void CreateItems()
    {
        ClearItems();
        RecordShopItem[] recordShopItems = DataController.Instance.ShopVO.GetDatasByName<RecordShopItem>("Shop");
        if (recordShopItems == null || recordShopItems.Length == 0) return;
        int length = recordShopItems.Length;
        for (int i = 0; i < length; i++)
        {
            RecordItem recordItem = DataController.Instance.itemVO.GetDataByName<RecordItem>("Item", $"{recordShopItems[i].id}");
            if (recordItem.type.Equals(currentTapShop.gameObject.name))
            {
                ItemPool item_manager = itemPool.GetInstance();
                item_manager.transform.SetParent(itemParent);
                item_manager.transform.localPosition = Vector3.zero;
                item_manager.transform.localScale = Vector3.one;
                item_manager.gameObject.SetActive(true);
                ItemShop itemShop = item_manager.GetComponent<ItemShop>();
                if (itemShop != null)
                {
                    itemShop.transform.localScale = Vector3.zero;
                    itemShop.Init(recordShopItems[i], ClickBuy);
                    lstCurrentItems.Add(item_manager);
                }
            }
        }
        StartCoroutine(IEShowItems());
    }
    private void InstanceRemoveAds()
    {
        RecordShopItem[] recordShopItems = DataController.Instance.ShopVO.GetDatasByName<RecordShopItem>("Shop");
        if (recordShopItems == null) return;
        int length = recordShopItems.Length;
        for (int i = 0; i < length; i++)
        {
            if (recordShopItems[i].id == 10)
            {
                item_removeAds.Init(recordShopItems[i], ClickBuy);
                break;
            }
        }

    }
    private void OnDestroy()
    {
        ClearItems();
    }

    private void ClearItems()
    {
        StopAllCoroutines();
        int length = lstCurrentItems.Count;
        for (int i = 0; i < length; i++)
        {
            if (lstCurrentItems[i] == null) continue;
            Destroy(lstCurrentItems[i].gameObject);
        }
        lstCurrentItems.Clear();
        SetActiveObject(videoGold, false);
        SetActiveObject(item_removeAds.gameObject, false);
    }

    protected override void Awake()
    {
        base.Awake();
        if (btClose != null)
            btClose.onClick = HidePanel;
    }

    private IEnumerator IEShowItems()
    {
        int length = lstCurrentItems.Count;
        for (int i = 0; i < length; i++)
        {
            LeanTween.scale(lstCurrentItems[i].gameObject, Vector3.one, 0.4f).setEase(LeanTweenType.easeSpring);
            yield return new WaitForSeconds(0.2f);
        }
    }
    private void ClickBuy(ItemShop item)
    {
        if (isClickingBuy) return;
        isClickingBuy = true;
        RecordShopItem record = item.recordItem;

        if (record.price_gold > 0)
        {
            if (UserDatas.user_Data.currency.gold < record.price_gold)
            {
                PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
                popUpNotice.OnSetTextOneButton("", "Not enough gold", null, "OK");
                isClickingBuy = false;
                return;
            }
            #region Firebase
            RecordItem recordItem = DataController.Instance.itemVO.GetDataByName<RecordItem>("Item", $"{record.id}");
            if (recordItem.type.Equals("character"))
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("id_character", recordItem.id);
                FirebaseManager.instance.LogEvent("character_unlock", hashtable);
            }
            else if (recordItem.type.Equals("weapon"))
            {
                Hashtable hashtable = new Hashtable();
                hashtable.Add("id_weapon", recordItem.id);
                FirebaseManager.instance.LogEvent("weapon_unlock", hashtable);
            }
            PopupSucessfulBuy popupSucessfulBuy = PanelManager.Show<PopupSucessfulBuy>();
            string content = "";
            InventoryItemType type = Ultis.ParseEnum<InventoryItemType>(recordItem.type);
            if (type == InventoryItemType.weapon)
            {
                content = "New Weapon!";
            }
            else if (type == InventoryItemType.character)
            {
                content = "New Character!";
            }
            else if (type == InventoryItemType.currency)
            {
                content = "New Currency!";
            }
            popupSucessfulBuy.Init(recordItem, "UNLOCKER", content);
            #endregion
            if (AddItemToInventory(record))
                UserDatas.UpdateGold(-record.price_gold);
            item.CheckBought();
            isClickingBuy = false;
            return;
        }
        if (record.price_money > 0.0f)
        {
            RecordItem recordItem = DataController.Instance.itemVO.GetDataByName<RecordItem>("Item", $"{record.id}");
            InventoryItemType type = Ultis.ParseEnum<InventoryItemType>(recordItem.type);
            RecordItem[] recordItems = UserDatas.GetRecordItemInventoriesByType(type);
            bool isContain = UserDatas.IsContainItem(recordItems, recordItem.id);
            if (isContain)
            {

                PopUpNotice popUpNotice = PanelManager.Show<PopUpNotice>();
                popUpNotice.OnSetTextOneButton("", "You already owned this item, buy another !", null, "OK");
                return;
            }
            IAPManager.instance.OnBuyDone = delegate { OnBuyItemSuccess(item, recordItem); };
            IAPManager.instance.OnBuyFail = OnBuyItemFailed;
            IAPManager.instance.BuyProductID(record.product_id, record.product_name, $"{record.price_money}", "USD");
        }
    }

    private void OnBuyItemSuccess(ItemShop item, RecordItem recordItem)
    {
        RecordShopItem record = item.recordItem;
        if (record.id == 10)
        {
            UserDatas.SetBoughtRemoveAds(true, true);
            FirebaseManager.instance.LogEvent("purchase_remove_ads");
        }
        else
            AddItemToInventory(record);
        item.CheckBought();
        isClickingBuy = false;
        PopupSucessfulBuy popupSucessfulBuy = PanelManager.Show<PopupSucessfulBuy>();
        string content = "";
        InventoryItemType type = Ultis.ParseEnum<InventoryItemType>(recordItem.type);
        if (type == InventoryItemType.weapon)
        {
            content = "New Weapon!";
        }
        else if (type == InventoryItemType.character)
        {
            content = "New Character!";
        }
        else if (type == InventoryItemType.currency)
        {
            content = "Block Ads!";
        }
        popupSucessfulBuy.Init(recordItem, "UNLOCKER", content);
    }

    private void OnBuyItemFailed()
    {
        isClickingBuy = false;
    }

    private bool AddItemToInventory(RecordShopItem record)
    {
        RecordItem recordItem = DataController.Instance.itemVO.GetDataByName<RecordItem>("Item", $"{record.id}");
        return UserDatas.AddItemToInventory(recordItem);
    }
    private void SetActiveObject(GameObject gob, bool status)
    {
        if (gob != null)
            gob.SetActive(status);
    }
}
