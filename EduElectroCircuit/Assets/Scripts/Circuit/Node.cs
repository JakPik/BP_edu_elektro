using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Splines;

public abstract class Node: MonoBehaviour
{
    #region  Variables
    [Header("Node Type")]
    [SerializeField] protected NodeType type;

    [Header("Node Ports")]
    [Tooltip("Out port used for line generation")]
    [SerializeField] protected GameObject[] outPort;
    [Tooltip("In port used for line generation")]
    [SerializeField] protected GameObject[] inPort;

    [Header("Refferences")]
    [SerializeField] protected Node nextNode;
    [SerializeField] protected SplineContainer spline;
    [SerializeField] protected GameObject line;

    [SerializeField] protected float R;
    [SerializeField] protected float U;
    [SerializeField] protected float I;
    [SerializeField] protected bool connected = false;
    #endregion

    /// <summary>
    /// Calculates the voltage and current for this node.
    /// </summary>
    /// <param name="passValues">
    /// Data model containing values passed from the previous node.
    /// These values are updated according to this node's calculation logic.
    /// </param>
    /// <param name="originValues">
    /// Data model representing the values of the node from which this calculation originates.
    /// For linear connections, this is usually the source node; for branched connections,
    /// this contains the values of the node at the start of the branch.
    /// These values are not modified and serve as a reference for calculation purposes.
    /// </param>
    public abstract void CalculateValues(NodeDataModel passValues, NodeDataModel originValues);

    public abstract (float, bool) GetResistanceSum();

    public NodeType GetNodeType()
    {
        return type;
    }

    public (Vector3, Vector3, Vector3) GetOutPortPosition(int idx)
    {
        if(idx < outPort.Length)
        {
            return (transform.TransformDirection(outPort[idx].transform.localPosition.normalized), outPort[idx].transform.position, outPort[idx].transform.localPosition);
        }
        throw new Exception("OutPort not found");
    }

    public (Vector3, Vector3) GetInPortPosition(int idx)
    {
        if(idx < inPort.Length)
        {
            return (transform.TransformDirection(inPort[idx].transform.localPosition.normalized), inPort[idx].transform.position);
        }
        throw new Exception("InPort not found");
    }

    public abstract void BuildConections(Node branchInRef, int branchId);
}
