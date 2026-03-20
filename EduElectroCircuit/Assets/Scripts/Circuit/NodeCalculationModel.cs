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
        Logger.Log("Ic", "Ic: " + Ic+"\n R top: " + (float)Math.Pow(R[idx], -1) + "\n R total: " + CalculateInvertedResistanceSum(R), Log_Type.WARNING);
        return Ic * ((float)Math.Pow(R[idx], -1)/CalculateInvertedResistanceSum(R));
    }

    private static float CalculateInvertedResistanceSum(float[] R)
    {
        float Rc_ = 0f;
        for(int i =  0; i < R.Length; i++)
        {
            Rc_ += R[i];
        }
        return Rc_;
    }
}
