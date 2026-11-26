/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node_Measurement
 */

using System;
using UnityEngine;

public class Node_Phasor : Node
{
    #region Variables
    [SerializeField] private UIPanel _uiPanel;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> _circuitActiveState;

    #endregion

    void Start()
    {
        DisplayValues(false);
    }

    #region Calculation
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        U = passValues.Uc;
        R = originValues.Rc;
        I = U / R;
        Logger.LogNode(this, "Measured => U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);

        DisplayValues(true);

        if (_nextNode != null) _nextNode.CalculateValues(passValues, originValues);
    }

    public override ComplexFloat GetResistanceSum(SignalType signalType, float frequency)
    {
        ComplexFloat nextR = new ComplexFloat(0,0, true);
        if (_nextNode != null) nextR += _nextNode.GetResistanceSum(signalType, frequency);

        return nextR;
    }
    #endregion

    #region Node Conections
    public override bool CheckConection()
    {
        if (_nextNode == null)
        {
            return _connected;
        }
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

    #region Node_MeasurePoint local logic


    private void DisplayValues(bool visible)
    {
        if(visible) {
            _uiPanel.SetData(new PhasorDiagramData
            {
                voltage = NodeCalculationModel.FormatValue(U, ElectricalQuantityType.VOLTAGE),
                current = NodeCalculationModel.FormatValue(I, ElectricalQuantityType.CURRENT),
                voltageComplex = U,
                currentComplex = I,
                visible = visible
            });
        }
        else
        {
            _uiPanel.SetData(new PhasorDiagramData
            {
                voltage = "",
                current = "",
                voltageComplex = U,
                currentComplex = I,
                visible = visible
            });
        }
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
            DisplayValues(false);
        }
    }
    #endregion
}
