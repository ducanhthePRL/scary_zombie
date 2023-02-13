using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Unity.VisualScripting;

public class ItemVideoGold : MonoBehaviour
{
    public ButtonCustom btWatch;
    [SerializeField]
    private Image[] images;
    [SerializeField]
    private TMP_Text txCoinVideo,txWatch;
    public UnityAction actionClick = null;
    public int coinAdd = 50;
    private Color colorInteractable = new Color(0.7843137f,0.7843137f,0.7843137f,0.5f);
    private Color colorDefault = new Color(1, 1, 1, 1);
    private Color colorTxGold = new Color(1, 1, 1, 1);
    private void Start()
    {
        BtOnClick(btWatch, actionClick);
        SetTextCoin(txCoinVideo, coinAdd);
    }
    private void BtOnClick(ButtonCustom bt, UnityAction action)
    {
        if (bt != null)
        {
            bt.onClick = action;
        }
    }
    private void SetTextCoin(TMP_Text tx, int gold)
    {
        if (tx != null)
            tx.text = gold.ToString();
    }
    public void Interactable(bool status)
    {
        if (btWatch != null)
            btWatch.Interactable(status);
        int length = images.Length;
        for (int i = 0; i < length; i++)
        {
            SetInteractableImage(images[i], status);
        }
        if (status)
        {
            if(txCoinVideo!= null)
            {
                txCoinVideo.color = colorTxGold;
            }
            if (txWatch != null)
            {
                txWatch.color = colorDefault;
            }
        }
        else
        {
            SetInteractableImage(txCoinVideo);
            SetInteractableImage(txWatch);
        }

    }
    private void SetInteractableImage(Image im,bool status)
    {
        if (im == null) return;
        if (status)
        {
            im.color = colorDefault;
        }
        else
        {
            im.color = colorInteractable;
        }
    }
    private void SetInteractableImage(TMP_Text tx)
    {
        if (tx != null) 
        tx.color = colorInteractable;
    }
}
