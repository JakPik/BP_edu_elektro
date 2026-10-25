using System;

[System.Serializable]
public class SourcePanelDTO
{
    public string sourceType;
    public string signalType;
    public ComplexFloat sourceValue;

    public SourcePanelDTO(Node_Source node)
    {
        SourceData sourceData = node.GetSourceData();
        sourceType = sourceData.sourceType.ToString();
        signalType = sourceData.signalType.ToString();
        sourceValue = sourceData.sourceValue;
    }

    public void DTO_ToSourceNode(Node_Source node_Source)
    {
        node_Source.SetSourceData(new SourceData{
            sourceValue = sourceValue,
            sourceType = Enum.Parse<ElectricalQuantityType>(sourceType),
            signalType = Enum.Parse<SignalType>(signalType)
        });
    }
}