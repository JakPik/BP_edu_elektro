/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: NodeCalculationModel
 */

using System;
using NUnit.Framework;
using UnityEngine;

public static class NodeCalculationModel
{
    private const float PI_2 = 6.25f;
    private static string GetUnit(ElectricalQuantityType type)
    {
        switch (type)
        {
            case ElectricalQuantityType.VOLTAGE:
                return "V";
            case ElectricalQuantityType.CURRENT:
                return "A";
            case ElectricalQuantityType.RESISTANCE:
                return "Ω";
            case ElectricalQuantityType.CAPACITANCE:
                return "F";
            case ElectricalQuantityType.INDUCTANCE:
                return "H";
            case ElectricalQuantityType.FREQUENCY:
                return "Hz";
            default:
                return "";
        }
    }

    public static (ComplexFloat, ComplexFloat) CalculateNodeValues(NodeDataModel passValues, ComplexFloat R)
    {
        ComplexFloat U = passValues.Uc * (R / passValues.Rc);
        ComplexFloat I = passValues.Ic;
        passValues.UpdateReduceValues(U, R);
        return (U, I);
    }

    public static ComplexFloat CalculateCurrentDivider(ComplexFloat Ic, ComplexFloat[] R, int idx)
    {
        if(R[idx].infinite) return new ComplexFloat();
        ComplexFloat R_Branch = R[idx].Inverse();
        ComplexFloat R_Total = CalculateInvertedResistanceSum(R);
        ComplexFloat result = Ic * (R_Branch / R_Total);
        Logger.LogCalculation("Current divider", "Divider for branch " + idx,
        "Current In: " + Ic + ", R-1 for branch: " + R_Branch + ", R-1 total: " + R_Total,
        "Result:" + result);
        return result;
    }

    private static ComplexFloat CalculateInvertedResistanceSum(ComplexFloat[] R)
    {
        ComplexFloat Rc_ = new ComplexFloat();
        for (int i = 0; i < R.Length; i++)
        {
            if (R[i] == 0) continue;
            Rc_ += R[i].Inverse();
        }
        return Rc_;
    }

    public static ComplexFloat CalculateShortCircuitCurrentSplit(ComplexFloat[] branch_R, bool[] branchConnected, ComplexFloat I)
    {
        int count = 0;
        for (int i = 0; i < branch_R.Length; i++)
        {
            if(!branch_R[i].infinite && branch_R[i].GetMagnitude() == 0 && branchConnected[i]) count++;
        }
        return I / count;
    }

    public static (bool, ComplexFloat, ComplexFloat[]) CalculateBranchResistance(Node[] nextBranchNode, bool[] branch_connected, SignalType signalType, float frequency)
    {
        ComplexFloat[] branch_R = new ComplexFloat[nextBranchNode.Length];
        ComplexFloat R = new ComplexFloat();
        bool shortCircuited = false;
        for (int i = 0; i < nextBranchNode.Length; i++)
        {
            if (nextBranchNode[i] == null)
            {
                Logger.LogNode("Branch Resistance Calculation", "Branch " + i + " is empty", LogType.EXCEPTION);
                continue;
            }
            if(!branch_connected[i]) continue;

            branch_R[i] = nextBranchNode[i].GetResistanceSum(signalType, frequency);
            if(branch_R[i].infinite) continue;
            if(branch_R[i] == 0)
            {
                if(!shortCircuited) shortCircuited = true;
                continue;
            }
            R += branch_R[i].Inverse();
        }
        if(shortCircuited)
        {
            R = new ComplexFloat();
        }
        return (shortCircuited, R.Inverse(), branch_R);
    }

    public static string FormatValue(ComplexFloat _value, ElectricalQuantityType type)
    {
        float value = _value.GetMagnitude();
        if (value >= 1e6)
        {
            return (value / 1e6).ToString("F2") + " M" + GetUnit(type);
        }
        else if (value >= 1e3)
        {
            return (value / 1e3).ToString("F2") + " k" + GetUnit(type);
        }
        else if (value >= 1)
        {
            return value.ToString("F2") + " " + GetUnit(type);
        }
        else if (value >= 1e-3)
        {
            return (value * 1e3).ToString("F2") + " m" + GetUnit(type);
        }
        else if (value == 0 || value <= 1e-10)
        {
            return value.ToString("F2") + " " + GetUnit(type);
        }
        else
        {
            return (value * 1e6).ToString("F4") + " μ" + GetUnit(type);
        }
    }

    public static string FormatValue(float _value, ElectricalQuantityType type)
    {
        float value = _value;
        if (value >= 1e6)
        {
            return (value / 1e6).ToString("F2") + " M" + GetUnit(type);
        }
        else if (value >= 1e3)
        {
            return (value / 1e3).ToString("F2") + " k" + GetUnit(type);
        }
        else if (value >= 1)
        {
            return value.ToString("F2") + " " + GetUnit(type);
        }
        else if (value >= 1e-3)
        {
            return (value * 1e3).ToString("F2") + " m" + GetUnit(type);
        }
        else if (value == 0 || value <= 1e-10)
        {
            return value.ToString("F2") + " " + GetUnit(type);
        }
        else
        {
            return (value * 1e6).ToString("F4") + " μ" + GetUnit(type);
        }
    }

    public static ComplexFloat GetImpedance(ComponentData componentData, float frequency, bool isDC)
    {
        switch(componentData.type)
        {
            case ComponentType.RESISTOR:
                return ((ResistorData)componentData).Resistance;
            case ComponentType.LIGHTBULB:
                return ((LightbulbData)componentData).Resistance;
            case ComponentType.CAPACITOR:
                return isDC? ((CapacitorData)componentData).ChargingRezistanceChange: CalculateCapacitiveImpedance(frequency, ((CapacitorData)componentData).Capacitance);
            case ComponentType.INDUCTOR:
                return isDC? new ComplexFloat() : CalculateInductiveImpedance(frequency, ((InductorData)componentData).Inductance);
            default:
                return new ComplexFloat();
        }
    }

    private static ComplexFloat CalculateInductiveImpedance(float frequency, float inductance)
    {
        return new ComplexFloat(0, PI_2 * frequency * inductance);
    }

    private static ComplexFloat CalculateCapacitiveImpedance(float frequency, float capacitance)
    {
        return new ComplexFloat(0, -Mathf.Pow(PI_2 * frequency * capacitance, -1));
    }
}