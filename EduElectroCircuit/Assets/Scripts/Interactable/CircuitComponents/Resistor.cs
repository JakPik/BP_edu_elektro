/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Resistor
 */

using System;
using UnityEngine;

public class Resistor : CircuitComponent
{
    [SerializeField] private bool canGrab;
    [SerializeField] private string grabInfo;
    [SerializeField] private ResistorData resistorData;
    public ComplexFloat GetResistorResistance() => resistorData.GetResistance();
    
    [SerializeField] private InteractionSO grabInteraction;

    private Renderer rend;
    private MaterialPropertyBlock propBlock;

    public void LoadResistor(ComplexFloat resistance)
    {
        resistorData = new ResistorData(resistance);
        SetColorCode();
        SaveCurrentState();
        LockGrab(false);
        ComponentPropertyData[] data =
        {
            new ComponentPropertyData
            {
                label = "Resistance",
                value = NodeCalculationModel.FormatValue(resistance, ElectricalQuantityType.RESISTANCE)
            }
        };
        _componentPropertyPanel.SetData(data, "Resistor");
    }

    private void SetColorCode()
    {
        rend = GetComponent<Renderer>();
        propBlock = new MaterialPropertyBlock();
        Color[] colorCode = resistorData.GetColorCode();
        propBlock.SetColor("_Color_1", colorCode[0]);
        propBlock.SetColor("_Color_2", colorCode[1]);
        propBlock.SetColor("_Color_3", colorCode[2]);
        propBlock.SetColor("_Color_4", colorCode[3]);
        propBlock.SetColor("_Color_5", colorCode[4]);
        rend.SetPropertyBlock(propBlock);
    }

    protected override void SendData(INodeInteraction node, bool placed = true)
    {
        if (placed)
        {
            node?.SetComponentData(resistorData, this.gameObject);
        }
        else
        {
            node?.SetComponentData(null, this.gameObject);
        }
    }

    public override void SetData(ComponentControlState controlState, ComplexFloat voltage, ComplexFloat current)
    {
        return;
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
    public override bool CanGrab() => canGrab;
    public override void LockGrab(bool locked) => canGrab = !locked;

    public override void OnGrab(bool grabbed, GameObject caller)
    {
        rb.angularVelocity = Vector3.zero;
        if (grabbed)
        {
            isConnected = false;
            SetGrabbedState(rb, caller);
            SendData(nodeInteraction, false);
        }
        else
        {
            SetReleasedState(rb);
            if(nodeInteraction == null) return;

            Logger.Log(this, "COMPONENT", "Animating new position", LogType.INFO);
            SendData(nodeInteraction);
            var (targetPos, targetTransfrom) = nodeInteraction.GetTransform();
            CanAnimate(rb, targetPos, targetTransfrom);
        }
    }

    public override void DisplayInfo(bool display)
    {
        if (canGrab)
        {
            //_componentPropertyPanel.DisplayInteractionInfo(grabInteraction, display);
        }
    }
    #endregion

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out INodeInteraction nodeInteraction))
        {
            if(!nodeInteraction.CanConnect(resistorData.type)) return;
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

    public override bool CanConnectToNode() => nodeInteraction != null;
}
