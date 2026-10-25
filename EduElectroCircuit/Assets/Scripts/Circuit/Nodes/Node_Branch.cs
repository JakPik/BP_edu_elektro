/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node_Branch
 */

using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class Node_Branch : Node
{
    #region Variables
    [SerializeField] private Node[] _nextBranchNode = new Node[3];
    [SerializeField] private ComplexFloat[] _branch_R = new ComplexFloat[3];
    [SerializeField] private bool[] _branchConnected = new bool[3];
    private BranchLightRenderer _branchLightrender;

    private bool _shortCircuited = false;
    #endregion

    #region Calculation
    /// <summary>
    /// <param>Calculates based on the type of the node:</param>
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
        Logger.LogNode(this, _type.ToString(), LogType.INFO);
        if (_type == NodeType.BRANCH_OUT) BranchOutValueCalculation(passValues);

        if (_nextNode == null) return;

        _nextNode.CalculateValues(passValues, originValues);
    }

    private void BranchOutValueCalculation(NodeDataModel passValues)
    {
        ComplexFloat zeroRef = new ComplexFloat();
        ComplexFloat currentSplit = new ComplexFloat();
        ComplexFloat local_I;

        NodeDataModel localValues = new NodeDataModel(
            U,
            new ComplexFloat(),
            new ComplexFloat(),
            passValues.sourceType,
            passValues.circuitType);

        NodeDataModel staticValues = new NodeDataModel(
            U,
            new ComplexFloat(),
            new ComplexFloat(),
            passValues.sourceType,
            passValues.circuitType);

        (U, I) = NodeCalculationModel.CalculateNodeValues(passValues, R);

        if(_shortCircuited)
        {
            currentSplit = NodeCalculationModel.CalculateShortCircuitCurrentSplit(_branch_R, _branchConnected, I);
        }
        
        for (int i = 0; i < _nextBranchNode.Length; i++)
        {
            if(!_branchConnected[i]) continue;

            if(_shortCircuited)
            {
                local_I = (_branch_R[i].infinite || _branch_R[i].GetMagnitude() != 0)? zeroRef : currentSplit;
            }
            else
            {
                local_I = NodeCalculationModel.CalculateCurrentDivider(I, _branch_R, i);
            }
            localValues.SetValues(U, local_I, _branch_R[i]);
            staticValues.SetValues(U, local_I, R);
            _nextBranchNode[i].CalculateValues(localValues, staticValues);
        }
    }

    public override ComplexFloat GetResistanceSum(SignalType signalType, float frequency)
    {
        _shortCircuited = false;
        if (_type == NodeType.BRANCH_OUT)
        {
            (_shortCircuited, R, _branch_R) = NodeCalculationModel.CalculateBranchResistance(_nextBranchNode, _branchConnected, signalType, frequency);
            Logger.LogNode(this, "Calculating Local Resistance\nLocal R: " + R, LogType.INFO);
        }

        return _nextNode == null? R: R + _nextNode.GetResistanceSum(signalType, frequency);
    }
    #endregion

    #region Node Conections
    public override bool CheckConection()
    {
        if (_type == NodeType.BRANCH_OUT) _connected = CheckBranchOutConections();

        if(_nextNode == null)
        {
            _branchLightrender.SetBranchConectionLight(_branchConnected, _connected, _type);
            return _connected;
        }
        else
        {
            var nextConnected = _nextNode.CheckConection();
            if (_type == NodeType.BRANCH_OUT) _branchLightrender.SetBranchConectionLight(_branchConnected, null, NodeType.BRANCH_IN);
            if (_type == NodeType.BRANCH_IN) _connected = nextConnected;
            _branchLightrender.SetBranchConectionLight(_branchConnected, nextConnected ? _connected : nextConnected, _type);
            return nextConnected ? _connected : nextConnected;
        }
    }

    private bool CheckBranchOutConections()
    {
        int disconnectedBranches = 0;
        for (int i = 0; i < _nextBranchNode.Length; i++)
        {
            if (_nextBranchNode[i] != null)
            {
                _branchConnected[i] = _nextBranchNode[i].CheckConection();
                disconnectedBranches += _branchConnected[i] ? 0 : 1;
            }
            else
            {
                disconnectedBranches++;
            }
        }
        return !(disconnectedBranches == _branchConnected.Length);
    }
    #endregion

    #region Conection Construction
    public override void BuildConections(Node branchInRef, int branchId, string containerName)
    {
        try
        {
            if (_type == NodeType.BRANCH_OUT)
            {
                BuildBracnhOutConections(branchInRef, branchId, containerName);
            }
            else
            {
                BuildBranchInConection(branchInRef, branchId, containerName);
            }
        }
        catch (Exception e)
        {
            Logger.LogNode(this, e.Message, LogType.ERROR);
        }
    }

    private void BuildBracnhOutConections(Node branchInRef, int branchId, string containerName)
    {
        for (int i = 0; i < _nextBranchNode.Length; i++)
        {
            if (_nextBranchNode[i] != null)
            {
                LineRenderer.BuildLine(this, GetOutPortPosition(i), _nextBranchNode[i].GetInPortPosition(0), containerName, i);
                _nextBranchNode[i].BuildConections(_nextNode, i, containerName);
            }
            else
            {
                Logger.LogNode(this, "Branch " + i + " is empty", LogType.EXCEPTION);
            }
        }
        _nextNode.BuildConections(branchInRef, branchId, containerName);
    }

    private void BuildBranchInConection(Node branchInRef, int branchId, string containerName)
    {
        if(_nextNode == null)
        {
            LineRenderer.BuildLine(this, GetOutPortPosition(0), branchInRef.GetInPortPosition(branchId), containerName);
        }
        else
        {
            LineRenderer.BuildLine(this, GetOutPortPosition(0), _nextNode.GetInPortPosition(0), containerName);
            _nextNode.BuildConections(branchInRef, branchId, containerName);
        }
    }
    #endregion

    #region Node_Branch local logic
    public void InitLightRenderer(BranchLightRenderer branchLightRenderer)
    {
        _branchLightrender = branchLightRenderer;
    }
    #endregion
}
