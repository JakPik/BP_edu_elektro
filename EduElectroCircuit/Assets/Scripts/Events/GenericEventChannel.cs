using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class GenericEventChannel<T> : ScriptableObject
{
    public UnityAction<T> OnEventRaised;

    public void RaiseEvent(T parameter, string callerName)
    {
        EventBaseType eventBase = null;
        if (parameter is EventBaseType baseType) eventBase = baseType;

        Logger.LogEvent(this, callerName + " raises event " + this.name + " and sends values " + eventBase?.DisplayData());
        OnEventRaised?.Invoke(parameter);
    }
}