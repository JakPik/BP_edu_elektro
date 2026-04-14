using System;
using UnityEngine;
using UnityEngine.UIElements;

public class Node_Source : Node
{
    #region Variables
    [SerializeField] private CircuitValueType sourceType;
    [SerializeField] private SignalType signalType;
    [SerializeField] private UIDocument uiDocument;

    [Header("Event Channels")]
    [SerializeField] private GenericVoidEventChannel nodeStateChangeChannel;
    [SerializeField] private GenericEventChannel<ButtonPressedEvent> startCalculationChannel;
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveStateEventChannel;

    private Label sourceTypeLabel;
    private Label signalTypeLabel;
    private Label valueLabel;
    #endregion

    void Awake()
    {
        sourceTypeLabel = uiDocument.rootVisualElement.Q<Label>("SourceType");
        signalTypeLabel = uiDocument.rootVisualElement.Q<Label>("SignalType");
        valueLabel = uiDocument.rootVisualElement.Q<Label>("Value");

        sourceTypeLabel.text = sourceType.ToString();

        signalTypeLabel.text = signalType.ToString();

        float displayValue = sourceType == CircuitValueType.VOLTAGE? U:I;

        valueLabel.text = NodeCalculationModel.FormatValue(displayValue, sourceType);
    }
    
    /// <summary>
    /// Starts the calculation process.
    /// </summary>
    /// <inheritdoc />
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        if (nextNode == null)
        {
            Logger.Log(this.name, "No next node available", LogType.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
    }

    public override (float,bool) GetResistanceSum()
    {
        if (nextNode == null)
        {
            Logger.Log(this.name, "No next node available", LogType.WARNING);
            return (0, false);
        }
        return nextNode.GetResistanceSum();
    }

    [ContextMenu("Construct line")]
    private void BuildLines()
    {
        BuildConections(null, 0);
    }
    public override void BuildConections(Node branchInRef, int branchId)
    {
        try {
            LineRenderer.BuildLine(this, GetOutPortPosition(0), nextNode.GetInPortPosition(branchId));
        }
        catch (Exception e)
        {
            Logger.Log(this.name, e.Message, LogType.ERROR);
            return;
        }
        nextNode.BuildConections(null, branchId);
    }

    private void OnEnable()
    {
        nodeStateChangeChannel.OnEventRaised += GetResistance;
        startCalculationChannel.OnEventRaised += Calculate;
    }

    private void OnDisable()
    {
        nodeStateChangeChannel.OnEventRaised -= GetResistance;
        startCalculationChannel.OnEventRaised -= Calculate;
    }

    private void GetResistance()
    {
        Logger.Log(this.name, "Start calculating total Resistance", LogType.SUCCESS);
        (R, connected) = GetResistanceSum();
        if(!connected)
        {
            Logger.Log(this.name, "Circuit not connected", LogType.WARNING);
        }
        else
        {
            Logger.Log(this.name, "Total R: " + R, LogType.INFO);
            Logger.Log(this.name, "Calculation completed", LogType.SUCCESS);
        }
    }

    private void Calculate(ButtonPressedEvent @event)
    {
        circuitActiveStateEventChannel.RaiseEvent(new CircuitActiveStateEvent(@event.IsON), this.name);
        if(!@event.IsON) return;
        if(!connected)
        {
            Logger.Log(this.name, "Circuit not connected", LogType.WARNING);
            return;
        }

        Logger.Log(this.name, "Start calculating node Values", LogType.SUCCESS);

        I = U / R;

        NodeDataModel outData = new NodeDataModel(U,I,R,sourceType,signalType);
        NodeDataModel originData = new NodeDataModel(U, I, R, sourceType, signalType);
        CalculateValues(outData, originData);


        Logger.Log(this.name, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        Logger.Log(this.name, "Calculation completed", LogType.SUCCESS);
    }

    [ContextMenu("Test Calculation")]
    private void TestCalculation()
    {
        GetResistance();
        Calculate(new ButtonPressedEvent(true));
    }

    
}
