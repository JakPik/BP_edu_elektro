/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node_Source
 */

using System;
using UnityEngine;

public class Node_Source : Node
{
    #region Variables
    [SerializeField] private ElectricalQuantityType _sourceType;
    [SerializeField] private SignalType _signalType;
    [SerializeField] private float _frequency;
    [SerializeField] private SourceUIPanel _uiPanel;

    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> _circuitActiveStateEventChannel;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<NodeStateChange> _nodeStateChangeChannel;

    #endregion

    void Start()
    {
        _uiPanel.SetData(GetSourceData());
        CheckConection();
    }

    #region Calculation
    /// <summary>
    /// Starts the calculation process.
    /// </summary>
    /// <inheritdoc />
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        if (_nextNode == null) return;
        _nextNode.CalculateValues(passValues, originValues);
    }

    public override ComplexFloat GetResistanceSum(SignalType _signalType, float _frequency)
    {
        Logger.LogNode(this, "Start calculating total Resistance", LogType.SUCCESS);
        
        if (_nextNode == null) return new ComplexFloat();

        R = _nextNode.GetResistanceSum(_signalType, _frequency);
        Logger.LogNode(this, "Total R: " + R, LogType.INFO);
        return R;
    }
    #endregion

    #region Node Conection
    public override bool CheckConection()
    {
        Logger.LogNode(this, "Checking Circuit conection state", LogType.SUCCESS);
        _connected = _nextNode.CheckConection();
        if (!_connected)
        {
            Logger.LogNode(this, "Circuit is disconnected", LogType.WARNING);
        }
        else
        {
            Logger.LogNode(this, "Circuit is connected", LogType.SUCCESS);
        }
        return _connected;
    }
    #endregion

    #region Conection Construction
    public override void BuildConections(Node branchInRef, int branchId, string containerName)
    {
        try
        {
            LineRenderer.BuildLine(this, GetOutPortPosition(0), _nextNode.GetInPortPosition(branchId), containerName);
        }
        catch (Exception e)
        {
            Logger.LogNode(this, e.Message, LogType.ERROR);
            return;
        }
        _nextNode.BuildConections(null, branchId, containerName);
    }
    #endregion
 
    #region Node_Source Get/Set data
    public SourceData GetSourceData()
    {
        ComplexFloat value;
        switch (_sourceType)
        {
            case ElectricalQuantityType.VOLTAGE: value = U; break;
            case ElectricalQuantityType.CURRENT: value = I; break;
            default: value = new ComplexFloat(); break;
        }
        return new SourceData
        {
            sourceValue = value,
            sourceType = _sourceType,
            signalType = _signalType
        };
    }

    public void SetSourceData(SourceData sourceData)
    {
        _sourceType = sourceData.sourceType;
        _signalType = sourceData.signalType;
        switch(_sourceType)
        {
            case ElectricalQuantityType.VOLTAGE:
                U = sourceData.sourceValue;
                break;
            case ElectricalQuantityType.CURRENT:
                I = sourceData.sourceValue;
                break;
            default:
                Logger.Log(this, "Source Node", _sourceType.ToString() + " is not a valid Source type", LogType.EXCEPTION);
                throw new Exception();
        }
        _uiPanel.SetData(sourceData);
    }
    #endregion

    #region Event Handling Logic
    private void OnEnable()
    {
        _nodeStateChangeChannel.OnEventRaised += GetConectionState;
    }

    private void OnDisable()
    {
        _nodeStateChangeChannel.OnEventRaised -= GetConectionState;
    }

    private void GetConectionState(NodeStateChange @event)
    {
        if(_nodeCircuit_ID != @event.nodeCircuit_ID) return;
        _uiPanel.TurnCircuitOff();
        _circuitActiveStateEventChannel.RaiseEvent(new CircuitActiveStateEvent(false, _nodeCircuit_ID), this.name);
        CheckConection();
    }

    public void Calculate(bool isOn)
    {
        _circuitActiveStateEventChannel.RaiseEvent(new CircuitActiveStateEvent(isOn, _nodeCircuit_ID), this.name);
        if (!isOn) return;
        if (!_connected)
        {
            Logger.LogNode(this, "Circuit not connected", LogType.WARNING);
            return;
        }

        Logger.LogNode(this, "Start circuit Calculation", LogType.SUCCESS);
        GetResistanceSum(_signalType, _frequency);
        switch(_sourceType)
        {
            case ElectricalQuantityType.VOLTAGE:
                I = U / R;
                break;
            case ElectricalQuantityType.CURRENT:
                U = R * I;
                break;
            default:
                Logger.Log(this, "Source Node", _sourceType.ToString() + " is not a valid Source type", LogType.EXCEPTION);
                throw new Exception();
        }

        NodeDataModel outData = new NodeDataModel(U, I, R, _sourceType, _signalType);
        NodeDataModel originData = new NodeDataModel(U, I, R, _sourceType, _signalType);
        CalculateValues(outData, originData);

        Logger.LogNode(this, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        Logger.LogNode(this, "Calculation completed", LogType.SUCCESS);
    }
    #endregion

    #region Debug Inspector Call Functions

    [ContextMenu("Construct line")]
    private void BuildLines()
    {
        string containerName = this.transform.parent.transform.parent.name;
        BuildConections(null, 0, containerName);
    }

    [ContextMenu("Test Calculation")]
    private void TestCalculation()
    {
        GetResistanceSum(_signalType, _frequency);
        Calculate(true);
    }
    #endregion
}