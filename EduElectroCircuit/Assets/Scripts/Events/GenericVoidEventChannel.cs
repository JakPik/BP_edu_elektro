using UnityEngine;
using UnityEngine.Events;

public abstract class GenericVoidEventChannel: ScriptableObject
{
    public UnityAction OnEventRaised;

    public void RaiseEvent()
    {
        OnEventRaised?.Invoke();
    }
}