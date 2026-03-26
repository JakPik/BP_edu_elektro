public struct CircuitUpdateEvent
{
    public bool CircuitActive;

    public CircuitUpdateEvent(bool circuitActive)
    {
        CircuitActive = circuitActive;
    }
}