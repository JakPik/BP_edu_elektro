using System.Reflection;

public class ReloadRequestEvent : EventBaseType
{
    public bool FirstLoad;

    public ReloadRequestEvent(bool firstLoad)
    {
        FirstLoad = firstLoad;
    }

    public override string DisplayData()
    {
        return "(First Load: " + FirstLoad.ToString() + ")";
    }
}