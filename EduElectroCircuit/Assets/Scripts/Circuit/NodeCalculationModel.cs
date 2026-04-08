using System;
using NUnit.Framework;
using UnityEngine;

public static class NodeCalculationModel
{
    public static (float, float) CalculateNodeValues(NodeDataModel passValues, float R)
    {
        float U = 0f;
        float I = 0f;

        U = passValues.Uc * (R / passValues.Rc);
        I = passValues.Ic;
        passValues.UpdateReduceValues(U, R);
        return (U,I);        
    }

    public static float CalculateCurrentDivider(float Ic, float[] R, int idx)
    {
        float R_Branch = (float)Math.Pow(R[idx], -1);
        float R_Total = CalculateInvertedResistanceSum(R);
        float result = Ic * (R_Branch/R_Total);
        Logger.LogCalculation("Current divider", "Divider for branch " + idx,
        "Current In: " + Ic+", R-1 for branch: " + R_Branch + ", R-1 total: " + R_Total,
        "Result:" + result);
        return result;
    }

    private static float CalculateInvertedResistanceSum(float[] R)
    {
        float Rc_ = 0f;
        for(int i =  0; i < R.Length; i++)
        {
            if(R[i] == 0) continue;
            Rc_ += (float)Math.Pow(R[i], -1);
        }
        return Rc_;
    }

    public static string FormatValue(float value)
    {
        if(value >= 1e6)
        {
            return (value / 1e6).ToString("F2") + " M";
        }
        else if(value >= 1e3)
        {
            return (value / 1e3).ToString("F2") + " k";
        }
        else if(value >= 1)
        {
            return value.ToString("F2") + " ";
        }
        else if(value >= 1e-3)
        {
            return (value * 1e3).ToString("F2") + " m";
        }
        else if(value == 0 || value <= 1e-10)
        {
            return value.ToString("F2") + " ";
        }
        else
        {
            return (value * 1e6).ToString("F4") + " μ";
        }
    }
}
