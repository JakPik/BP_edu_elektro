using System;
using System.Collections;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Lightbulb : CircuitComponent
{
    public float bright;
    [Header("Control Data")]
    [SerializeField] private LightbulbData lightbulbData;
    [SerializeField] private float maxBrightness;

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

    private IEnumerator BreakLight()
    {
        yield return new WaitForSeconds(1.0f);
        broken = true;
        isConnected = false;
        LockGrab(false);

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

    public override ComponentData GetComponentData() => lightbulbData;

    public override void SetData(ComponentControlState controlState, ComplexFloat voltage, ComplexFloat current, ComplexFloat R, bool isDC)
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
                if(!broken) StartCoroutine(BreakLight());
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
                label = "Odpor",
                value = NodeCalculationModel.FormatValue(lightbulbData.Resistance, ElectricalQuantityType.RESISTANCE)
            },
            new ComponentPropertyData
            {
                label = "Pracovní Napětí",
                value = NodeCalculationModel.FormatValue(lightbulbData.RatedVoltage, ElectricalQuantityType.VOLTAGE)
            },
            new ComponentPropertyData
            {
                label = "Maximální Napětí",
                value = NodeCalculationModel.FormatValue(lightbulbData.MaxVoltage, ElectricalQuantityType.VOLTAGE)
            }
        };
        _componentPropertyPanel.SetData(data, "Žárovka");
    }
    public LightbulbData GetLightbulbData() => lightbulbData;


  /* protected override void TriggerStay(Collider other, INodeInteraction nodeInteraction)
    {
        if(!nodeInteraction.CanConnect(lightbulbData.type)) return;
        Logger.Log(this, "COMPONENT", "Found Interaction Node: " + other.name, LogType.INFO);
        this.nodeInteraction = nodeInteraction;
    }

    protected override void TriggerExit(Collider other)
    {
        this.nodeInteraction = null;
        Logger.Log(this, "COMPONENT", "Pending Coroutine remove", LogType.INFO);
    }*/

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
        SetData(ComponentControlState.REPAIR, new ComplexFloat(),new ComplexFloat(),new ComplexFloat(), false);
    }

    public override bool CanConnectToNode() => nodeInteraction?.CanConnect(lightbulbData.type) ?? false && !broken;
}
