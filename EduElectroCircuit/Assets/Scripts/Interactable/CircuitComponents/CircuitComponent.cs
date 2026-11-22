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
    [SerializeField] private bool canGrab;
    protected bool isConnected = false;

    protected Vector3 savedPosition;
    protected Quaternion savedRotation;
    protected INodeInteraction savedNodeInteraction;
    private bool _permanentlyLockedToNode;
    [SerializeField] protected ComponentPropertyPanel _componentPropertyPanel;

    protected abstract void SendData(INodeInteraction node, bool placed = true);
    public virtual void SetData(ComponentControlState controlState, ComplexFloat voltage, ComplexFloat current, ComplexFloat R, bool isDC) { return; }
    public abstract ComponentData GetComponentData();
    protected Quaternion FindTargetRotation(Transform local, Transform target)
    {
        float angle = Vector3.SignedAngle(target.forward, local.forward, target.up);

        angle = Math.Abs(angle) > 90f ? 180 * Math.Sign(angle) : 0;

        Quaternion baseRot = Quaternion.LookRotation(target.forward);
        Quaternion finalRot = baseRot * Quaternion.AngleAxis(angle, Vector3.up);
        return finalRot;
    }

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

    protected void CanAnimate(Vector3 targetPos, Transform targetTransfrom)
    {
        if (curCoroutine != null) StopCoroutine(curCoroutine);

        rb.constraints = RigidbodyConstraints.FreezeAll;
        curCoroutine = StartCoroutine(AnimateNewPosition(targetPos, targetTransfrom));
    }

    public void ForcePermanentNodeConnection(Vector3 targetPos, Transform targetTransfrom, INodeInteraction nodeInteraction)
    {
        _permanentlyLockedToNode = true;
        CanAnimate(targetPos, targetTransfrom);
        this.nodeInteraction = nodeInteraction;
    }
    public INodeInteraction GetIsConnected() => (nodeInteraction == null || !isConnected)? null : nodeInteraction;

    public void SetGrabbedState(Rigidbody rb, GameObject caller)
    {
        if(curCoroutine != null)
        {
            StopCoroutine(curCoroutine);
            curCoroutine = null;
        }
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

    public bool CanGrab() => !_permanentlyLockedToNode && canGrab;
    public void LockGrab(bool locked) => canGrab = !locked;
    public void OnGrab(bool grabbed, GameObject caller)
    {
        rb.angularVelocity = Vector3.zero;
        if (grabbed)
        {
            SetGrabbedState(rb, caller);
            if(isConnected)
            {
                isConnected = false;
                SendData(nodeInteraction, false);
            }
        }
        else
        {
            SetReleasedState(rb);
            if(!CanConnectToNode()) return;

            Logger.Log(this, "COMPONENT", "Animating new position", LogType.INFO);
            SendData(nodeInteraction);
            var (targetPos, targetTransfrom) = nodeInteraction.GetTransform();
            CanAnimate(targetPos, targetTransfrom);
        }
    }
    public abstract bool CanConnectToNode();

    public void SaveCurrentState()
    {
        savedPosition = transform.localPosition;
        savedRotation = transform.localRotation;
        Debug.Log(nodeInteraction);
        savedNodeInteraction = GetIsConnected();
        Debug.Log(savedNodeInteraction);
    }

    public void ReloadComponentState()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.localPosition = savedPosition;
        transform.localRotation = savedRotation;
        if(_permanentlyLockedToNode) return;
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out INodeInteraction nodeInteraction))
        {
            Logger.Log(this, "COMPONENT", "Found Interaction Node: " + other.name, LogType.INFO);
            this.nodeInteraction = nodeInteraction;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out INodeInteraction _))
        {
            this.nodeInteraction = null;
            Logger.Log(this, "COMPONENT", "Pending Coroutine remove", LogType.INFO);
        }
    }
}