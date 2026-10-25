/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: ControlPointUIPanel
 */

using UnityEngine.UIElements;

public class ControlPointUIPanel : UIPanel
{
    private Label _stateLabel;
    private Label _resistanceLabel;
    private Label _requireLabel;

    void Awake()
    {
        _stateLabel = display.rootVisualElement.Q<Label>("State");
        _resistanceLabel = display.rootVisualElement.Q<Label>("Resistance");
        _requireLabel = display.rootVisualElement.Q<Label>("Required");

        _stateLabel.text = "Zmačeny";
        StyleAsError(_stateLabel);
        StyleAsError(_requireLabel);
    }

    public override bool SetData<T>(T data)
    {
        if (data is ValidationNodeData nodeData)
        {
            _resistanceLabel.text = NodeCalculationModel.FormatValue(nodeData.resistance, ElectricalQuantityType.RESISTANCE);
            _requireLabel.text = NodeCalculationModel.FormatValue(nodeData.expectedValue, nodeData.expectedValueType);
            DisplayLockState(nodeData.locked);
            Logger.LogUI(this, "Data loaded. ", LogType.SUCCESS);
            return true;
        }
        Logger.LogUI(this, "Data could not be loaded. ", typeof(T) + " != ControlPointData");
        return false;
    }

    private void DisplayLockState(bool locked)
    {
        if (locked)
        {
            _stateLabel.text = "Zmačeny";
            StyleAsError(_stateLabel);
            StyleAsError(_requireLabel);
        }
        else
        {
            _stateLabel.text = "Odemčeny";
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