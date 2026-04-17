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