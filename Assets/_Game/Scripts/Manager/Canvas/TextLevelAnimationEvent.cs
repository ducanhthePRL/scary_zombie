using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextLevelAnimationEvent : MonoBehaviour
{
    [SerializeField] private GameObject activeAfter;
    private void Start()
    {
        activeAfter.SetActive(false);
    }
    public void SetActiveAfter()
    {
        if(activeAfter != null)
            activeAfter.SetActive(true);
    }
}
