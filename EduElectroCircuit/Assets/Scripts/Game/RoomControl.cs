using System.IO;
using Mono.Cecil;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.Events;

public class RoomControl : MonoBehaviour
{
    [SerializeField] private GenericVoidEventChannel roomReloadEventChannel;
    [SerializeField] private UnityEvent onRoomReload;

    void OnRoomReload()
    {
        onRoomReload?.Invoke();
        GameObject prefab = Resources.Load<GameObject>("meters");
        Logger.Log(this.name, "Reloading room, instantiating prefab: " + (prefab == null), LogType.INFO);
        ComponentSetUp();
    }

    private void ComponentSetUp()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Data/S1_R1_setUp");
        if (textAsset == null)
        {
            Logger.Log(this.name, "Set up file not found", LogType.ERROR);
            return;
        }
        string json = textAsset.text;
        Resistors data = JsonUtility.FromJson<Resistors>(json);
        Transform componentsParent = transform.Find("Components");
        foreach (var resistor in data.resistors)
        {
            GameObject obj = componentsParent.Find(resistor.name)?.gameObject;
            if (obj == null)
            {
                Logger.Log(this.name, "Resistor " + resistor.name + " not found in scene", LogType.WARNING);
                Logger.Log(this.name, "Creating new resistor: " + resistor.name, LogType.INFO);
                GameObject resistorPrefab = Resources.Load<GameObject>("Prefabs/Resistor");
                obj = Instantiate(resistorPrefab, componentsParent.position, componentsParent.rotation, componentsParent);
                obj.name = resistor.name;
                Resistor resistorComponent = obj.GetComponent<Resistor>();
                resistorComponent.resistorData = Resources.Load<ResistorDataSO>("ScriptableObjects/Component_properties/" + resistor.resistance);
            }
            obj.transform.position = resistor.position;
            obj.transform.rotation = componentsParent.rotation;
        }

    }

    [ContextMenu("Save Component SetUp")]
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
                    position = child.position,
                    resistance = resistorComponent.resistorData.name
                };
            }
        }
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Application.dataPath + "/Resources/Data/S1_R1_setUp.json", json);
        Logger.Log(this.name, "Component set up saved to file", LogType.INFO);
    }

    void OnEnable()
    {
        roomReloadEventChannel.OnEventRaised += OnRoomReload;
    }

    void OnDisable()
    {
        roomReloadEventChannel.OnEventRaised -= OnRoomReload;
    }
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
