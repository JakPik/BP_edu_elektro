using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;

public class Node_Source : Node
{
    [SerializeField] private Source_Type sourceType;
    [SerializeField] private Circuit_Type circuitType;

    [SerializeField] private InputActionReference getResistanceAction;
    [SerializeField] private InputActionReference calculateAction;

    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        if (nextNode == null)
        {
            Logger.Log(this.name, "No next node available", Log_Type.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
    }

    public override float GetResistanceSum()
    {
        BuildLines();
        if (nextNode == null)
        {
            Logger.Log(this.name, "No next node available", Log_Type.WARNING);
            return 0;
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
            Logger.Log(this.name, e.Message, Log_Type.ERROR);
            return;
        }
        nextNode.BuildConections(null, branchId);
    }

    private void OnEnable()
    {
        getResistanceAction.action.started += GetResistance;
        calculateAction.action.started += Calculate;
    }

    private void OnDisable()
    {
        getResistanceAction.action.started -= GetResistance;
        calculateAction.action.started -= Calculate;
    }

    private void GetResistance(InputAction.CallbackContext context)
    {
        Logger.Log(this.name, "Start calculating total Resistance", Log_Type.SUCCESS);
        R = GetResistanceSum();
        Logger.Log(this.name, "Total R: " + R, Log_Type.INFO);
        Logger.Log(this.name, "Calculation completed", Log_Type.SUCCESS);
    }

    private void Calculate(InputAction.CallbackContext context)
    {
        Logger.Log(this.name, "Start calculating node Values", Log_Type.SUCCESS);
        I = U / R;
        NodeDataModel outData = new NodeDataModel(U,I,R,sourceType,circuitType);
        NodeDataModel originData = new NodeDataModel(U, I, R, sourceType, circuitType);
        CalculateValues(outData, originData);
        Logger.Log(this.name, "U: " + U +"V, I: " + I + " mA, R: " + R, Log_Type.INFO);
        Logger.Log(this.name, "Calculation completed", Log_Type.SUCCESS);
    }

    [ContextMenu("Test Calculation")]
    private void TestCalculation()
    {
        GetResistance(new InputAction.CallbackContext());
        Calculate(new InputAction.CallbackContext());
    }

    
}
