/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node_Measurement
 */

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Node_Measurement : Node
{
    #region Variables
    [SerializeField] private MeterType meterType;
    [SerializeField] private GameObject uiPanel;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveState;

    private UIPanel _uiPanel;
    private string _signalType = "";
    private string _displayValue = "";
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
        _uiPanel.SetData(GetMeasurePointData());
    }

    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        U = originValues.Uc;
        I = originValues.Ic;
        R = originValues.Rc;
        Logger.LogNode(this, "Measured => U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);

        DisplayValues(passValues.circuitType.ToString());

        if (nextNode == null)
        {
            Logger.LogNode(this, "No next node available", LogType.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
    }

    public override (ComplexFloat, bool) GetResistanceSum()
    {
        if (nextNode == null)
        {
            Logger.LogNode(this, "No next node available", LogType.WARNING);
            return (new ComplexFloat(), connected);
        }
        var (nextR, nextConnected) = nextNode.GetResistanceSum();
        nextConnected = nextConnected ? connected : nextConnected;

        Logger.LogNode(this, "Skipping resistance sum, measuring tool active", LogType.INFO);
        return (nextR, nextConnected);
    }

    public override void BuildConections(Node branchInRef, int branchId, string containerName)
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
                LineRenderer.BuildLine(this, GetOutPortPosition(0), nextNode.GetInPortPosition(0), containerName);
                nextNode.BuildConections(branchInRef, branchId, containerName);
            }
            else
            {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), branchInRef.GetInPortPosition(branchId), containerName);
            }
        }
        catch (Exception e)
        {
            Logger.LogNode(this, e.Message, LogType.ERROR);
            return;
        }
    }

    public Dictionary<string, string> GetEventChannels()
    {
        return new Dictionary<string, string> {
            { "circuitActiveState", circuitActiveState.name }
        };
    }

    public string GetMeterType() => meterType.ToString();
    public void SetMeterType(string type)
    {
        meterType = Enum.Parse<MeterType>(type);
    }

    #region Node_MeasurePoint local logic
    private MeasurePointData GetMeasurePointData()
    {
        return new MeasurePointData
        {
            name = EnumTransformer.MeterTypeToString(meterType),
            signal = _signalType,
            value = _displayValue
        };
    }

    private void DisplayValues(string mode)
    {
        _signalType = mode;
        switch (meterType)
        {
            case MeterType.OHMMETER:
                _displayValue = NodeCalculationModel.FormatValue(R, CircuitValueType.RESISTANCE);
                break;
            case MeterType.VOLTMETER:
                _displayValue = NodeCalculationModel.FormatValue(U, CircuitValueType.VOLTAGE);
                break;
            case MeterType.AMPMETER:
                _displayValue = NodeCalculationModel.FormatValue(I, CircuitValueType.CURRENT);
                break;
        }

        _uiPanel.SetData(GetMeasurePointData());
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
            _signalType = "";
            _displayValue = "";
            _uiPanel.SetData(GetMeasurePointData());
        }
    }
    #endregion
}
