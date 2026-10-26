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
        UIStyleControler.StyleLabel(_stateLabel, LabelStyles.ERROR);
        UIStyleControler.StyleLabel(_requireLabel, LabelStyles.ERROR);
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
            UIStyleControler.StyleLabel(_stateLabel, LabelStyles.ERROR);
            UIStyleControler.StyleLabel(_requireLabel, LabelStyles.ERROR);
        }
        else
        {
            _stateLabel.text = "Odemčeny";
            UIStyleControler.StyleLabel(_stateLabel, LabelStyles.SUCCESS);
            UIStyleControler.StyleLabel(_requireLabel, LabelStyles.SUCCESS);
        }
    }
}