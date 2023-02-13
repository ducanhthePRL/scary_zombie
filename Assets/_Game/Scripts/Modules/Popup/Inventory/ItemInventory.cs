using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemInventory : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private GameObject obSelected, obEquiped;
    [SerializeField]
    private ButtonCustom obUse;
    [SerializeField]
    private ButtonCustom btClickIcon;
    private UnityAction<ItemInventory> onClick;
    private RecordItem _recordItem = default;
    public RecordItem recordItem => _recordItem;
    private InventoryItemType _inventoryItemType;
    public InventoryItemType inventoryItemType => _inventoryItemType;
    private bool isNotConsumable = true;
    public UnityAction onEquipItem;

    private void Awake()
    {
        SetBtEvents(btClickIcon, Click);
        SetBtEvents(obUse, ClickUse);
        //SetBtEvents(obUnuse, ClickUnuse);
    }

    private void SetBtEvents(ButtonCustom bt, UnityAction action)
    {
        if (bt != null)
            bt.onClick = action;
    }

    public void Init(RecordItem record_item, UnityAction<ItemInventory> on_click)
    {
        _recordItem = record_item;
        SetEquiped(record_item.is_equiped);
        SetSelected(false);
        SetActiveUseWhenSelected(false, true);
        LoadIcon(record_item.icon);
        onClick = on_click;
        _inventoryItemType = Ultis.ParseEnum<InventoryItemType>(_recordItem.type);
    }

    private void LoadIcon(string icon_name)
    {
        if (icon != null)
        {
            icon.sprite = TexturesManager.Instance.GetSprites(icon_name);
            icon.color = new Color(1, 1, 1, 1);
            icon.preserveAspect = true;
        }
    }

    public void SetEquiped(bool is_equip)
    {
        _recordItem.is_equiped = is_equip;
        if (obEquiped != null)
            obEquiped.SetActive(is_equip);
    }

    public void SetSelected(bool is_selected)
    {
        if (obSelected != null)
            obSelected.SetActive(is_selected);
    }

    private void Click()
    {
        onClick?.Invoke(this);
    }

    private void ClickUse()
    {
        if (isNotConsumable)
        {
            onEquipItem?.Invoke();
            SetEquiped(true);
            RecordItemEquip recordItem = new RecordItemEquip();
            recordItem.id = _recordItem.id;
            recordItem.type = _inventoryItemType;
            recordItem.is_equiped = true;
            SetActiveUseWhenSelected(false);
            UserDatas.SetEquipItem(recordItem);
            Observer.Instance.Notify(ObserverKey.EquipItem, recordItem);
        }
        else
        {

        }
    }

    //private void ClickUnuse()
    //{
    //    SetEquiped(false);
    //    RecordItemEquip recordItem = new RecordItemEquip();
    //    recordItem.id = _recordItem.id;
    //    recordItem.type = _inventoryItemType;
    //    recordItem.is_equiped = false;
    //    SetActiveUseWhenSelected(true);
    //    UserDatas.SetEquipItem(recordItem);
    //    Observer.Instance.Notify(ObserverKey.EquipItem, recordItem);
    //}

    private void ClickBuy()
    {

    }

    public void SetActiveUseWhenSelected(bool active, bool is_force = false)
    {
        if (obUse != null)
            obUse.gameObject.SetActive(active);

        //if (obUnuse != null)
        //{
        //    obUnuse.gameObject.SetActive(is_force ? active : isNotConsumable ? !active : false);
        //}
    }
}
