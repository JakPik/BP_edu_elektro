using System;
using UnityEngine;

public class Lightbulb : CircuitComponent
{
    public float bright;
    [Header("Control Data")]
    [SerializeField] private bool canGrab;
    [SerializeField] private LightbulbData lightbulbData;
    [SerializeField] private float maxBrightness;

    [Header("Interaction set up")]
    [SerializeField] private InteractionSO grabInteraction;

    [Header("Refference set up")]
    [SerializeField] private Light pointLight;
    [SerializeField] private Mesh solid_base;
    [SerializeField] private Mesh broken_base;
    [SerializeField] private GameObject glassSolid;
    [SerializeField] private GameObject glassBroken;
    [SerializeField] private Renderer rend;
    [SerializeField] private MeshFilter meshFilter;

    [Header("Collor States")]
    [SerializeField] private Color OFF;
    [SerializeField] private Color ON_33P;
    [SerializeField] private Color ON_66P;
    [SerializeField] private Color ON_MAX;

    private Vector3[] shardPositions;
    private Quaternion[] shardRotations;
    private MaterialPropertyBlock propertyBlock;
    private bool broken = false;

    public void Start()
    {
        propertyBlock = new MaterialPropertyBlock();
        RepairLight();

        int numberOfChildren = glassBroken.transform.childCount;
        shardPositions = new Vector3[numberOfChildren];
        shardRotations = new Quaternion[numberOfChildren];
        for (int i = 0; i < numberOfChildren; i++)
        {
            Transform obj = glassBroken.transform.GetChild(i);
            shardPositions[i] = obj.localPosition;
            shardRotations[i] = obj.localRotation;
        }
    }

    private void RepairLight()
    {
        broken = false;

        glassBroken.SetActive(false);
        glassSolid.SetActive(true);

        TurnLightOFF();

        meshFilter.mesh = solid_base;
    }

    private void BreakLight()
    {
        broken = true;
        isConnected = false;
        canGrab = true;

        glassBroken.SetActive(true);
        for (int i = 0; i < glassBroken.transform.childCount; i++)
        {
            Transform obj = glassBroken.transform.GetChild(i);
            obj.localPosition = shardPositions[i];
            obj.localRotation = shardRotations[i];
        }
        glassSolid.SetActive(false);
        
        TurnLightOFF();

        meshFilter.mesh = broken_base;
        
        SendData(nodeInteraction, false);
    }

    private void TurnLightOFF()
    {
        pointLight.intensity = 0f;
        propertyBlock.SetFloat("_Intensity", 0.0f);
        propertyBlock.SetColor("_Color", OFF);
        rend.SetPropertyBlock(propertyBlock);
    }

    private void SetLightColor(float brightness)
    {
        float intensity = brightness * maxBrightness;

        if(brightness > 0.66f)
        {
            propertyBlock.SetColor("_Color", ON_MAX);
            pointLight.color = ON_MAX;
        }
        else if(brightness > 0.33f)
        {
            propertyBlock.SetColor("_Color", ON_66P);
            pointLight.color = ON_66P;
        }
        else if(brightness > 0.0f)
        {
            propertyBlock.SetColor("_Color", ON_33P);
            pointLight.color = ON_33P;
        }
        else
        {
            TurnLightOFF();
        }

        pointLight.intensity = brightness * 3f;
        propertyBlock.SetFloat("_Intensity", intensity);
        rend.SetPropertyBlock(propertyBlock);
    }
    
    protected override void SendData(INodeInteraction node, bool placed = true)
    {
        if (placed)
        {
            node?.SetComponentData(lightbulbData, this.gameObject);
        }
        else
        {
            node?.SetComponentData(null, this.gameObject);
        }
    }

    public override void SetData(ComponentControlState controlState, ComplexFloat voltage, ComplexFloat current)
    {
        var (state ,brightness)= lightbulbData.GetBrightness(voltage);
        if(state != ComponentControlState.UNCHANGED)
        {
            controlState = state;
        }

        switch(controlState)
        {
            case ComponentControlState.ACTIVE:
                SetLightColor(brightness);
            break;
            case ComponentControlState.INACTIVE:
                TurnLightOFF();
            break;
            case ComponentControlState.REPAIR:
                RepairLight();
            break;
            case ComponentControlState.BROKEN:
                BreakLight();
            break;
        }
    }

    public void LoadLightbulb(LightbulbData lightbulbData)
    {
        this.lightbulbData = lightbulbData;
        ComponentPropertyData[] data =
        {
            new ComponentPropertyData
            {
                label = "Resistance",
                value = NodeCalculationModel.FormatValue(lightbulbData.GetResistance(), ElectricalQuantityType.RESISTANCE)
            },
            new ComponentPropertyData
            {
                label = "Rated Voltage",
                value = NodeCalculationModel.FormatValue(lightbulbData.GetRatedVoltage(), ElectricalQuantityType.VOLTAGE)
            },
            new ComponentPropertyData
            {
                label = "Max Voltage",
                value = NodeCalculationModel.FormatValue(lightbulbData.GetMaxVoltage(), ElectricalQuantityType.VOLTAGE)
            }
        };
        _componentPropertyPanel.SetData(data, "Lightbulb");
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
            if(nodeInteraction == null || broken) return;

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
            //interactionDisplay.DisplayInteractionInfo(grabInteraction, display);
        }
    }

    public LightbulbData GetLightbulbData() => lightbulbData;
    #endregion


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out INodeInteraction nodeInteraction))
        {
            if(!nodeInteraction.CanConnect(lightbulbData.type)) return;
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

    [ContextMenu("Test Brightness")]
    private void LoadSetUp()
    {
        propertyBlock = new MaterialPropertyBlock();
        SetLightColor(bright);
    }

    [ContextMenu("Repair")]
    private void RepairContextMenu()
    {
        propertyBlock = new MaterialPropertyBlock();
        SetData(ComponentControlState.REPAIR, new ComplexFloat(),new ComplexFloat());
    }

    public override bool CanConnectToNode() => nodeInteraction != null && !broken;
}
