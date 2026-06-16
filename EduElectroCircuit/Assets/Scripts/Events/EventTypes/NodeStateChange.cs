/*
 * Edukativní hra zamìøená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: NodeStateChange
 */

public class NodeStateChange : EventBaseType
{
    public NodeCircuit_ID nodeCircuit_ID;

    public NodeStateChange(NodeCircuit_ID nodeCircuit_ID)
    {
        this.nodeCircuit_ID = nodeCircuit_ID;
    }

    public override string DisplayData()
    {
        return "(Room_circuit ID: " + nodeCircuit_ID.roomID + "_" + nodeCircuit_ID.circuitID + ")";
    }
}

[System.Serializable]
public struct NodeCircuit_ID
{
    public int roomID;
    public int circuitID;

    public NodeCircuit_ID(int roomID, int circuitID)
    {
        this.roomID = roomID;
        this.circuitID = circuitID;
    }
}