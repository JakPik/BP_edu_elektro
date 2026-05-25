/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: ReloadRequestEvent
 */

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