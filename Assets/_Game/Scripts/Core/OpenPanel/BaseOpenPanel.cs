using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonCustom))]
public class BaseOpenPanel : MonoBehaviour
{
    // Start is called before the first frame update
    protected virtual void Start()
    {
        GetComponent<ButtonCustom>().onClick = () => { Open(); /*SoundController.instance.OnClickButton();*/  };
    }

    protected virtual void Open() {  }
}
