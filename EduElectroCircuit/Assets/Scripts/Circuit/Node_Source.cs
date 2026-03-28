using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.Splines;

public class Node_Source : Node
{
    #region Variables
    [SerializeField] private SourceType sourceType;
    [SerializeField] private CircuitType circuitType;
    [SerializeField] private InputActionReference getResistanceAction;
    [SerializeField] private InputActionReference calculateAction;

    [Header("Event Channels")]
    [SerializeField] private GenericVoidEventChannel nodeStateChangeChannel;
    [SerializeField] private GenericVoidEventChannel startCalculationChannel;
    #endregion
    
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
        nodeStateChangeChannel.OnEventRaised += GetResistanceEvent;
        getResistanceAction.action.started += GetResistance;
        calculateAction.action.started += Calculate;
        startCalculationChannel.OnEventRaised += TestCalculation;
    }

    private void OnDisable()
    {
        nodeStateChangeChannel.OnEventRaised -= GetResistanceEvent;
        getResistanceAction.action.started -= GetResistance;
        calculateAction.action.started -= Calculate;
        startCalculationChannel.OnEventRaised -= TestCalculation;
    }

    private void GetResistanceEvent()
    {
        GetResistance(new InputAction.CallbackContext());
    }
    private void GetResistance(InputAction.CallbackContext context)
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

    private void Calculate(InputAction.CallbackContext context)
    {
        if(!connected)
        {
            Logger.Log(this.name, "Circuit not connected", LogType.WARNING);
            return;
        }
        Logger.Log(this.name, "Start calculating node Values", LogType.SUCCESS);
        I = U / R;
        NodeDataModel outData = new NodeDataModel(U,I,R,sourceType,circuitType);
        NodeDataModel originData = new NodeDataModel(U, I, R, sourceType, circuitType);
        CalculateValues(outData, originData);
        Logger.Log(this.name, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        Logger.Log(this.name, "Calculation completed", LogType.SUCCESS);
    }

    [ContextMenu("Test Calculation")]
    private void TestCalculation()
    {
        GetResistance(new InputAction.CallbackContext());
        Calculate(new InputAction.CallbackContext());
    }

    
}
