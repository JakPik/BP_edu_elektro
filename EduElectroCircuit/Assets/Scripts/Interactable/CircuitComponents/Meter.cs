using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Meter : CircuitComponent, IMeasureTool
{
    [SerializeField] MeterType meterType;
    [SerializeField] UIDocument display;
    private MultimeterSO multimeterData;

    public void Start()
    {
        multimeterData = new MultimeterSO
        {
            multimeterName = EnumTransformer.MeterTypeToString(meterType)
        };
        VisualElement meterPanel = display.rootVisualElement.Q<VisualElement>("MeterPanel");
        meterPanel.dataSource = multimeterData;
    }
    public void DisplayValues(float U, float I, float R, string mode)
    {
        multimeterData.type = mode;
        switch(meterType)
        {
            case MeterType.OHMMETER:
                multimeterData.value = NodeCalculationModel.FormatValue(R) + "Ω";
                break;
            case MeterType.VOLTMETER:
                multimeterData.value = NodeCalculationModel.FormatValue(U) + "V";
                break;
            case MeterType.AMPMETER:
                multimeterData.value = NodeCalculationModel.FormatValue(I) + "A";
                break;
        }
    }

    protected override void CanAnimate()
    {
        throw new System.NotImplementedException();
    }

    protected override Quaternion FindTargetRotation(Transform local, Transform target)
    {
        throw new System.NotImplementedException();
    }

    protected override void InteractionLock(CircuitActiveStateEvent @event)
    {
        var locked = !@event.CircuitActive;
    }

    protected override void NodeLockedState(bool locked)
    {
        throw new System.NotImplementedException();
    }

    protected override void SendData()
    {
        throw new System.NotImplementedException();
    }
}
