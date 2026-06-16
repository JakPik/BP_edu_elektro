/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: NodeCalculationModel
 */

using System;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public enum CircuitValueType
{
    VOLTAGE,
    CURRENT,
    RESISTANCE
}

public static class NodeCalculationModel
{
    private static string GetUnit(CircuitValueType type)
    {
        switch (type)
        {
            case CircuitValueType.VOLTAGE:
                return "V";
            case CircuitValueType.CURRENT:
                return "A";
            case CircuitValueType.RESISTANCE:
                return "Ω";
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

    public static string FormatValue(ComplexFloat _value, CircuitValueType type)
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
}

[System.Serializable]
public struct ComplexFloat
{
    public float real;
    public float imaginary;

    private const float Epsilon = 1e-6f;

    public ComplexFloat(float real, float imaginary)
    {
        this.real = real;
        this.imaginary = imaginary;
    }

    public float GetMagnitude() => (float)Math.Sqrt(real * real + imaginary * imaginary);

    public override string ToString()
    {
        return real.ToString() + " + " + imaginary.ToString() + "i";
    }

    public void Refresh()
    {
        real = 0f;
        imaginary = 0f;
    }

    public static ComplexFloat operator *(ComplexFloat a, ComplexFloat b)
        => new ComplexFloat(a.real * b.real - a.imaginary * b.imaginary, a.real * b.imaginary + a.imaginary * b.real);

    public static ComplexFloat operator /(ComplexFloat a, ComplexFloat b)
    {
        float denominator = b.real * b.real + b.imaginary * b.imaginary;
        float inv = 1f / denominator;
        return new ComplexFloat((a.real * b.real + a.imaginary * b.imaginary) * inv, (a.imaginary * b.real - a.real * b.imaginary) * inv);
    }

    public static ComplexFloat operator +(ComplexFloat a, ComplexFloat b)
        => new ComplexFloat(a.real + b.real, a.imaginary + b.imaginary);

    public static ComplexFloat operator -(ComplexFloat a, ComplexFloat b)
        => new ComplexFloat(a.real - b.real, a.imaginary  - b.imaginary);

    public ComplexFloat Inverse()
    {
        float denominator = real * real + imaginary * imaginary;
        float inv = 1f / denominator;
        return new ComplexFloat(real * inv, -imaginary * inv);
    }

    public override bool Equals(object obj)
    {
        if (obj is not ComplexFloat other)
            return false;

        return
            Math.Abs(real - other.real) < Epsilon &&
            Math.Abs(imaginary - other.imaginary) < Epsilon;
    }

    public override int GetHashCode()
    {
        int r = (int)(real / Epsilon);
        int i = (int)(imaginary / Epsilon);

        return HashCode.Combine(r, i);
    }

    public static bool operator ==(ComplexFloat a, ComplexFloat b)
        => a.Equals(b);

    public static bool operator !=(ComplexFloat a, ComplexFloat b)
        => !a.Equals(b);

    public static bool operator ==(ComplexFloat a, float b)
        => a.GetMagnitude() == b;

    public static bool operator !=(ComplexFloat a, float b)
        => a.GetMagnitude() != b;
}
