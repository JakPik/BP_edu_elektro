using System;
using UnityEngine;
using UnityEngine.Splines;

public class Nod_Branch : Node
{
    [SerializeField] private Node[] nextBranchNode = new Node[3];
    [SerializeField] private float[] branch_R = new float[3];
    [SerializeField] private bool[] branch_connected = new bool[3];

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
                staticValues.SetValues(U, local_I, R);
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

    public override (float, bool) GetResistanceSum()
    {
        R = 0f;
        int disconnectedBranches = 0;
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
                (branch_R[i], branch_connected[i]) = nextBranchNode[i].GetResistanceSum();
                disconnectedBranches += branch_connected[i]? 0:1;
                if(branch_R[i] != 0 && branch_connected[i])
                {
                    R += (float)Math.Pow(branch_R[i], -1);
                }
            }
            R = (float)Math.Pow(R, -1);
            Logger.Log(this.name, "Local R: " + R, LogType.INFO);
            connected = !(disconnectedBranches == branch_connected.Length);
        }

        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available\n Returning this R: " + R, LogType.WARNING);
            return (R, connected);
        }
        var (nextR, nextConnected) = nextNode.GetResistanceSum();
        if(type == NodeType.BRANCH_IN)
        {
            connected = nextConnected;
        }
        else {
            nextConnected = nextConnected?connected:nextConnected;
        }
        return (R + nextR, nextConnected);
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
