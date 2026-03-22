using System;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Splines;

public abstract class Node: MonoBehaviour
{
    [SerializeField] protected Node_Type type;

    [SerializeField] protected float R;
    [SerializeField] protected float U;
    [SerializeField] protected float I;

    [SerializeField] protected Node nextNode;

    [SerializeField] protected SplineContainer spline;
    [SerializeField] protected GameObject line;
    [SerializeField] protected GameObject[] outPort;
    [SerializeField] protected GameObject[] inPort;

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
