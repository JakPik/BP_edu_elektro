using UnityEngine;
using System;
using UnityEngine.UIElements;

public class Node_MeasurePoint : Node
{
    [SerializeField] MeterType meterType;
    [SerializeField] UIDocument display;
    private MultimeterSO multimeterData;

    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveStateEventChannel;

    void Awake()
    {
        multimeterData = new MultimeterSO
        {
            multimeterName = EnumTransformer.MeterTypeToString(meterType)
        };
        VisualElement meterPanel = display.rootVisualElement.Q<VisualElement>("MeterPanel");
        meterPanel.dataSource = multimeterData;
    }

    public override void BuildConections(Node branchInRef, int branchId)
    {
        if(nextNode == null && branchInRef == null)
        {
            Logger.Log(this.name, "Line construction done. No nodes to connect to.", LogType.SUCCESS);
            return;
        }
        try {
            if(nextNode != null) {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), nextNode.GetInPortPosition(0));
                nextNode.BuildConections(branchInRef, branchId);
            }
            else
            {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), branchInRef.GetInPortPosition(branchId));
            }
        }
        catch (Exception e)
        {
            Logger.Log(this.name, e.Message, LogType.ERROR);
            return;
        }
    }

    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        U = originValues.Uc;
        I = originValues.Ic;
        R = originValues.Rc;
        Logger.Log(this.name, "Measured => U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);

        DisplayValues(U, I, R, passValues.circuitType.ToString());

        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available", LogType.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);

    }

    public override (float, bool) GetResistanceSum()
    {
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available", LogType.WARNING);
            return (0,connected);
        }
        var (nextR, nextConnected) = nextNode.GetResistanceSum();
        nextConnected = nextConnected ? connected:nextConnected;

        Logger.Log(this.name, "Skipping resistance sum, measuring tool active", LogType.INFO);
        return (nextR,nextConnected);
    }

    private void DisplayValues(float U, float I, float R, string mode)
    {
        multimeterData.type = mode;
        switch(meterType)
        {
            case MeterType.OHMMETER:
                multimeterData.value = NodeCalculationModel.FormatValue(R, CircuitValueType.RESISTANCE);
                break;
            case MeterType.VOLTMETER:
                multimeterData.value = NodeCalculationModel.FormatValue(U, CircuitValueType.VOLTAGE);
                break;
            case MeterType.AMPMETER:
                multimeterData.value = NodeCalculationModel.FormatValue(I, CircuitValueType.CURRENT);
                break;
        }
    }

    void OnEnable()
    {
        circuitActiveStateEventChannel.OnEventRaised += CircuitActiveStateChange;
    }

    void OnDisable()
    {
        circuitActiveStateEventChannel.OnEventRaised -= CircuitActiveStateChange;
    }

    public void Reset()
    {
        multimeterData.type = "";
        multimeterData.value = "";
    }

    private void CircuitActiveStateChange(CircuitActiveStateEvent @event)
    {
        locked = @event.CircuitActive;
        if(!locked)
        {
            Reset();
        }
    } 
}
