using System;
using UnityEngine;

public class Node_Source : Node
{
    #region Variables
    [SerializeField] private CircuitValueType sourceType;
    [SerializeField] private SignalType signalType;
    [SerializeField] private GameObject uiPanel;

    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveStateEventChannel;

    [Header("Event Channels")]
    [SerializeField] private GenericVoidEventChannel nodeStateChangeChannel;
    [SerializeField] private GenericEventChannel<ButtonPressedEvent> startCalculationChannel;

    private UIPanel _uiPanel;
    #endregion
    void Awake()
    {
        if (uiPanel.TryGetComponent(out UIPanel panel))
        {
            _uiPanel = panel;
        }
        else
        {
            Logger.LogNode(this.GetType().Name, uiPanel.name + " does not contain class UIPanel", LogType.ERROR);
        }
    }

    void Start()
    {
        _uiPanel.SetData(GetSourceData());
    }

    /// <summary>
    /// Starts the calculation process.
    /// </summary>
    /// <inheritdoc />
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        if (nextNode == null)
        {
            Logger.LogNode(this, "No next node available", LogType.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
    }

    public override (float, bool) GetResistanceSum()
    {
        if (nextNode == null)
        {
            Logger.LogNode(this, "No next node available", LogType.WARNING);
            return (0, false);
        }
        return nextNode.GetResistanceSum();
    }

    public override void BuildConections(Node branchInRef, int branchId)
    {
        try
        {
            LineRenderer.BuildLine(this, GetOutPortPosition(0), nextNode.GetInPortPosition(branchId));
        }
        catch (Exception e)
        {
            Logger.LogNode(this, e.Message, LogType.ERROR);
            return;
        }
        nextNode.BuildConections(null, branchId);
    }

    #region Node_Source local logic
    private SourceData GetSourceData()
    {
        float value;
        switch (sourceType)
        {
            case CircuitValueType.VOLTAGE: value = U; break;
            case CircuitValueType.CURRENT: value = I; break;
            default: value = 0f; break;
        }
        return new SourceData
        {
            value = NodeCalculationModel.FormatValue(value, sourceType),
            source = sourceType.ToString(),
            signal = signalType.ToString()
        };
    }
    #endregion

    #region Event Handling Logic
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
        Logger.LogNode(this, "Start calculating total Resistance", LogType.SUCCESS);
        (R, connected) = GetResistanceSum();
        if (!connected)
        {
            Logger.LogNode(this, "Circuit not connected", LogType.WARNING);
        }
        else
        {
            Logger.LogNode(this, "Total R: " + R, LogType.INFO);
            Logger.LogNode(this, "Calculation completed", LogType.SUCCESS);
        }
    }

    private void Calculate(ButtonPressedEvent @event)
    {
        circuitActiveStateEventChannel.RaiseEvent(new CircuitActiveStateEvent(@event.IsON), this.name);
        if (!@event.IsON) return;
        if (!connected)
        {
            Logger.LogNode(this, "Circuit not connected", LogType.WARNING);
            return;
        }

        Logger.LogNode(this, "Start circuit Calculation", LogType.SUCCESS);

        I = U / R;

        NodeDataModel outData = new NodeDataModel(U, I, R, sourceType, signalType);
        NodeDataModel originData = new NodeDataModel(U, I, R, sourceType, signalType);
        CalculateValues(outData, originData);



        Logger.LogNode(this, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        Logger.LogNode(this, "Calculation completed", LogType.SUCCESS);
    }
    #endregion

    #region Debug Inspector Call Functions

    [ContextMenu("Construct line")]
    private void BuildLines()
    {
        BuildConections(null, 0);
    }

    [ContextMenu("Test Calculation")]
    private void TestCalculation()
    {
        GetResistance();
        Calculate(new ButtonPressedEvent(true));
    }
    #endregion
}