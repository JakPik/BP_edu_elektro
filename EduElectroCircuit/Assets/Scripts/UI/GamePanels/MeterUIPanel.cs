using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public struct MeasurePointData
{
    public string name;
    public string signal;
    public string value;
}

public class MeterUIPanel : UIPanel
{
    [SerializeField] private UIDocument display;
    [SerializeField] private Node_MeasurePoint node;
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

    public override void SetData<T>(T data)
    {
        if (data is MeasurePointData measurePointData)
        {
            _meterName.text = measurePointData.name;
            _signalType.text = measurePointData.signal;
            _value.text = measurePointData.value;
            Logger.LogUI(this, "Data loaded. ", LogType.SUCCESS);
            return;
        }
        Logger.LogUI(this, "Data could not be loaded. ", typeof(T) + " != MeasurePointData");
    }
}