using System;
using UnityEngine;
using UnityEngine.Events;

public abstract class GenericEventChannel<T>: ScriptableObject
{
    public UnityAction<T> OnEventRaised;

    public void RaiseEvent(T parameter, string callerName)
    {
        Logger.Log(this.GetType().Name, callerName + " raises event " + this.name + " and sends values " + parameter, LogType.EVENT);
        OnEventRaised?.Invoke(parameter);
    }
}