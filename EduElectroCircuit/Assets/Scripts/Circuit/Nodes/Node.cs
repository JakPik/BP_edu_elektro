/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node
 */

using System;
using UnityEngine;

public abstract class Node : MonoBehaviour
{
    #region  Variables
    [Header("Node Type")]
    [SerializeField] protected NodeType _type;

    [Header("Node Ports")]
    [Tooltip("Out port used for line generation")]
    [SerializeField] protected GameObject[] _outPort;
    [Tooltip("In port used for line generation")]
    [SerializeField] protected GameObject[] _inPort;

    [Header("Refferences")]
    [SerializeField] protected Node _nextNode;

    [Header("Control Variables")]
    [SerializeField] protected NodeCircuit_ID _nodeCircuit_ID;
    [Space(5)]
    [SerializeField] protected ComplexFloat R;
    [SerializeField] protected ComplexFloat U;
    [SerializeField] protected ComplexFloat I;
    [Space(5)]
    [SerializeField] protected bool _connected = false;
    [SerializeField] protected bool _locked = false;
    #endregion

    #region Calculation
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

    public abstract ComplexFloat GetResistanceSum(SignalType signalType, float frequency);
    #endregion

    #region Node Conection
    public abstract bool CheckConection();
    #endregion

    #region Conection Construction
    public abstract void BuildConections(Node branchInRef, int branchId, string containerName);
    #endregion

    #region Node local logic
    protected PortData GetOutPortPosition(int idx)
    {
        if (idx < _outPort.Length)
        {
            return new PortData(transform.TransformDirection(_outPort[idx].transform.localPosition.normalized), _outPort[idx].transform.position, _outPort[idx].transform.localPosition);
        }
        throw new Exception("OutPort not found");
    }

    public PortData GetInPortPosition(int idx)
    {
        if (idx < _inPort.Length)
        {
            return new PortData(transform.TransformDirection(_inPort[idx].transform.localPosition.normalized), _inPort[idx].transform.position, null);
        }
        throw new Exception("InPort not found");
    }

    protected void RefreshValues()
    {
        U.Refresh();
        I.Refresh();
        R.Refresh();
    }
    #endregion
}
