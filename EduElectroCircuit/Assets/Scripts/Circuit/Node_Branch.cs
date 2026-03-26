using System;
using UnityEngine;
using UnityEngine.Splines;

public class Nod_Branch : Node
{
    [SerializeField] private Node[] nextBranchNode;
    [SerializeField] private float[] branch_R;

    /// <summary>
    /// <para>Calculates based on the type of the node:</para>
    /// <list type="bullet">
    ///   <item>NodeType.BRANCH_OUT => Calculates the local U,I.
    ///         Starts calculation for each branch and passes local values as originValues.
    /// </item>
    ///   <item>NodeType.BRANCH_IN  => Does nothing and passes calculation to the next node.</item>
    /// </list>
    /// </summary>
    /// <inheritdoc />
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        Logger.Log(this.name, type.ToString(), LogType.INFO);
        if(type == NodeType.BRANCH_OUT)
        {
            (U,I) = NodeCalculationModel.CalculateNodeValues(passValues, R);
            NodeDataModel localValues = new NodeDataModel(U, 0f, 0f, passValues.sourceType, passValues.circuitType);
            NodeDataModel staticValues = new NodeDataModel(U, 0f, 0f, passValues.sourceType, passValues.circuitType);
            for(int i =  0; i < nextBranchNode.Length; i++)
            {
                if(nextBranchNode[i] == null)
                {
                    Logger.Log(this.name, "Branch " + i + " is empty", LogType.EXCEPTION);
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
            Logger.Log(this.name, "No next node available" + R, LogType.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
    }

    public override float GetResistanceSum()
    {
        R = 0f;
        if(type == NodeType.BRANCH_OUT)
        {
            Logger.Log(this.name, "Calculating Local Resistance", LogType.INFO);
            for(int i = 0; i < nextBranchNode.Length; i++)
            {
                if(nextBranchNode[i] == null)
                {
                    Logger.Log(this.name, "Branch " + i + " is empty", LogType.EXCEPTION);
                    continue;
                }
                branch_R[i] = nextBranchNode[i].GetResistanceSum();
                R += (float)Math.Pow(branch_R[i], -1);
            }
            R = (float)Math.Pow(R, -1);
            Logger.Log(this.name, "Local R: " + R, LogType.INFO);
        }
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available\n Returning this R: " + R, LogType.WARNING);
            return R;
        }
        return R + nextNode.GetResistanceSum();
    }

    public override void BuildConections(Node branchInRef, int branchId)
    {
        if(nextNode == null && branchInRef == null)
        {
            Logger.Log(this.name, "Line construction done. No nodes to connect to.", LogType.SUCCESS);
            return;
        }
        try {
            if(type == NodeType.BRANCH_OUT)
            {
                Logger.Log(this.name, "Forward vector:" + this.transform.forward, LogType.WARNING);
                for(int i = 0; i < nextBranchNode.Length; i++)
                {
                    if(nextBranchNode[i] != null) {
                        LineRenderer.BuildLine(this, GetOutPortPosition(i), nextBranchNode[i].GetInPortPosition(0));
                        nextBranchNode[i].BuildConections(nextNode, i);
                    }
                    else
                    {
                        Logger.Log(this.name, "Branch " + i + " is empty", LogType.EXCEPTION);
                    }
                }
                nextNode.BuildConections(branchInRef, branchId);
            }
            else
            {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), nextNode.GetInPortPosition(0));
                nextNode.BuildConections(branchInRef, branchId);
            }
        }
        catch (Exception e)
        {
            Logger.Log(this.name, e.Message, LogType.ERROR);
            return;
        }
    }
}
