using System;

[System.Serializable]
public class DoorPanelDTO
{
    public string expectedValueType;
    public ComplexFloat expectedValue;
    public ComplexFloat resistance;

    public DoorPanelDTO(Node_Validation node)
    {
        ValidationNodeData nodeData = node.GetValidationNodeData();
        this.expectedValueType = nodeData.expectedValueType.ToString();
        this.expectedValue = nodeData.expectedValue;
        this.resistance = nodeData.resistance;
    }

    public void DTO_ToValidationNode(Node_Validation node_Validation)
    {
        node_Validation.SetValidationNodeData(new ValidationNodeData
        {
            locked = true,
            resistance = resistance,
            expectedValue = expectedValue,
            expectedValueType = Enum.Parse<ElectricalQuantityType>(expectedValueType)
        });
    }
}