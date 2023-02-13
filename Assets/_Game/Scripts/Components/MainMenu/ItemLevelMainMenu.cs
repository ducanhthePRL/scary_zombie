using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ItemLevelMainMenu : MonoBehaviour
{
    [SerializeField]
    private Image icon;
    [SerializeField]
    private GameObject obLock;
    public Transform[] levelTransforms;
    [SerializeField] ParticleSystem ptcAppear,ptcDisappear;
    private int level = 0;
    private void Awake()
    {
        level = UserDatas.user_Data.current_progress.level - 1;
    }

    private void SetText(TextMeshProUGUI text, string content)
    {
        if (text != null)
            text.text = content;
    }

    private void SetImage(Image img, string path)
    {
        if (img != null)
            img.sprite = TexturesManager.Instance.GetSprites(path); ;
    }

    public void SetLock(bool is_lock)
    {
        if (obLock != null)
            obLock.SetActive(is_lock);
    }
   
}
