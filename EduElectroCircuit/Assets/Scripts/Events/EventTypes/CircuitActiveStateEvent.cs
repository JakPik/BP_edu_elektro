/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: CircuitActiveStateEvent
 */

public class CircuitActiveStateEvent : EventBaseType
{
    public bool circuitActive;
    public NodeCircuit_ID nodeCircuit_ID;

    public CircuitActiveStateEvent(bool circuitActive, NodeCircuit_ID nodeCircuit_ID)
    {
        this.circuitActive = circuitActive;
        this.nodeCircuit_ID = nodeCircuit_ID;
    }

    public override string DisplayData()
    {
        return "(Circuit Active: " + circuitActive.ToString() + ", " + nodeCircuit_ID + ")";
    }
}