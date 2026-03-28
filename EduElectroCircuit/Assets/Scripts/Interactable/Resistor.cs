using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Resistor : CircuitComponent, IGrabable
{
    [SerializeField] private bool canGrab;
    [SerializeField] private string grabInfo;

    public bool CanGrab() => canGrab;
    public string GetGrabInfo() => grabInfo;

    public void OnGrab(bool grabbed, GameObject caller)
    {
        this.TryGetComponent(out Rigidbody rb);
        if(rb == null) throw new Exception("Rigidbody is null");
        rb.constraints = RigidbodyConstraints.None;
        rb.angularVelocity = Vector3.zero;
        if (grabbed)
        {
            rb.excludeLayers = 1 << caller.layer;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }
        else
        {
            rb.excludeLayers = LayerMask.GetMask("Nothig");
        }
        rb.useGravity = !grabbed;
    }

    protected override void InteractionLock(CircuitUpdateEvent @event)
    {
        canGrab = !@event.CircuitActive;
    }
}
