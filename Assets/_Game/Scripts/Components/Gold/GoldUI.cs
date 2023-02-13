using TMPro;
using UnityEngine;

public class GoldUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI txGold;
    [SerializeField]
    private ButtonCustom btAddGold;
    private bool isClickedAddGold = false;

    private void Awake()
    {
        Observer.Instance.AddObserver(ObserverKey.UpdateGold, UpdateGold);
        if (btAddGold != null)
            btAddGold.onClick = ClickAddGold;
    }

    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.UpdateGold, UpdateGold);
    }

    private void Start()
    {
        UpdateGold();
    }

    private void UpdateGold(object data = null)
    {
        if (txGold != null)
            txGold.text = $"{UserDatas.user_Data.currency.gold}";
    }

    private void ClickAddGold()
    {
        if (isClickedAddGold && AdsManager.instance.isShowingAd)
            isClickedAddGold = false;
        if (isClickedAddGold) return;
        isClickedAddGold = true;
        AdsManager.instance.ShowReward(delegate { UserDatas.UpdateGold(50); isClickedAddGold = false; }, delegate { isClickedAddGold = false; });
    }
}
