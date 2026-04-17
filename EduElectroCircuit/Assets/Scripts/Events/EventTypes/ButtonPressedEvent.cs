public class ButtonPressedEvent : EventBaseType
{
    public bool IsON;

    public ButtonPressedEvent(bool isON)
    {
        IsON = isON;
    }

    public override string DisplayData()
    {
        return "(Is ON: " + IsON.ToString() + ")";
    }
}