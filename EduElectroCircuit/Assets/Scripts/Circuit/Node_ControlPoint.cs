using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Node_ControlPoint : Node
{
    #region Variables
    [Space(10)]
    [Header("Control Value")]
    [Tooltip("Assign Voltage Value at which doorLockEvent Gets fired. This variable is used only when Node_Type == NODE_CONTROL")]
    [SerializeField] private float expected_U;
    [SerializeField] private UIDocument uiDocument;

    #region Events
    [Header("Event Channels - Raised Events")]
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveStateEventChannel;
    [SerializeField] private GenericEventChannel<NodeValidationEvent> nodeValidationEventChannel;
    #endregion

    private Label stateLabel;
    private Label resistanceLabel;
    private Label requireLabel;
    #endregion

    void Awake()
    {
        stateLabel = uiDocument.rootVisualElement.Q<Label>("State");
        resistanceLabel = uiDocument.rootVisualElement.Q<Label>("Resistance");
        requireLabel = uiDocument.rootVisualElement.Q<Label>("Required");

        stateLabel.text = "Locked";
        UIStyleControl.StyleAsError(stateLabel);

        resistanceLabel.text = NodeCalculationModel.FormatValue(R, CircuitValueType.RESISTANCE);
        UIStyleControl.StyleAsSuccess(resistanceLabel);

        requireLabel.text = NodeCalculationModel.FormatValue(expected_U, CircuitValueType.VOLTAGE);
        UIStyleControl.StyleAsError(requireLabel);
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
        Logger.Log(this.name, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        
        nodeValidationEventChannel?.RaiseEvent(new NodeValidationEvent(U == expected_U), this.name);
        DisplayLockedState(!(U == expected_U));
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

    private void DisplayLockedState(bool locked)
    {
        if(locked)
        {
            stateLabel.text = "Locked";
            UIStyleControl.StyleAsError(stateLabel);
            UIStyleControl.StyleAsError(requireLabel);
        }
        else
        {
            stateLabel.text = "Unlocked";
            UIStyleControl.StyleAsSuccess(stateLabel);
            UIStyleControl.StyleAsSuccess(requireLabel);
        }
    }

    void OnEnable()
    {
        circuitActiveStateEventChannel.OnEventRaised += CircuitActiveStateChange;
    }

    void OnDisable()
    {
        circuitActiveStateEventChannel.OnEventRaised -= CircuitActiveStateChange;
    }

    private void CircuitActiveStateChange(CircuitActiveStateEvent @event)
    {
        locked = @event.CircuitActive;
        if(!locked)
        {
            nodeValidationEventChannel?.RaiseEvent(new NodeValidationEvent(false), this.name);
            DisplayLockedState(true);
        }
    } 
}
