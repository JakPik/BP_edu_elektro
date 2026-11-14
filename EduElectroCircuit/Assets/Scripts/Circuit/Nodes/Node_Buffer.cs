/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node_Source
 */

using System;
using UnityEngine;

public class Node_Buffer : Node
{

    #region Calculation
    /// <summary>
    /// Starts the calculation process.
    /// </summary>
    /// <inheritdoc />
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        if (!_connected)
        {
            Logger.LogNode(this, "Circuit after Buffer not connected", LogType.WARNING);
            return;
        }
        Logger.LogNode(this, "Start Buffer circuit Calculation", LogType.SUCCESS);
        U = passValues.Uc;
        I = U / R;
        Logger.LogNode(this, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);

        NodeDataModel outData = new NodeDataModel(U, I, R, passValues.sourceType, passValues.signalType);
        NodeDataModel originData = new NodeDataModel(U, I, R, passValues.sourceType, passValues.signalType);

        if (_nextNode == null) return;
        _nextNode.CalculateValues(outData, originData);
    }

    public override ComplexFloat GetResistanceSum(SignalType _signalType, float _frequency)
    {
        if (_connected && _nextNode != null) {
            R = _nextNode.GetResistanceSum(_signalType, _frequency);
            Logger.LogNode(this, "Total R: " + R, LogType.INFO);
        }
        return new ComplexFloat(0,0,true);
    }
    #endregion

    #region Node Conection
    public override bool CheckConection()
    {
        _connected = _nextNode?.CheckConection()??false;
        return true;
    }
    #endregion

    #region Conection Construction
    public override void BuildConections(Node branchInRef, int branchId, string containerName)
    {
        try
        {
            LineRenderer.BuildLine(this, GetOutPortPosition(0), _nextNode.GetInPortPosition(branchId), containerName);
        }
        catch (Exception e)
        {
            Logger.LogNode(this, e.Message, LogType.ERROR);
            return;
        }
        _nextNode.BuildConections(null, branchId, containerName);
    }
    #endregion
 



    #region Debug Inspector Call Functions

    [ContextMenu("Construct line")]
    private void BuildLines()
    {
        string containerName = this.transform.parent.transform.parent.name;
        BuildConections(null, 0, containerName);
    }
    #endregion

}