using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Splines;

public abstract class Node: MonoBehaviour
{
    [Header("Node Type")]
    [SerializeField] protected Node_Type type;

    [Header("Node Ports")]
    [Tooltip("Out port used for line generation")]
    [SerializeField] protected GameObject[] outPort;
    [Tooltip("In port used for line generation")]
    [SerializeField] protected GameObject[] inPort;

    protected float R;
    protected float U;
    protected float I;

    [Header("Refferences")]
    [SerializeField] protected Node nextNode;

    [SerializeField] protected SplineContainer spline;
    [SerializeField] protected GameObject line;


    /// <summary>
    /// Calculates Voltage and Current for current node
    /// </summary>
    /// <param name="passValues"></param>
    /// <param name="originValues"></param>
    public abstract void CalculateValues(NodeDataModel passValues, NodeDataModel originValues);

    public abstract float GetResistanceSum();

    public Node_Type GetNodeType()
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
