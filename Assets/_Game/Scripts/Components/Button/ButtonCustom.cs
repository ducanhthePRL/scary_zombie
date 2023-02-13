using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonCustom : MonoBehaviour
{
    private Button _bt;
    private Button bt {
        get {
            if (_bt == null)
                _bt = GetComponent<Button>();
            return _bt;
        }
    }
    public UnityAction onClick;
    [SerializeField]
    private LeanTweenType tweenType = LeanTweenType.notUsed;

    private void Awake()
    {
        if (bt != null)
            bt.onClick.AddListener(Click);
    }

    private void Click()
    {
        transform.LeanScale(new Vector3(1.05f, 1.05f, 1.05f), 0.1f).setEase(tweenType).setOnComplete(()=> { transform.LeanScale(Vector3.one, 0.1f); });
        onClick?.Invoke();
        TPRLSoundManager.Instance.PlaySoundFx("clickButton");
    }
    public void Interactable (bool status)
    {
        bt.interactable = status;
    }

}
