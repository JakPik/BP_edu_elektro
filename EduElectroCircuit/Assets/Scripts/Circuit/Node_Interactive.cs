using UnityEngine;
using System;

public class Node_Interactive : Node, INodeInteraction
{
    #region Variables
    [Space(10)]
    [Header("Control Value")]
    [SerializeField] private GameObject refConnected;
    [SerializeField] private Vector3 positionOffset;

    #region Events
    [Header("Raised Events")]
    [SerializeField] private GenericVoidEventChannel nodeStateChange;
    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveState;
    #endregion
    #endregion
    void Awake()
    {
        if (nodeStateChange != null && !(nodeStateChange is NodeStateChangeEventChannel))
        {
            Logger.LogNode(this, "nodeValidation does not have NodeValidationEventChannel assigned", LogType.ERROR);
            nodeStateChange = null;
        }
    }

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
        (U, I) = NodeCalculationModel.CalculateNodeValues(passValues, R);
        Logger.LogNode(this, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);

        if (nextNode == null)
        {
            Logger.LogNode(this, "No next node available", LogType.WARNING);
            return;
        }

        nextNode.CalculateValues(passValues, originValues);
    }

    public override (float, bool) GetResistanceSum()
    {
        if (nextNode == null)
        {
            Logger.LogNode(this, "No next node available\n Returning this R: " + R, LogType.WARNING);
            return (R, connected);
        }

        var (nextR, nextConnected) = nextNode.GetResistanceSum();
        nextConnected = nextConnected ? connected : nextConnected;

        Logger.LogNode(this, "Local R: " + R, LogType.INFO);
        return (R + nextR, nextConnected);
    }

    public override void BuildConections(Node branchInRef, int branchId)
    {
        if (nextNode == null && branchInRef == null)
        {
            Logger.LogNode(this, "Line construction done. No nodes to connect to.", LogType.SUCCESS);
            return;
        }
        try
        {
            if (nextNode != null)
            {
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
            Logger.LogNode(this, e.Message, LogType.ERROR);
            return;
        }
    }

    #region INodeInteraction
    public (Vector3, Transform) GetTransform() => (transform.position + positionOffset, transform);

    public void SetComponentData(ComponentDataSO componentData, GameObject circuitComponent)
    {
        refConnected = circuitComponent;
        connected = true;
        if (componentData.type == ComponentType.RESISTOR)
        {
            R = ((ResistorDataSO)componentData).resistance;
        }
        nodeStateChange?.RaiseEvent(this.name);
    }

    public bool CanConnect() => !connected && !locked;

    public bool CanGrab() => !locked;
    #endregion

    #region Node_Interactive local logic
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == refConnected)
        {
            refConnected = null;
            connected = false;
            R = 0;
            U = 0;
            I = 0;
            nodeStateChange?.RaiseEvent(this.name);
        }
    }
    #endregion

    #region Event Handling Logic
    void OnEnable()
    {
        circuitActiveState.OnEventRaised += CircuitActiveStateChange;
    }

    void OnDisable()
    {
        circuitActiveState.OnEventRaised -= CircuitActiveStateChange;
    }

    private void CircuitActiveStateChange(CircuitActiveStateEvent @event)
    {
        locked = @event.CircuitActive;
        if (refConnected != null && refConnected.TryGetComponent(out IGrabable grabable))
        {
            Logger.LogNode(this, "Grab lock state changed for: " + refConnected.name, LogType.INFO);
            grabable.LockGrab(locked);
        }
    }
    #endregion
}
