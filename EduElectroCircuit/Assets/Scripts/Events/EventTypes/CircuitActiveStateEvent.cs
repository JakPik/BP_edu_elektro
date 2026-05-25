/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: CircuitActiveStateEvent
 */

public class CircuitActiveStateEvent : EventBaseType
{
    public bool CircuitActive;

    public CircuitActiveStateEvent(bool circuitActive)
    {
        CircuitActive = circuitActive;
    }

    public override string DisplayData()
    {
        return "(Circuit Active: " + CircuitActive.ToString() + ")";
    }
}