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