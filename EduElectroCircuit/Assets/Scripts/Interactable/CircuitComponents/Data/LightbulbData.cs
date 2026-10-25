/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: LightbulbDataSO
 */

using System;
using System.Numerics;
using UnityEngine;

[Serializable]
public class LightbulbData : ComponentData
{
    [SerializeField] private ComplexFloat resistance;
    [SerializeField] private ComplexFloat ratedVoltage;
    [SerializeField] private ComplexFloat maxVoltage;

    public LightbulbData(ComplexFloat resistance, ComplexFloat ratedVoltage, ComplexFloat maxVoltage)
    {
        this.resistance = resistance;
        this.ratedVoltage = ratedVoltage;
        this.maxVoltage = maxVoltage;
    }

    public (ComponentControlState, float) GetBrightness(ComplexFloat voltage)
    {
        if(voltage.GetMagnitude() > maxVoltage.GetMagnitude()) return (ComponentControlState.BROKEN, 0);
        float amplitude = voltage.GetMagnitude() / ratedVoltage.GetMagnitude();
        return (ComponentControlState.UNCHANGED, amplitude > 0 ? (float)Math.Pow(amplitude, 3) : 0.0f);
    }

    public ComplexFloat GetResistance() => resistance;

    public ComplexFloat GetRatedVoltage() => ratedVoltage;

    public ComplexFloat GetMaxVoltage() => maxVoltage;
}