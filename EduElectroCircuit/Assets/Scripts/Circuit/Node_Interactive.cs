using UnityEngine;
using UnityEngine.Splines;
using System;
using UnityEngine.Events;
using Unity.VisualScripting;

public class Node_Interactive: Node
{
    [Space(10)]
    [Header("")]
    [SerializeField] private UnityEvent onCorrect;
    [SerializeField] private UnityEvent onIncorrect;
    [SerializeField] private float expected_U;

    [SerializeField] private GenericVoidEventChannel nodeStateChangeChannel;

    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        if(type == Node_Type.NODE_PASSIVE)
        {
            U = originValues.Uc;
            I = originValues.Ic;
            R = originValues.Rc;
            Logger.Log(this.name, "Measured => U: " + U +"V, I: " + I + " mA, R: " + R, Log_Type.INFO);
        }
        else
        {
            (U, I) = NodeCalculationModel.CalculateNodeValues(passValues, R);
            Logger.Log(this.name, "U: " + U +"V, I: " + I + " mA, R: " + R, Log_Type.INFO);
        }
        if(type == Node_Type.NODE_CONTROL)
        {
            ValueCheck();
        }
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available", Log_Type.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
    }

    private void ValueCheck()
    {
        if(U == expected_U)
        {
            onCorrect?.Invoke();
        }
        else
        {
            onIncorrect?.Invoke();
        }
    }

    public override float GetResistanceSum()
    {
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available\n Returning this R: " + R, Log_Type.WARNING);
            return R;
        }
        if(type == Node_Type.NODE_PASSIVE)
        {
            Logger.Log(this.name, "Skipping resistance sum, measuring tool active", Log_Type.INFO);
            return nextNode.GetResistanceSum();
        }
        Logger.Log(this.name, "Local R: " + R, Log_Type.INFO);
        return R + nextNode.GetResistanceSum();
    }

    public override void BuildConections(Node branchInRef, int branchId)
    {
        if(nextNode == null && branchInRef == null)
        {
            Logger.Log(this.name, "Line construction done. No nodes to connect to.", Log_Type.SUCCESS);
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
            Logger.Log(this.name, e.Message, Log_Type.ERROR);
            return;
        }
    }

    [ContextMenu("Updated")]
    private void UpdateNode()
    {
        if(nodeStateChangeChannel != null) {
            nodeStateChangeChannel.RaiseEvent();
        }
        else
        {
            Logger.Log(this.name, "NodeStateChangeChannel not assigned", Log_Type.ERROR);
        }
    }
}
