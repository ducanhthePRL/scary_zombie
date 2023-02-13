using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemTapInvetory : MonoBehaviour
{
    [SerializeField]
    private GameObject obSelected;
    [SerializeField]
    private ButtonCustom btTap;
    public UnityAction<ItemTapInvetory> onClick;
    private RecordItem[] _recordItemInventories = null;
    public RecordItem[] recordItemInventories {
        get {
            InventoryItemType item_type = Ultis.ParseEnum<InventoryItemType>(gameObject.name);
            _recordItemInventories = UserDatas.GetRecordItemInventoriesByType(item_type);
            return _recordItemInventories;
        }
        set
        {
            _recordItemInventories = value;
        }
    }

    private void Awake()
    {
        if (btTap != null)
            btTap.onClick = Click;
    }

    public void SetActiveHighlight(bool active)
    {
        if (obSelected != null)
            obSelected.SetActive(active);
    }

    private void Click()
    {
        onClick?.Invoke(this);
    }
}
