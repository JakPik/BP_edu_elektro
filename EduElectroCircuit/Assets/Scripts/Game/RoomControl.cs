using System.IO;
using UnityEngine;
using UnityEngine.Events;

public class RoomControl : MonoBehaviour
{
    [Header("Set-up data")]
    [SerializeField] private TextAsset jsonFile;

    [Header("Raised Events")]
    [SerializeField] private UnityEvent onRoomReload;
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveStateEventChannel;
    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;
    [SerializeField] private GameObject spawnPointReference;
    private void LoadComponentSetUp()
    {
        if (jsonFile == null)
        {
            Logger.Log(this, this.name, "Set up file not found", LogType.ERROR);
            return;
        }
        string json = jsonFile.text;
        Resistors data = JsonUtility.FromJson<Resistors>(json);
        Transform componentsParent = transform.Find("Components");
        foreach (var resistor in data.resistors)
        {
            GameObject obj = componentsParent.Find(resistor.name)?.gameObject;
            if (obj == null)
            {
                Logger.Log(this, this.name, "Resistor " + resistor.name + " not found in scene\nCreating new resistor: " + resistor.name, LogType.WARNING);
                GameObject resistorPrefab = Resources.Load<GameObject>("Prefabs/Resistor");
                obj = Instantiate(resistorPrefab, componentsParent.position, componentsParent.rotation, componentsParent);
                obj.name = resistor.name;
                Resistor resistorComponent = obj.GetComponent<Resistor>();
                resistorComponent.resistorData = Resources.Load<ResistorDataSO>("ScriptableObjects/Component_properties/" + resistor.resistance);
            }
            obj.transform.position = resistor.position + componentsParent.position;
            obj.transform.rotation = componentsParent.rotation;
            Resistor resistorComp = obj.GetComponent<Resistor>();
            if (resistorComp == null)
            {
                Logger.Log(this, this.name, "Resistor component not found on " + obj.name, LogType.ERROR);
                continue;
            }
            resistorComp.LockGrab(false);
        }

    }

    private void SaveComponentSetUp()
    {
        Transform componentsParent = transform.Find("Components");
        Resistors data = new Resistors();
        data.resistors = new ResistorData[componentsParent.childCount];
        for (int i = 0; i < componentsParent.childCount; i++)
        {
            Transform child = componentsParent.GetChild(i);
            Resistor resistorComponent = child.GetComponent<Resistor>();
            if (resistorComponent != null)
            {
                data.resistors[i] = new ResistorData
                {
                    name = child.name,
                    position = child.localPosition,
                    resistance = resistorComponent.resistorData.name
                };
            }
        }
        string json = JsonUtility.ToJson(data, true);
        if (jsonFile != null)
        {
            File.WriteAllText(Application.dataPath + "/Resources/Data/" + jsonFile.name + ".json", json);
        }
        else
        {
            File.WriteAllText(Application.dataPath + "/Resources/Data/ComponentSetUp.json", json);
        }
        Logger.Log(this, this.name, "Component set up saved to file", LogType.INFO);
    }

    #region Event Handling Logic
    void OnEnable()
    {
        reload.OnEventRaised += OnRoomReload;
    }

    void OnDisable()
    {
        reload.OnEventRaised -= OnRoomReload;
    }

    void OnRoomReload(ReloadEvent @event)
    {
        if (@event.SpawnReference != null && @event.SpawnReference != spawnPointReference) return;
        onRoomReload?.Invoke();
        circuitActiveStateEventChannel.RaiseEvent(new CircuitActiveStateEvent(false), this.name);
        LoadComponentSetUp();
    }
    #endregion

    #region Debug Inspector Call Functions

    [ContextMenu("Load Components")]
    private void LoadSetUp()
    {
        LoadComponentSetUp();
    }

    [ContextMenu("Save Component SetUp")]
    private void SaveSetUp()
    {
        SaveComponentSetUp();
    }
    #endregion
}

[System.Serializable]
public class Resistors
{
    public ResistorData[] resistors;
}

[System.Serializable]
public class ResistorData
{
    public string name;
    public Vector3 position;
    public string resistance;
}
