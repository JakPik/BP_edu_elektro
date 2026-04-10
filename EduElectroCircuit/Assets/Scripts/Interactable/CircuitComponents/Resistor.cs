using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Resistor : CircuitComponent, IGrabable
{
    [SerializeField] private bool canGrab;
    [SerializeField] private string grabInfo;
    [SerializeField] private ResistorDataSO resistorData;
    [SerializeField] private InteractionDisplay interactionDisplay;
    [SerializeField] private InteractionSO grabInteraction;

    public void Start()
    {
        var rend = GetComponent<Renderer>();
        var propBlock = new MaterialPropertyBlock();
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color_1", resistorData.colorCode[0]);
        propBlock.SetColor("_Color_2", resistorData.colorCode[1]);
        propBlock.SetColor("_Color_3", resistorData.colorCode[2]);
        propBlock.SetColor("_Color_4", resistorData.colorCode[3]);
        propBlock.SetColor("_Color_5", resistorData.colorCode[4]);
        rend.SetPropertyBlock(propBlock);
    }

    public bool CanGrab() => canGrab;
    public void DisplayInfo(bool display) {
        if(canGrab) {
            interactionDisplay.DisplayInteractionInfo(grabInteraction, display);
        }
    }

    public void LockGrab(bool locked)
    {
        canGrab = !locked;
    }

    public void OnGrab(bool grabbed, GameObject caller)
    {
        rb.constraints = RigidbodyConstraints.None;
        rb.angularVelocity = Vector3.zero;
        if (grabbed)
        {
            rb.excludeLayers = 1 << caller.layer;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            rb.excludeLayers = LayerMask.GetMask("Nothig");
            CanAnimate();
        }
        rb.useGravity = !grabbed;
    }

    protected override Quaternion FindTargetRotation(Transform local, Transform target)
    {
        float angle = Vector3.SignedAngle(target.forward, local.forward, target.up);
        
        angle = Math.Abs(angle) > 90f? 180 * Math.Sign(angle) : 0;

        Quaternion baseRot = Quaternion.LookRotation(target.forward);
        Quaternion finalRot = baseRot * Quaternion.AngleAxis(angle, Vector3.up);
        return finalRot;
    }

    protected override void NodeLockedState(bool locked)
    {
        if(locked)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    protected override void CanAnimate()
    {
        if(pendingCoroutine != null)
        {

            if (curCoroutine != null)
            {
                StopCoroutine(curCoroutine);
            }
            NodeLockedState(true);
            curCoroutine = StartCoroutine(pendingCoroutine);
            pendingCoroutine = null;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.gameObject.TryGetComponent(out INodeInteraction nodeInteraction))
        { 
            if(!nodeInteraction.CanConnect()) return;
            this.nodeInteraction = nodeInteraction;
            Logger.Log(this.name, "Pending Coroutine set", LogType.INFO);
            var (targetPos, targetTransfrom) = nodeInteraction.GetTransform();
            pendingCoroutine = AnimateNewPosition(targetPos, targetTransfrom);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if(other.gameObject.TryGetComponent(out INodeInteraction nodeInteraction))
        {
            this.nodeInteraction = null;
            Logger.Log(this.name, "Pending Coroutine remove", LogType.INFO);
            pendingCoroutine = null;
        }
    }

    protected override void SendData()
    {
        nodeInteraction.SetComponentData(resistorData, this.gameObject);
    }
}
