/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node_Validation
 */

using System;
using UnityEngine;

public class Node_Validation : Node
{
    #region Variables
    [Space(10)]
    [Header("Control Value")]
    [Tooltip("Set what type of value is expected to be checked on calculation")]
    [SerializeField] private ElectricalQuantityType _expectedValueType = ElectricalQuantityType.VOLTAGE;

    [Tooltip("Assign value to set off unlock condition. Local value of valueType is compared to this value")]
    [SerializeField] private ComplexFloat _expectedValue;
    private ComplexFloat _Rc;

    [Space(10)]
    [SerializeField] private ControlPointUIPanel _uiPanel;

    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<NodeValidationEvent> _nodeValidation;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> _circuitActiveState;
    #endregion

    void Start()
    {
        _Rc.Refresh();
        _locked = true;
        _uiPanel?.SetData(GetValidationNodeData());
    }
    
    #region Calculation
    /// <summary>
    /// <para>Calculates based on the type of the node:</para>
    /// <list type="bullet">
    ///   <item>NodeType.NODE_PASSIVE => Reads the originValues and assignes it as local R,U,I.</item>
    ///   <item>NodeType.NODE_ACTIVE  => Calculates the local U,I.</item>
    ///   <item>NodeType.NODE_CONTROL => Calculates local U,I and Invokes NodeValidationEvent.</item>
    /// </list>
    /// </summary>
    /// <inheritdoc />
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        (U, I) = NodeCalculationModel.CalculateNodeValues(passValues, R);
        Logger.LogNode(this, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        _Rc = originValues.Rc;

        _nodeValidation?.RaiseEvent(new NodeValidationEvent(IsCorrectValue()), this.name);
        DisplayLockState(!IsCorrectValue());
        if (_nextNode != null) _nextNode.CalculateValues(passValues, originValues);
    }

    public override ComplexFloat GetResistanceSum(SignalType signalType, float frequency)
    {
        R.SetNumberAsInf(_expectedValueType == ElectricalQuantityType.RESISTANCE);

        Logger.LogNode(this, "Local R: " + R, LogType.INFO);

        return _nextNode == null? R : R + _nextNode.GetResistanceSum(signalType, frequency);
    }
    #endregion

    #region Node Conection
    public override bool CheckConection()
    {
        if (_nextNode == null) return _connected;

        var nextConnected = _nextNode.CheckConection();
        nextConnected = nextConnected ? _connected : nextConnected;

        return nextConnected;
    }
    #endregion

    #region Conection Construction
    public override void BuildConections(Node branchInRef, int branchId, string containerName)
    {
        if (_nextNode == null && branchInRef == null)
        {
            Logger.LogNode(this, "Line construction done. No nodes to connect to.", LogType.SUCCESS);
            return;
        }
        try
        {
            if (_nextNode != null)
            {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), _nextNode.GetInPortPosition(0), containerName);
                _nextNode.BuildConections(branchInRef, branchId, containerName);
            }
            else
            {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), branchInRef.GetInPortPosition(branchId), containerName);
            }
        }
        catch (Exception e)
        {
            Logger.LogNode(this, e.Message, LogType.ERROR);
            return;
        }
    }
    #endregion

    #region Node_Validation local logic
    private bool IsCorrectValue()
    {
        switch (_expectedValueType)
        {
            case ElectricalQuantityType.VOLTAGE:
                return U == _expectedValue;
            case ElectricalQuantityType.CURRENT:
                return I == _expectedValue;
            case ElectricalQuantityType.RESISTANCE:
                return _Rc == _expectedValue;
            default:
                return false;
        }
    }

    private void DisplayLockState(bool locked)
    {
        _uiPanel.SetData(GetValidationNodeData(locked));
    }
    #endregion
 
    #region Node_Source Get/Set data
    public ValidationNodeData GetValidationNodeData(bool locked = true)
    {
        return new ValidationNodeData
        {
            locked = locked,
            resistance = R,
            expectedValue = _expectedValue,
            expectedValueType = _expectedValueType
        };
    }
    
    public void SetValidationNodeData(ValidationNodeData nodeData)
    {
        R = nodeData.resistance;
        _expectedValue = nodeData.expectedValue;
        _expectedValueType = nodeData.expectedValueType;
        _uiPanel.SetData(nodeData.locked = _locked);
    }
    #endregion

    #region Event Handling Logic
    void OnEnable()
    {
        _circuitActiveState.OnEventRaised += CircuitActiveStateChange;
    }

    void OnDisable()
    {
        _circuitActiveState.OnEventRaised -= CircuitActiveStateChange;
    }

    private void CircuitActiveStateChange(CircuitActiveStateEvent @event)
    {
        if(!_nodeCircuit_ID.RoomMatch(@event.nodeCircuit_ID)) return;
        _locked = @event.circuitActive;
        if (!_locked)
        {
            _nodeValidation?.RaiseEvent(new NodeValidationEvent(false), this.name);
            DisplayLockState(true);
        }
    }
    #endregion
}
