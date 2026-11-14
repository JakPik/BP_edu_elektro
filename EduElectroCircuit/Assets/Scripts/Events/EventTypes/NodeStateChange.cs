/*
 * Edukativn� hra zam��en� na elektrick� obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: NodeStateChange
 */

public class NodeStateChange : EventBaseType
{
    public NodeCircuit_ID nodeCircuit_ID;
    public bool valueUpdate;

    public NodeStateChange(NodeCircuit_ID nodeCircuit_ID, bool valueUpdate)
    {
        this.nodeCircuit_ID = nodeCircuit_ID;
        this.valueUpdate = valueUpdate;
    }

    public override string DisplayData()
    {
        return "(Room_circuit ID: " + nodeCircuit_ID.roomID + "_" + nodeCircuit_ID.circuitID + ")";
    }
}