/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Resistor
 */

using System;
using UnityEngine;

public class Inductor : CircuitComponent
{
    [SerializeField] private InductorData _inductorData;
    public float GetInductance() => _inductorData.Inductance;
    public int GetNumberOfTurns() => _inductorData.NumberOfTurns;
    
    public void LoadInductor(float inductance, int numberOfTurns)
    {
        _inductorData = new InductorData(inductance, numberOfTurns);
        SaveCurrentState();
        LockGrab(false);
        ComponentPropertyData[] data =
        {
            new ComponentPropertyData
            {
                label = "Indukčnost",
                value = NodeCalculationModel.FormatValue(inductance, ElectricalQuantityType.INDUCTANCE)
            },
            new ComponentPropertyData
            {
                label = "Počet závitů",
                value = numberOfTurns.ToString()
            }
        };
        _componentPropertyPanel.SetData(data, "Cívka");
    }

    protected override void SendData(INodeInteraction node, bool placed = true)
    {
        if (placed)
        {
            node?.SetComponentData(_inductorData, this.gameObject);
        }
        else
        {
            node?.SetComponentData(null, this.gameObject);
        }
    }

    public override void SetData(ComponentControlState controlState, ComplexFloat voltage, ComplexFloat current, ComplexFloat R, bool isDC)
    {
        if(!isDC && controlState == ComponentControlState.ACTIVE)
        {
            // magnetic field
            throw new NotImplementedException();
        }
    }

    public override ComponentData GetComponentData() => _inductorData;

    public override bool CanConnectToNode() => nodeInteraction?.CanConnect(_inductorData.type) ?? false;
}
