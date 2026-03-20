using System;
using UnityEngine;

public class Nod_Branch : Node
{
    [SerializeField] private Node[] nextBranchNode;
    [SerializeField] private float[] branch_R;
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        Logger.Log(this.name, type.ToString(), Log_Type.INFO);
        if(type == Node_Type.BRANCH_OUT)
        {
            (U,I) = NodeCalculationModel.CalculateNodeValues(passValues, R);
            NodeDataModel localValues = new NodeDataModel(U, 0f, 0f, passValues.source_Type, passValues.circuit_Type);
            NodeDataModel staticValues = new NodeDataModel(U, 0f, 0f, passValues.source_Type, passValues.circuit_Type);
            for(int i =  0; i < nextBranchNode.Length; i++)
            {
                if(nextBranchNode[i] == null)
                {
                    Logger.Log(this.name, "Extra empty branch or not Connected branch", Log_Type.WARNING);
                    continue;
                }
                float local_I = NodeCalculationModel.CalculateCurrentDivider(I,branch_R,i);
                localValues.SetValues(U, local_I, branch_R[i]);
                staticValues.SetValues(U, local_I, branch_R[i]);
                nextBranchNode[i].CalculateValues(localValues, staticValues);
            }
        }
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available" + R, Log_Type.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
    }

    public override float GetResistanceSum()
    {
        R = 0f;
        if(type == Node_Type.BRANCH_OUT)
        {
            Logger.Log(this.name, "Calculating Local Resistance", Log_Type.INFO);
            for(int i = 0; i < nextBranchNode.Length; i++)
            {
                if(nextBranchNode[i] == null)
                {
                    Logger.Log(this.name, "Extra empty branch or not Connected branch", Log_Type.WARNING);
                    continue;
                }
                branch_R[i] = nextBranchNode[i].GetResistanceSum();
                R += (float)Math.Pow(branch_R[i], -1);
            }
            R = (float)Math.Pow(R, -1);
            Logger.Log(this.name, "Local R: " + R, Log_Type.INFO);
        }
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available\n Returning this R: " + R, Log_Type.WARNING);
            return R;
        }
        return R + nextNode.GetResistanceSum();
    }
}
