using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class Node_ControlPoint : Node
{
    #region Variables
    [Space(10)]
    [Header("Control Value")]
    [Tooltip("Set what type of value is expected to be checked on calculation")]
    [SerializeField] private CircuitValueType valueType = CircuitValueType.VOLTAGE;

    [Tooltip("Assign value to set off unlock condition. Local value of valueType is compared to this value")]
    [SerializeField] private float expectedValue;

    [Space(10)]
    [SerializeField] private GameObject uiPanel;

    #region Events
    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<NodeValidationEvent> nodeValidation;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveState;
    #endregion

    private UIPanel _uiPanel;
    #endregion

    void Awake()
    {
        if (uiPanel.TryGetComponent(out UIPanel panel))
        {
            _uiPanel = panel;
        }
        else
        {
            Logger.LogNode(this.GetType().Name, uiPanel.name + " does not contain class UIPanel", LogType.ERROR);
        }
    }

    void Start()
    {
        _uiPanel?.SetData(GetControlPointData(true));
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

        nodeValidation?.RaiseEvent(new NodeValidationEvent(IsCorrectValue()), this.name);
        DisplayLockState(!IsCorrectValue());
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

    #region Node_ControlPoint local logic
    private bool IsCorrectValue()
    {
        switch (valueType)
        {
            case CircuitValueType.VOLTAGE:
                return U == expectedValue;
            case CircuitValueType.CURRENT:
                return I == expectedValue;
            default:
                return false;
        }
    }

    private ControlPointData GetControlPointData(bool locked)
    {
        return new ControlPointData
        {
            locked = locked,
            resistance = NodeCalculationModel.FormatValue(R, CircuitValueType.RESISTANCE),
            required = NodeCalculationModel.FormatValue(expectedValue, valueType)
        };
    }

    private void DisplayLockState(bool locked)
    {
        _uiPanel.SetData(GetControlPointData(locked));
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
        if (!locked)
        {
            nodeValidation?.RaiseEvent(new NodeValidationEvent(false), this.name);
            DisplayLockState(true);
        }
    }
    #endregion
}
