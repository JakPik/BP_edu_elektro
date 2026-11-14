/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Resistor
 */

using System;
using UnityEngine;

public class Resistor : CircuitComponent
{
    [SerializeField] private ResistorData resistorData;
    public ComplexFloat GetResistorResistance() => resistorData.Resistance;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    public void LoadResistor(ComplexFloat resistance)
    {
        resistorData = new ResistorData(resistance);
        SetColorCode();
        SaveCurrentState();
        LockGrab(false);
        ComponentPropertyData[] data =
        {
            new ComponentPropertyData
            {
                label = "Odpor",
                value = NodeCalculationModel.FormatValue(resistance, ElectricalQuantityType.RESISTANCE)
            }
        };
        _componentPropertyPanel.SetData(data, "Rezistor");
    }

    private void SetColorCode()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        Color[] colorCode = resistorData.ColorCode;
        propBlock.SetColor("_Color_1", colorCode[0]);
        propBlock.SetColor("_Color_2", colorCode[1]);
        propBlock.SetColor("_Color_3", colorCode[2]);
        propBlock.SetColor("_Color_4", colorCode[3]);
        propBlock.SetColor("_Color_5", colorCode[4]);
        rend.SetPropertyBlock(propBlock);
    }

    protected override void SendData(INodeInteraction node, bool placed = true)
    {
        if (placed)
        {
            node?.SetComponentData(resistorData, this.gameObject);
        }
        else
        {
            node?.SetComponentData(null, this.gameObject);
        }
    }

    public override ComponentData GetComponentData() => resistorData;

    public override bool CanConnectToNode() => nodeInteraction?.CanConnect(resistorData.type) ?? false;
}
