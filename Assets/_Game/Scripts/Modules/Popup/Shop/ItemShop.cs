using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemShop : MonoBehaviour
{
    [SerializeField]
    private Image iconItem;
    [SerializeField]
    private ButtonCustom btBuy;
    [SerializeField]
    private TextMeshProUGUI txDescription, txPriceGold, txPriceMoney;
    [SerializeField]
    private GameObject obPriceGold, obPriceMoney;
    [SerializeField]
    private GameObject obBought;
    private RecordShopItem _recordItem;
    public RecordShopItem recordItem => _recordItem;
    private UnityAction<ItemShop> onClickBuy = null;

    private void Awake()
    {
        if (btBuy != null)
            btBuy.onClick = ClickBuy;
    }

    public void Init(RecordShopItem record, UnityAction<ItemShop> on_click_buy)
    {
        onClickBuy = on_click_buy;
        _recordItem = record;
        RecordItem item = DataController.Instance.itemVO.GetDataByName<RecordItem>("Item", $"{record.id}");
        SetImage(iconItem, item.icon);
        SetText(txDescription, item.name);
        //if (item.effect_values != null && item.effect_values.Length > 0)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    int effect_length = item.effect_values.Length;
        //    for (int i = 0; i < effect_length; i++)
        //    {
        //        if (i > 0 && i < effect_length - 1)
        //            sb.Append("\n");
        //        RecordEffect effect = DataController.Instance.effectVO.GetDataByName<RecordEffect>("Effect", item.effect_values[i].id);
        //        sb.Append(string.Format(effect.description, item.effect_values[i].value));
        //    }
        //    SetText(txDescription, sb.ToString());
        //}

        CheckBought();
        if (record.price_gold > 0)
        {
            SetActive(obPriceGold, true);
            SetActive(obPriceMoney, false);
            SetText(txPriceGold, $"{record.price_gold}");
        }
        else if (record.price_money > 0.0f)
        {
            SetActive(obPriceGold, false);
            SetActive(obPriceMoney, true);
            SetText(txPriceMoney, $"$ {record.price_money}");
        }
    }

    public void CheckBought()
    {
        bool is_bought = false;
        if(_recordItem.price_money > 0.0f)
            is_bought = IAPManager.instance.CheckBought(_recordItem.product_id) && _recordItem.iap_type.ToLower().Equals(IAPItemType.nonconsumable.ToString());
        RecordItem recordItem = DataController.Instance.itemVO.GetDataByName<RecordItem>("Item", $"{_recordItem.id}");
        InventoryItemType type = Ultis.ParseEnum<InventoryItemType>(recordItem.type);
        RecordItem[] recordItems = UserDatas.GetRecordItemInventoriesByType(type);
        bool is_contain = false;
        if (_recordItem.price_gold > 0)
            is_contain = UserDatas.IsContainItem(recordItems, recordItem.id) && _recordItem.iap_type.ToLower().Equals(IAPItemType.nonconsumable.ToString());
        SetActive(obBought, is_bought || is_contain);
        SetActive(btBuy.gameObject, is_bought == false && is_contain == false);
    }

    private void SetActive(GameObject ob, bool active)
    {
        if (ob != null)
            ob.SetActive(active);
    }

    private void SetImage(Image img, string icon)
    {
        if (img != null)
        {
            img.sprite = TexturesManager.Instance.GetSprites(icon);
            img.color = new Color(1, 1, 1, 1);
            img.preserveAspect = true;
        }
    }

    private void SetText(TextMeshProUGUI text, string content)
    {
        if (text != null)
            text.text = content;
    }

    private void ClickBuy()
    {
        onClickBuy?.Invoke(this);
    }
}
