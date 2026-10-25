/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: MeterUIPanel
 */

using UnityEngine;
using UnityEngine.UIElements;



public class MeterUIPanel : UIPanel
{
    [SerializeField] private Node_Measurement node;
    //  private IDataUIProvider meter;
    private Label _meterName;
    private Label _signalType;
    private Label _value;

    void Awake()
    {
        _meterName = display.rootVisualElement.Q<Label>("MeterName");
        _signalType = display.rootVisualElement.Q<Label>("SignalType");
        _value = display.rootVisualElement.Q<Label>("Value");
    }

    public override bool SetData<T>(T data)
    {
        if (data is MeasurePointData measurePointData)
        {
            _meterName.text = measurePointData.name;
            _signalType.text = measurePointData.signal;
            _value.text = measurePointData.value;
            Logger.LogUI(this, "Data loaded. ", LogType.SUCCESS);
            return true;
        }
        Logger.LogUI(this, "Data could not be loaded. ", typeof(T) + " != MeasurePointData");
        return false;
    }
}