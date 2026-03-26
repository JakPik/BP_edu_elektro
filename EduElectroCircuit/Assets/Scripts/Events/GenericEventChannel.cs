using UnityEngine;
using UnityEngine.Events;

public abstract class GenericEventChannel<T>: ScriptableObject
{
    public UnityAction<T> OnEventRaised;

    public void RaiseEvent(T parameter)
    {
        OnEventRaised?.Invoke(parameter);
    }
}