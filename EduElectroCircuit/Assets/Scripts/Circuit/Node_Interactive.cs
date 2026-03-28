using UnityEngine;
using UnityEngine.Splines;
using System;
using UnityEngine.Events;
using Unity.VisualScripting;

public class Node_Interactive: Node, INodeInteraction
{
    #region Variables
    [Space(10)]
    [Header("Control Value")]
    [Tooltip("Assign Voltage Value at which doorLockEvent Gets fired. This variable is used only when Node_Type == NODE_CONTROL")]
    [SerializeField] private float expected_U;
    [SerializeField] private CircuitComponent refConnected;
    [SerializeField] private Vector3 positionOffset;

    #region Events
    [Header("Event Channels - Raised Events")]
    [SerializeField] private GenericVoidEventChannel nodeStateChangeChannel;
    [SerializeField] private GenericEventChannel<NodeValidationEvent> nodeValidationEventChannel;
    #endregion
    #endregion


    /// <summary>
    /// <para>Calculates based on the type of the node:</para>
    /// <list type="bullet">
    ///   <item>NodeType.NODE_PASSIVE => Reads the originValues and assignes it as local R,U,I.</item>
    ///   <item>NodeType.NODE_ACTIVE  => Calculates the local U,I.</item>
    ///   <item>NodeType.NODE_CONTROL => Calculates local U,I and Invokes NodeValidationEvent.</item>
    /// </list>
    /// </summary>
    /// <inheritdoc />
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        if(type == NodeType.NODE_PASSIVE)
        {
            U = originValues.Uc;
            I = originValues.Ic;
            R = originValues.Rc;
            Logger.Log(this.name, "Measured => U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        }
        else
        {
            (U, I) = NodeCalculationModel.CalculateNodeValues(passValues, R);
            Logger.Log(this.name, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        }
        if(type == NodeType.NODE_CONTROL)
        {
            nodeValidationEventChannel?.RaiseEvent(new NodeValidationEvent(U == expected_U), this.name);
        }
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available", LogType.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
    }

    public override (float, bool) GetResistanceSum()
    {
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available\n Returning this R: " + R, LogType.WARNING);
            return (R,connected);
        }

        var (nextR, nextConnected) = nextNode.GetResistanceSum();
        nextConnected = nextConnected ? connected:nextConnected;

        if(type == NodeType.NODE_PASSIVE)
        {
            Logger.Log(this.name, "Skipping resistance sum, measuring tool active", LogType.INFO);
            return (nextR,nextConnected);
        }
        Logger.Log(this.name, "Local R: " + R, LogType.INFO);
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
            if(nextNode != null) {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), nextNode.GetInPortPosition(0));
                nextNode.BuildConections(branchInRef, branchId);
            }
            else
            {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), branchInRef.GetInPortPosition(branchId));
            }
        }
        catch (Exception e)
        {
            Logger.Log(this.name, e.Message, LogType.ERROR);
            return;
        }
    }

    [ContextMenu("Updated")]
    private void UpdateNode()
    {
        if(nodeStateChangeChannel != null) {
            nodeStateChangeChannel.RaiseEvent(this.name);
        }
        else
        {
            Logger.Log(this.name, "NodeStateChangeChannel not assigned", LogType.ERROR);
        }
    }

    public (Vector3, Transform) GetTransform() => (transform.position + positionOffset, transform);

    public void SetComponentData(ComponentDataSO componentData, CircuitComponent circuitComponent)
    {
        refConnected = circuitComponent;
        connected = true;
        UpdateNode();
        if(componentData.type == ComponentType.RESISTOR)
        {
            R = ((ResistorDataSO)componentData).resistance;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent(out CircuitComponent component) && component == refConnected)
        {
            refConnected = null;
            connected = false;
            UpdateNode();
            R = 0;
            U = 0;
            I = 0;
        }
    }

    public bool CanConnect() => !connected;
}
