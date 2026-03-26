using UnityEngine;
using UnityEngine.Events;

public abstract class GenericVoidEventChannel: ScriptableObject
{
    public UnityAction OnEventRaised;

    public void RaiseEvent(string callerName)
    {
        Logger.Log(this.GetType().Name, callerName + " raises event " + this.name, LogType.EVENT);
        OnEventRaised?.Invoke();
    }
}