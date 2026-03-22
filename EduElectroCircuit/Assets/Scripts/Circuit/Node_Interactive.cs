using UnityEngine;
using UnityEngine.Splines;
using System;

public class Node_Interactive: Node
{
    [SerializeField] private bool readVoltage = false;
    [SerializeField] private bool readCurrent = false;
    [SerializeField] private bool readResistance = false;

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
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available", Log_Type.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
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
}
