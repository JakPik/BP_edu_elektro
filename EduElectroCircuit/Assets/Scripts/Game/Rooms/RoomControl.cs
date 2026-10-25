using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;
using UnityEngine.Events;
using System.Text.RegularExpressions;

public class RoomControl: MonoBehaviour
{
    [SerializeField] private int minimumOfActivations;
    [SerializeField] private bool isReturnable;
    [SerializeField] private Transform componentsContainer;
    [SerializeField] private Node_Source[] node_Source;
    [SerializeField] private Node_Validation[] node_Validation;
    [Header("Raised Events")]
    [SerializeField] private UnityEvent<bool> onRoomReload;
    [SerializeField] private UnityEvent onRoomReset;
    [SerializeField] private UnityEvent onRoomStateSave;
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveStateEventChannel;

    private Resistor[] resistors;
    private Lightbulb[] lightbulbs;
    private int roomID;
    private EvaluationGrade grade;

    private int _numberOfActivations;
    private int _numberOfReloads;

    private void Awake() {
        roomID = int.Parse(Regex.Match(this.name, @"-?\d+").Value);
    }
    public void LoadComponentSetUp(ComponentsDTO componentsDTO)
    {
        if(componentsDTO == null) {
            Logger.Log(this, "Set up Load", "No data to be loaded!", LogType.WARNING);
        };
        
        circuitActiveStateEventChannel.RaiseEvent(new CircuitActiveStateEvent(false, new NodeCircuit_ID(roomID) ), this.name);

        for(int i = 0; i < componentsDTO.sourcePanelDTOs.Length; i++)
        {
            componentsDTO.sourcePanelDTOs[i].DTO_ToSourceNode(node_Source[i]);
        }
        
        for(int i = 0; i < componentsDTO.doorPanelDTOs.Length; i++)
        {
            componentsDTO.doorPanelDTOs[i].DTO_ToValidationNode(node_Validation[i]);
        }

        resistors = RoomDataManager.LoadComponentSetUp<Resistor, ResistorDTO>(componentsDTO.resistors, componentsContainer);
        lightbulbs = RoomDataManager.LoadComponentSetUp<Lightbulb, LightbulbDTO>(componentsDTO.lightbulbs, componentsContainer);
        
        _numberOfActivations = 0;
        _numberOfReloads = 0;

        onRoomReset.Invoke();
        //onRoomReload.Invoke(false);
    }

    public void ReloadComponets(bool circuitActive)
    {
        circuitActiveStateEventChannel.RaiseEvent(new CircuitActiveStateEvent(false, new NodeCircuit_ID(roomID) ), this.name);
        foreach(Resistor resistor in resistors)
        {
            resistor.ReloadResistorState();            
        }
        foreach(Lightbulb lightbulb in lightbulbs)
        {
            lightbulb.ReloadResistorState();
            lightbulb.SetData(ComponentControlState.REPAIR, new ComplexFloat(),new ComplexFloat());    
        }

        circuitActiveStateEventChannel.RaiseEvent(new CircuitActiveStateEvent(circuitActive, new NodeCircuit_ID(roomID) ), this.name);
        onRoomReload.Invoke(circuitActive);
    }

    public ComponentsDTO SaveComponentSetUp()
    {
        ComponentsDTO components = new ComponentsDTO();

        SourcePanelDTO[] sourcePanelDTOs = new SourcePanelDTO[node_Source.Length];
        for(int i = 0; i < node_Source.Length; i++)
        {
            sourcePanelDTOs[i] = new SourcePanelDTO(node_Source[i]);
        }

        DoorPanelDTO[] doorPanelDTOs = new DoorPanelDTO[node_Validation.Length];
        for(int i = 0; i < node_Validation.Length; i++)
        {
            doorPanelDTOs[i] = new DoorPanelDTO(node_Validation[i]);
        }
        components.doorPanelDTOs = doorPanelDTOs;
        components.sourcePanelDTOs = sourcePanelDTOs;

        RoomDataManager.SaveComponentSetUp(componentsContainer, components);
        return components;
    }

    public void SaveComponents()
    {
        onRoomStateSave.Invoke();
        foreach(Resistor resistor in resistors)
        {
            resistor.SaveCurrentState();            
        }
        foreach(Lightbulb lightbulb in lightbulbs)
        {
            lightbulb.SaveCurrentState();
        }
    }

    public void UpdateGradeValues(int numberOfActivations, int numberOfReloads)
    {
        _numberOfActivations += numberOfActivations;
        _numberOfReloads += numberOfReloads;
        grade = LevelEvaluation.GetRoomGrade(_numberOfActivations, _numberOfReloads, minimumOfActivations);
    }

    public bool GetIsReturnable() => isReturnable;
    public EvaluationGrade GetRoomGrade() => grade;


    #region Debug Inspector Call Functions
    public void LoadSetUp(ComponentsDTO componentsDTO)
    {
        circuitActiveStateEventChannel.RaiseEvent(new CircuitActiveStateEvent(false, new NodeCircuit_ID(roomID) ), this.name);

        resistors = RoomDataManager.LoadComponentSetUp<Resistor, ResistorDTO>(componentsDTO.resistors, componentsContainer);
        lightbulbs = RoomDataManager.LoadComponentSetUp<Lightbulb, LightbulbDTO>(componentsDTO.lightbulbs, componentsContainer);
        
        _numberOfActivations = 0;
        _numberOfReloads = 0;

        onRoomReset.Invoke();
    }

    [ContextMenu("Save Components")]
    private void SaveSetUp()
    {
        SaveComponentSetUp();
    }
    #endregion
}