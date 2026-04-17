using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public struct ControlPointData
{
    public bool locked;
    public string resistance;
    public string required;
}

public class ControlPointUIPanel : UIPanel
{
    [SerializeField] private UIDocument display;

    private Label _stateLabel;
    private Label _resistanceLabel;
    private Label _requireLabel;

    void Awake()
    {
        _stateLabel = display.rootVisualElement.Q<Label>("State");
        _resistanceLabel = display.rootVisualElement.Q<Label>("Resistance");
        _requireLabel = display.rootVisualElement.Q<Label>("Required");

        _stateLabel.text = "Locked";
        StyleAsError(_stateLabel);
        StyleAsError(_requireLabel);
    }

    public override void SetData<T>(T data)
    {
        if (data is ControlPointData controlPointData)
        {
            _resistanceLabel.text = controlPointData.resistance;
            _requireLabel.text = controlPointData.required;
            DisplayLockState(controlPointData.locked);
            Logger.LogUI(this, "Data loaded. ", LogType.SUCCESS);
            return;
        }
        Logger.LogUI(this, "Data could not be loaded. ", typeof(T) + " != ControlPointData");
    }

    private void DisplayLockState(bool locked)
    {
        if (locked)
        {
            _stateLabel.text = "Locked";
            StyleAsError(_stateLabel);
            StyleAsError(_requireLabel);
        }
        else
        {
            _stateLabel.text = "Unlocked";
            StyleAsSuccess(_stateLabel);
            StyleAsSuccess(_requireLabel);
        }
    }

    private void StyleAsSuccess(Label label)
    {
        label.RemoveFromClassList("label-error");
        label.AddToClassList("label-success");
    }

    private void StyleAsError(Label label)
    {
        label.RemoveFromClassList("label-success");
        label.AddToClassList("label-error");
    }
}