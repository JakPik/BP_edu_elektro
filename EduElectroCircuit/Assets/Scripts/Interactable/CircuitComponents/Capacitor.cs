/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Resistor
 */

using System;
using System.Collections;
using UnityEngine;

public class Capacitor : CircuitComponent
{
    [SerializeField] private CapacitorData _capacitorData;

    private Coroutine _DC_CapacitorRoutine;
    private bool _capacitorChargingStarted;
    const float ChargingMultiplier = 8;
    public float GetCapacitance() => _capacitorData.Capacitance;
    
    public void LoadCapacitor(float capacitance)
    {
        _capacitorData = new CapacitorData(capacitance);
        SaveCurrentState();
        LockGrab(false);
        ComponentPropertyData[] data =
        {
            new ComponentPropertyData
            {
                label = "Kapacita",
                value = NodeCalculationModel.FormatValue(capacitance, ElectricalQuantityType.CAPACITANCE)
            }
        };
        _componentPropertyPanel.SetData(data, "Kondenzátor");
    }

    protected override void SendData(INodeInteraction node, bool placed = true)
    {
        if (placed)
        {
            node?.SetComponentData(_capacitorData, this.gameObject);
        }
        else
        {
            node?.SetComponentData(null, this.gameObject);
        }
    }

    public override void SetData(ComponentControlState controlState, ComplexFloat voltage, ComplexFloat current, ComplexFloat R, bool isDC)
    {
        if(isDC && !_capacitorChargingStarted && controlState == ComponentControlState.ACTIVE)
        {
            _capacitorChargingStarted = true;
            _DC_CapacitorRoutine = StartCoroutine(StartCapacitorCharging());
        }
        if(controlState == ComponentControlState.INACTIVE)
        {
            if(_DC_CapacitorRoutine != null) StopCoroutine(_DC_CapacitorRoutine);
            _capacitorData.UpdatChargingResistance(new ComplexFloat(), false);
            _capacitorChargingStarted = false;
            //SendData(nodeInteraction);
        }
    }
    private IEnumerator StartCapacitorCharging()
    {
        float elapsed = 0f;
        float capacitance = _capacitorData.Capacitance;
        float totalRezistance = 255;
        ComplexFloat R = new ComplexFloat();
        while(_capacitorData.ChargingRezistanceChange.GetMagnitude() < 100000)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
            float divider = capacitance * totalRezistance * ChargingMultiplier;
            R.real += Mathf.Exp(elapsed/divider);
            _capacitorData.UpdatChargingResistance(R, false);
            SendData(nodeInteraction);
        }
        _capacitorData.UpdatChargingResistance(R, true);
        SendData(nodeInteraction);
        _DC_CapacitorRoutine = null;
    }

    public override ComponentData GetComponentData() => _capacitorData;

    public override bool CanConnectToNode() => nodeInteraction?.CanConnect(_capacitorData.type) ?? false;
}
