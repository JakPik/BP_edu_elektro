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
        passValues.UpdateReduceValues(U, 0f, R);
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
            Rc_ += (float)Math.Pow(R[i], -1);
        }
        return Rc_;
    }
}
