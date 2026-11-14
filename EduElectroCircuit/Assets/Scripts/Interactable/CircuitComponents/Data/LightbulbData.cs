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

    public ComplexFloat Resistance => resistance;
    public ComplexFloat RatedVoltage => ratedVoltage;
    public ComplexFloat MaxVoltage => maxVoltage;

    public LightbulbData(ComplexFloat resistance, ComplexFloat ratedVoltage, ComplexFloat maxVoltage)
    {
        this.resistance = resistance;
        this.ratedVoltage = ratedVoltage;
        this.maxVoltage = maxVoltage;
    }

    public (ComponentControlState, float) GetBrightness(ComplexFloat voltage)
    {
        if(voltage.GetMagnitude() > maxVoltage.GetMagnitude()) return (ComponentControlState.BROKEN, 0);
        float voltageRatio = voltage.GetMagnitude() / ratedVoltage.GetMagnitude();
        return (ComponentControlState.UNCHANGED, voltageRatio > 0 ? (float)Math.Pow(voltageRatio, 3) : 0.0f);
    }
}