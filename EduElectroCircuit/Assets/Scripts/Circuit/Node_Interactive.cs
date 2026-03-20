using UnityEngine;

public class Node_Interactive: Node
{
    [SerializeField] private bool readVoltage = false;
    [SerializeField] private bool readCurrent = false;
    [SerializeField] private bool readResistance = false;

    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        if(type == Node_Type.NODE_PASSIVE)
        {
            Logger.Log(this.name, "Not implemented interaction with measuring tools", Log_Type.ERROR);
        }
        else
        {
            (U, I) = NodeCalculationModel.CalculateNodeValues(passValues, R);
            Logger.Log(this.name, "U: " + U +"V, I: " + I + " mA, R: " + R, Log_Type.INFO);
        }
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available", Log_Type.WARNING);
            return;
        }
        nextNode.CalculateValues(passValues, originValues);
    }

    public override float GetResistanceSum()
    {
        if(nextNode == null)
        {
            Logger.Log(this.name, "No next node available\n Returning this R: " + R, Log_Type.WARNING);
            return R;
        }
        Logger.Log(this.name, "Local R: " + R, Log_Type.INFO);
        return R + nextNode.GetResistanceSum();
    }
}
