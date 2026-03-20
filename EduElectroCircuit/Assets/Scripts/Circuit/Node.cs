using System;
using UnityEngine;

public abstract class Node: MonoBehaviour
{
    [SerializeField] protected Node_Type type;

    [SerializeField] protected float R;
    [SerializeField] protected float U;
    [SerializeField] protected float I;

    [SerializeField] protected Node nextNode;

    public abstract void CalculateValues(NodeDataModel passValues, NodeDataModel originValues);

    public abstract float GetResistanceSum();

    public Node_Type GetNodeType()
    {
        return type;
    }
}
