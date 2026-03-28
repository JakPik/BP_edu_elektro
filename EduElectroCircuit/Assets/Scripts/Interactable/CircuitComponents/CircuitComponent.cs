using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class CircuitComponent : MonoBehaviour
{
    [SerializeField] protected Coroutine curCoroutine;
    [SerializeField] protected IEnumerator pendingCoroutine;
    [SerializeField] protected float animationSpeed;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected GenericEventChannel<CircuitUpdateEvent> circuitUpdateChannel;
    [SerializeField] protected INodeInteraction nodeInteraction;
    protected abstract void InteractionLock(CircuitUpdateEvent @event);

    protected void OnEnable() {
        circuitUpdateChannel.OnEventRaised += InteractionLock;
    }

    protected void OnDisable() {
        circuitUpdateChannel.OnEventRaised -= InteractionLock;
    }

    protected IEnumerator AnimateNewPosition(Vector3 targetPosition, Transform targetTransform)
    {
        float totalDistance = Vector3.Distance(transform.position, targetPosition);
        Vector3 origin = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = FindTargetRotation(transform, targetTransform);
        float step;
        float traveled = 0f;
        Logger.Log(this.name, "Animating from "+origin + " animating to "+ targetPosition,LogType.INFO);
        while(transform.position != targetPosition)
        {
            step = Time.deltaTime * animationSpeed;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            traveled += step;

            float t = totalDistance > 0f ? traveled / totalDistance : 1f;
            t = Mathf.Clamp01(t);

            transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }
        transform.position = targetPosition;
        SendData();
        Logger.Log(this.name, "Animating from "+transform.position + " animating to "+ targetPosition,LogType.INFO);
    }

    protected abstract void SendData();
    protected abstract Quaternion FindTargetRotation(Transform local, Transform target);
    protected abstract void CanAnimate();
    protected abstract void NodeLockedState(bool locked);
}
