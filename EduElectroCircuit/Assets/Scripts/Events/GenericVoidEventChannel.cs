/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: GenericVoidEventChannel
 */

using UnityEngine;
using UnityEngine.Events;

public abstract class GenericVoidEventChannel : ScriptableObject
{
    public UnityAction OnEventRaised;

    public void RaiseEvent(string callerName)
    {
        Logger.LogEvent(this, callerName + " raises event " + this.name);
        OnEventRaised?.Invoke();
    }
}