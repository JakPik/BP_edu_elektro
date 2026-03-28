using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class CircuitComponent : MonoBehaviour
{
    [SerializeField] protected GenericEventChannel<CircuitUpdateEvent> circuitUpdateChannel;
    protected abstract void InteractionLock(CircuitUpdateEvent @event);

    protected void OnEnable() {
        circuitUpdateChannel.OnEventRaised += InteractionLock;
    }

    protected void OnDisable() {
        circuitUpdateChannel.OnEventRaised -= InteractionLock;
    }
}
