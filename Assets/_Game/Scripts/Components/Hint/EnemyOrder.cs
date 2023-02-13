using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnemyOrder : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer; 
    [SerializeField] private TMP_Text orderText;
    public void SetOrder(int order)
    {
        orderText.text = $"{order}";
    }
    private int alpha = 255;
    private bool increasing;
    private void Update()
    {
        if (alpha <= 25)
        {
            increasing = true;
        }
        else if (alpha >= 255)
        {
            increasing = false;
        }
        alpha += increasing ? 1 : -1;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha/255f);
        transform.rotation = Camera.main.transform.rotation;
    }
}
