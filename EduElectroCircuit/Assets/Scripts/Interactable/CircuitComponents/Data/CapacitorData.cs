/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: CapacitorData
 */
using UnityEngine;

[System.Serializable]
public class CapacitorData : ComponentData
{
    [SerializeField] private float capacitance;
    public ComplexFloat ChargingRezistanceChange;
    public float Capacitance => capacitance;

    public CapacitorData(float capacitance)
    {
        this.capacitance = capacitance;
        ChargingRezistanceChange = new ComplexFloat();
        type = ComponentType.CAPACITOR;
    }

    public void UpdatChargingResistance(ComplexFloat value, bool isInfinite)
    {
        ChargingRezistanceChange = value;
        ChargingRezistanceChange.SetNumberAsInf(isInfinite);
    }
}