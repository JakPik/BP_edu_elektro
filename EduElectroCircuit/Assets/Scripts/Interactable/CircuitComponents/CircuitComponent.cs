/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: CircuitComponent
 */

using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
public abstract class CircuitComponent : MonoBehaviour, IGrabable, IInteractable
{
    [SerializeField] protected Coroutine curCoroutine;
    [SerializeField] protected float animationSpeed;
    [SerializeField] protected Rigidbody rb;
    [SerializeField] protected INodeInteraction nodeInteraction;
    protected bool isConnected = false;

    protected Vector3 savedPosition;
    protected Quaternion savedRotation;
    protected INodeInteraction savedNodeInteraction;
    [SerializeField] protected ComponentPropertyPanel _componentPropertyPanel;

    protected abstract void SendData(INodeInteraction node, bool placed = true);
    public abstract void SetData(ComponentControlState controlState, ComplexFloat voltage, ComplexFloat current);
    protected abstract Quaternion FindTargetRotation(Transform local, Transform target);

    protected IEnumerator AnimateNewPosition(Vector3 targetPosition, Transform targetTransform)
    {
        Logger.Log(this, "INTERACTION", "Animating from " + transform.position + " animating to " + targetPosition, LogType.INFO);
        
        yield return AnimatingPosition(
            transform.rotation,
            FindTargetRotation(transform, targetTransform),
            targetPosition,
            Vector3.Distance(transform.position, targetPosition));
        
        isConnected = true;
    }

    private IEnumerator AnimatingPosition(Quaternion startRot, Quaternion targetRot, Vector3 targetPosition, float totalDistance)
    {
        Rigidbody rigidBody = GetComponent<Rigidbody>();
        float step;
        float traveled = 0f;
        while (transform.position != targetPosition)
        {
            step = Time.deltaTime * animationSpeed;
            rigidBody.transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            traveled += step;

            float t = totalDistance > 0f ? traveled / totalDistance : 1f;
            t = Mathf.Clamp01(t);

            rigidBody.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);
            yield return null;
        }

        rigidBody.transform.position = targetPosition;
    }

    protected void CanAnimate(Rigidbody rb, Vector3 targetPos, Transform targetTransfrom)
    {
        if (curCoroutine != null) StopCoroutine(curCoroutine);

        rb.constraints = RigidbodyConstraints.FreezeAll;
        curCoroutine = StartCoroutine(AnimateNewPosition(targetPos, targetTransfrom));
    }
    public INodeInteraction GetIsConnected() => (nodeInteraction == null || !isConnected)? null : nodeInteraction;

    public void SetGrabbedState(Rigidbody rb, GameObject caller)
    {
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.excludeLayers = 1 << caller.layer;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.useGravity = false;
    }

    protected void SetReleasedState(Rigidbody rb)
    {
        rb.interpolation = RigidbodyInterpolation.None;
        rb.excludeLayers = LayerMask.GetMask("Nothing");
        rb.constraints = RigidbodyConstraints.None;
        rb.useGravity = true;
    }

    public abstract bool CanGrab();
    public abstract void LockGrab(bool locked);
    public abstract void OnGrab(bool grabbed, GameObject caller);
    public abstract void DisplayInfo(bool display);
    public abstract bool CanConnectToNode();

    public void SaveCurrentState()
    {
        savedPosition = transform.localPosition;
        savedRotation = transform.localRotation;
        Debug.Log(nodeInteraction);
        savedNodeInteraction = GetIsConnected();
        Debug.Log(savedNodeInteraction);
    }

    public void ReloadResistorState()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = savedPosition;
        transform.localRotation = savedRotation;

        if(savedNodeInteraction != null)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
            LockGrab(true);
            InitConnection(savedNodeInteraction);
            nodeInteraction = savedNodeInteraction;
            isConnected = true;
        }
        else
        {
            rb.constraints = RigidbodyConstraints.None;
            LockGrab(false);
            nodeInteraction = null;
            isConnected = false;
        }
    }

    private void InitConnection(INodeInteraction node)
    {
        SendData(node);
    }

    #region IInteractable
    public void OnInteract(bool resetInteraction = false)
    {
        if(resetInteraction)
        {
            _componentPropertyPanel.ResetInteraction();
        }
        else
        {
            _componentPropertyPanel.Interact();
        }
    }
    public bool CanInteract() => true;
    public void DisplayInfo(bool display, RaycastHit hit = default, Camera camera = null)
    {
        
    }
    #endregion
}