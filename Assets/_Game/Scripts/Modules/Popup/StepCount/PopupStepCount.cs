using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[UIPanelPrefabAttr("Popup/StepCount/PopupStepCount", "Popup")]
public class PopupStepCount : BasePanel
{
    [SerializeField] private TMP_Text remainingStep;
    
    private int maxStep;
    private int currentStep;

    public bool isOutOfStep = false;
    protected override void Awake()
    {
        base.Awake();
    }
    protected override void OnEnable()
    {
        if (main != null)
        {
            main.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            main.LeanScale(Vector3.one, 0.4f).setEase(tweenType);
        }
        Observer.Instance.Notify(ObserverKey.PopupShowed, true);
        Observer.Instance.AddObserver(ObserverKey.PlayerMove, DecreaseStep);
    }
    protected override void Start()
    {
        base.Start();
    }
    private void OnDisable()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PlayerMove, DecreaseStep);
    }
    private void OnDestroy()
    {
        Observer.Instance.RemoveObserver(ObserverKey.PlayerMove, DecreaseStep);
    }
    public void Init(int step)
    {
        maxStep = step;
        currentStep = step;
        isOutOfStep = false;
        remainingStep.text = $"{currentStep}";
    }
    public void DecreaseStep(object data)
    {
        currentStep--;
        remainingStep.text = $"{currentStep}";
        if(currentStep <= 0)
        {
            isOutOfStep = true;
            Observer.Instance.Notify(ObserverKey.OutOfStep, null);
        }
    }
}
