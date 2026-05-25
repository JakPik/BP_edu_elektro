/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: NodeValidationEvent
 */

public class NodeValidationEvent : EventBaseType
{
    public bool Valid;

    public NodeValidationEvent(bool valid)
    {
        Valid = valid;
    }

    public override string DisplayData()
    {
        return "(Valid: " + Valid.ToString() + ")";
    }
}