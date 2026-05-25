/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Resistor
 */

using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Resistor : CircuitComponent, IGrabable
{
    [SerializeField] private bool canGrab;
    [SerializeField] private string grabInfo;
    [SerializeField] public ResistorDataSO resistorData;
    [SerializeField] private InteractionDisplay interactionDisplay;
    [SerializeField] private InteractionSO grabInteraction;

    public void Start()
    {
        var rend = GetComponent<Renderer>();
        var propBlock = new MaterialPropertyBlock();
        propBlock.SetColor("_Color_1", resistorData.colorCode[0]);
        propBlock.SetColor("_Color_2", resistorData.colorCode[1]);
        propBlock.SetColor("_Color_3", resistorData.colorCode[2]);
        propBlock.SetColor("_Color_4", resistorData.colorCode[3]);
        propBlock.SetColor("_Color_5", resistorData.colorCode[4]);
        rend.SetPropertyBlock(propBlock);
    }

    protected override void SendData(bool placed = true)
    {
        if (placed)
        {
            nodeInteraction?.SetComponentData(resistorData, this.gameObject);
        }
        else
        {
            nodeInteraction?.SetComponentData(null, this.gameObject);
        }
    }

    protected override Quaternion FindTargetRotation(Transform local, Transform target)
    {
        float angle = Vector3.SignedAngle(target.forward, local.forward, target.up);

        angle = Math.Abs(angle) > 90f ? 180 * Math.Sign(angle) : 0;

        Quaternion baseRot = Quaternion.LookRotation(target.forward);
        Quaternion finalRot = baseRot * Quaternion.AngleAxis(angle, Vector3.up);
        return finalRot;
    }

    #region IGrabable
    public bool CanGrab() => canGrab;
    public void LockGrab(bool locked) => canGrab = !locked;

    public void OnGrab(bool grabbed, GameObject caller)
    {
        rb.angularVelocity = Vector3.zero;
        if (grabbed)
        {
            GrabControlUtility.SetGrabbedState(rb, caller);
            //  StartCoroutine(GrabControlUtility.AnimateRotationCorrection(this.gameObject, caller.transform));
            SendData(false);
        }
        else
        {
            GrabControlUtility.SetReleasedState(rb);
            if (!nodeInteraction?.CanConnect() ?? true) return;
            Logger.Log(this, "COMPONENT", "Animating new position", LogType.INFO);

            var (targetPos, targetTransfrom) = nodeInteraction.GetTransform();
            CanAnimate(rb, targetPos, targetTransfrom);
        }
    }

    public void DisplayInfo(bool display)
    {
        if (canGrab)
        {
            interactionDisplay.DisplayInteractionInfo(grabInteraction, display);
        }
    }
    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out INodeInteraction nodeInteraction))
        {
            this.nodeInteraction = nodeInteraction;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out INodeInteraction nodeInteraction))
        {
            this.nodeInteraction = null;
            Logger.Log(this, "COMPONENT", "Pending Coroutine remove", LogType.INFO);
        }
    }
}
