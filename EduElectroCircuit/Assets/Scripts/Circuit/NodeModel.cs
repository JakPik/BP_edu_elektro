using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

public static class NodeModel
{

    public static NodeDTOBuilder nodeDTOs;
    public static void SaveModel(Node_Source node)
    {
        nodeDTOs = new NodeDTOBuilder(new Node_SourceDTO(node));
        ReadTree(node.GetNextNode());
        var test = nodeDTOs;
        //
        string json = JsonUtility.ToJson(nodeDTOs, true);
        File.WriteAllText(Application.dataPath + "/Resources/Data/NodeTree.json", json);
    }

    public static void ReadTree(Node node)
    {
        Node curNode = node;
        while (curNode != null)
        {
            switch(curNode.GetNodeType())
            {
                case NodeType.INTERACTIVE: nodeDTOs.node_InteractiveDTOs.Add(new Node_InteractiveDTO(curNode as Node_Interactive)); break;
                case NodeType.VALIDATION: nodeDTOs.node_ValidationDTOs.Add(new Node_ValidationDTO(curNode as Node_Validation)); break;
                case NodeType.MEASUREMENT: nodeDTOs.node_MeasurementDTOs.Add(new Node_MeasurementDTO(curNode as Node_Measurement)); break;
                case NodeType.BRANCH_OUT: nodeDTOs.node_BranchOutDTOs.Add(new Node_BranchOutDTO(curNode as Node_Branch)); break;
                case NodeType.BRANCH_IN: nodeDTOs.node_BranchInDTOs.Add(new Node_BranchInDTO(curNode as Node_Branch)); break;
                case NodeType.GND: nodeDTOs.node_GND_DTOs.Add(new Node_GND_DTO(curNode as Node_GND)); break;
                default: Logger.Log(curNode, "SaveModel", "Not a valid Node type for: ", LogType.WARNING); break;
            }
            curNode = curNode.GetNextNode();
        }
    }
    
    public static void LoadModel(GameObject root, string modelPath)
    {
        TextAsset jsonFile = Resources.Load<TextAsset>("Data/" + modelPath);
        string json = jsonFile.text;
        NodeDTOBuilder nodeDTO = JsonUtility.FromJson<NodeDTOBuilder>(json);
        Dictionary<string, (Node, string)> nodeRefferences = new Dictionary<string, (Node, string)>();
        Transform container = root.transform.Find("Node");
        var (node, gameObject) = nodeDTO.node_SourceDTOs.DTO_to_Node();
        nodeRefferences[gameObject.name] = (node, nodeDTO.node_SourceDTOs.nextNodeID);
        gameObject.transform.parent = container;
        LoadInteractiveDTOs(nodeRefferences, container, nodeDTO.node_InteractiveDTOs);
        LoadValidationDTOs(nodeRefferences, container, nodeDTO.node_ValidationDTOs);
        LoadMeasurementDTOs(nodeRefferences, container, nodeDTO.node_MeasurementDTOs);
        LoadBranchInDTOs(nodeRefferences, container, nodeDTO.node_BranchInDTOs);
        LoadGND_DTOs(nodeRefferences, container, nodeDTO.node_GND_DTOs);
        LoadBranchOutDTOs(nodeRefferences, container, nodeDTO.node_BranchOutDTOs);
        foreach(var value in nodeRefferences.Values)
        {
            if(!value.Item2.Contains("null"))
            {
                value.Item1.SetNextNode(nodeRefferences[value.Item2].Item1);
            }
        }
    }

    private static void LoadInteractiveDTOs(Dictionary<string, (Node, string)> nodeRefferences, Transform container, List<Node_InteractiveDTO> DTOs)
    {
        foreach (Node_InteractiveDTO _node in DTOs)
        {
            var (node, gameObject) = _node.DTO_to_Node();
            nodeRefferences[gameObject.name] = (node, _node.nextNodeID);
            gameObject.transform.parent = container;
        }
    }

    private static void LoadValidationDTOs(Dictionary<string, (Node, string)> nodeRefferences, Transform container, List<Node_ValidationDTO> DTOs)
    {
        foreach (Node_ValidationDTO _node in DTOs)
        {
            var (node, gameObject) = _node.DTO_to_Node();
            nodeRefferences[gameObject.name] = (node, _node.nextNodeID);
            gameObject.transform.parent = container;
        }
    }

    private static void LoadMeasurementDTOs(Dictionary<string, (Node, string)> nodeRefferences, Transform container, List<Node_MeasurementDTO> DTOs)
    {
        foreach (Node_MeasurementDTO _node in DTOs)
        {
            var (node, gameObject) = _node.DTO_to_Node();
            nodeRefferences[gameObject.name] = (node, _node.nextNodeID);
            gameObject.transform.parent = container;
        }
    }

    private static void LoadBranchOutDTOs(Dictionary<string, (Node, string)> nodeRefferences, Transform container, List<Node_BranchOutDTO> DTOs)
    {
        foreach (Node_BranchOutDTO _node in DTOs)
        {
            Node[] nodes = new Node[3];
            int id = 0;
            foreach(string key in _node.branches)
            {
                nodes[id++] = nodeRefferences[key].Item1;
            }
            var (node, gameObject) = _node.DTO_to_Node(nodes);
            nodeRefferences[gameObject.name] = (node, _node.nextNodeID);
            gameObject.transform.parent = container;
        }
    }

    private static void LoadBranchInDTOs(Dictionary<string, (Node, string)> nodeRefferences, Transform container, List<Node_BranchInDTO> DTOs)
    {
        foreach (Node_BranchInDTO _node in DTOs)
        {
            var (node, gameObject) = _node.DTO_to_Node();
            nodeRefferences[gameObject.name] = (node, _node.nextNodeID);
            gameObject.transform.parent = container;
        }
    }

    private static void LoadGND_DTOs(Dictionary<string, (Node, string)> nodeRefferences, Transform container, List<Node_GND_DTO> DTOs)
    {
        foreach (Node_GND_DTO _node in DTOs)
        {
            var (node, gameObject) = _node.DTO_to_Node();
            nodeRefferences[gameObject.name] = (node, "null");
            gameObject.transform.parent = container;
        }
    }





    public static List<ConectionLine> SaveConectionLine(Transform node)
    {
        List<ConectionLine> conectionLine = new List<ConectionLine>();
        foreach (Transform child in node)
        {
            if (child.name.Contains("_Line"))
            {
                conectionLine.Add(new ConectionLine(child));
            }
        }
        return conectionLine;
    }

    public static TransformProperty SetTransformProperty(Transform transform)
    {
        return new TransformProperty(transform.position, transform.rotation);
    }
}

public class NodeDTOBuilder
{
    public Node_SourceDTO node_SourceDTOs;
    public List<Node_InteractiveDTO> node_InteractiveDTOs;
    public List<Node_BranchOutDTO> node_BranchOutDTOs;
    public List<Node_BranchInDTO> node_BranchInDTOs;
    public List<Node_MeasurementDTO> node_MeasurementDTOs;
    public List<Node_ValidationDTO> node_ValidationDTOs;
    public List<Node_GND_DTO> node_GND_DTOs;

    public NodeDTOBuilder(Node_SourceDTO node_SourceDTO)
    {
        node_SourceDTOs = node_SourceDTO;
        node_InteractiveDTOs = new List<Node_InteractiveDTO>();
        node_BranchOutDTOs = new List<Node_BranchOutDTO>();
        node_BranchInDTOs = new List<Node_BranchInDTO>();
        node_MeasurementDTOs = new List<Node_MeasurementDTO>();
        node_ValidationDTOs = new List<Node_ValidationDTO>();
        node_GND_DTOs = new List<Node_GND_DTO>();
    }
}

[System.Serializable]
public struct TransformProperty
{
    public Vector3 position;
    public Quaternion rotation;

    public TransformProperty(Vector3 position, Quaternion rotation)
    {
        this.position = position;
        this.rotation = rotation;
    }
}

[System.Serializable]
public struct ConectionLine
{
    public List<Vector3> knots;
    public Vector3 position;

    public ConectionLine(Transform line)
    {
        knots = new List<Vector3>();
        position = line.localPosition;
        SplineContainer splineContainer = line.GetComponent<SplineContainer>();
        Spline spline = splineContainer.Spline;
        foreach (var knot in spline.Knots)
        {
            knots.Add(knot.Position);
        }
    }
}

[System.Serializable]
public class Node_SourceDTO
{
    public string id;
    public TransformProperty transform;
    public ConectionLine conectionLine;
    public string sourceType;
    public string signalType;
    public ComplexFloat sourceValue;
    public string nextNodeID;
    // public Dictionary<string, string> eventChannels;
    // public Dictionary<string, object> properties;

    public Node_SourceDTO(Node_Source node)
    {
        id = node.name;
        conectionLine = NodeModel.SaveConectionLine(node.transform).First();
        transform = NodeModel.SetTransformProperty(node.transform);
        sourceType = node.GetSourceType();
        signalType = node.GetSignalType();
        sourceValue = node.GetSourceValue();
        nextNodeID = node.GetNextNode()?.name ?? "null";
    }

    public (Node_Source, GameObject) DTO_to_Node()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Source");
        GameObject node = Object.Instantiate(prefab);
        node.name = id;
        node.transform.position = transform.position;
        node.transform.rotation = transform.rotation;
        Node_Source node_Source = node.GetComponent<Node_Source>();
        LineRenderer.CreateLineFromKnots(node_Source, "Room_", conectionLine, null);
        node_Source.SetSourceType(sourceType);
        node_Source.SetSignalType(signalType);
        node_Source.SetSourceValue(sourceValue);
        return (node_Source, node);
    }
}

[System.Serializable]
public class Node_InteractiveDTO
{
    public string id;
    public TransformProperty transform;
    public ConectionLine conectionLine;
    public string nextNodeID;
    //  public Dictionary<string, string> eventChannels;

    public Node_InteractiveDTO(Node_Interactive node)
    {
        id = node.name;
        conectionLine = NodeModel.SaveConectionLine(node.transform).First();
        transform = NodeModel.SetTransformProperty(node.transform);
        nextNodeID = node.GetNextNode()?.name ?? "null";
    }

    public (Node_Interactive, GameObject) DTO_to_Node()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Node");
        GameObject node = Object.Instantiate(prefab);
        node.name = id;
        node.transform.position = transform.position;
        node.transform.rotation = transform.rotation;
        Node_Interactive node_Interactive = node.GetComponent<Node_Interactive>();
        LineRenderer.CreateLineFromKnots(node_Interactive, "Room_", conectionLine, null);
        return (node_Interactive, node);
    }
}

[System.Serializable]
public class Node_BranchOutDTO
{
    public string id;
    public TransformProperty transform;
    public List<ConectionLine> conectionLine;
    public List<string> branches;
    public string nextNodeID;

    public Node_BranchOutDTO(Node_Branch node)
    {
        id = node.name;
        branches = new List<string>();
        conectionLine = NodeModel.SaveConectionLine(node.transform);
        transform = NodeModel.SetTransformProperty(node.transform);
        foreach (Node n in node.GetBranches())
        {
            NodeModel.ReadTree(n);
            branches.Add(n?.name ?? "null");
        }
        nextNodeID = node.GetNextNode()?.name ?? "null";
    }

    public (Node_Branch, GameObject) DTO_to_Node(Node[] branches)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Branch Out");
        GameObject node = Object.Instantiate(prefab);
        node.name = id;
        node.transform.position = transform.position;
        node.transform.rotation = transform.rotation;
        Node_Branch node_Branch = node.GetComponent<Node_Branch>();
        int idx = 0;
        foreach(ConectionLine conectionLine in conectionLine)
        {
            LineRenderer.CreateLineFromKnots(node_Branch, "Room_", conectionLine, idx++);
        }
        node_Branch.SetBranches(branches);
        return (node_Branch, node);
    }
}

[System.Serializable]
public class Node_BranchInDTO
{
    public string id;
    public TransformProperty transform;
    public ConectionLine conectionLine;
    public string nextNodeID;

    public Node_BranchInDTO(Node_Branch node)
    {
        id = node.name;
        conectionLine = NodeModel.SaveConectionLine(node.transform).First();
        transform = NodeModel.SetTransformProperty(node.transform);
        nextNodeID = node.GetNextNode()?.name ?? "null";
    }

    public (Node_Branch, GameObject) DTO_to_Node()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Branch In");
        GameObject node = Object.Instantiate(prefab);
        node.name = id;
        node.transform.position = transform.position;
        node.transform.rotation = transform.rotation;
        Node_Branch node_Branch = node.GetComponent<Node_Branch>();
        LineRenderer.CreateLineFromKnots(node_Branch, "Room_", conectionLine, null);
        return (node_Branch, node);
    }
}

[System.Serializable]
public class Node_MeasurementDTO
{
    public string id;
    public TransformProperty transform;
    public ConectionLine conectionLine;
    //public Dictionary<string, string> eventChannels;
    public string type;
    public string nextNodeID;

    public Node_MeasurementDTO(Node_Measurement node)
    {
        id = node.name;
        conectionLine = NodeModel.SaveConectionLine(node.transform).First();
        transform = NodeModel.SetTransformProperty(node.transform);
        type = node.GetMeterType();
        nextNodeID = node.GetNextNode()?.name ?? "null";
    }

    public (Node_Measurement, GameObject) DTO_to_Node()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Multimeter");
        GameObject node = Object.Instantiate(prefab);
        node.name = id;
        node.transform.position = transform.position;
        node.transform.rotation = transform.rotation;
        Node_Measurement node_Measurement = node.GetComponent<Node_Measurement>();
        LineRenderer.CreateLineFromKnots(node_Measurement, "Room_", conectionLine, null);
        node_Measurement.SetMeterType(type);
        return (node_Measurement, node);
    }
}

[System.Serializable]
public class Node_ValidationDTO
{
    public string id;
    public TransformProperty transform;
    public ConectionLine conectionLine;
    public ComplexFloat R;
    public string controlType;
    public ComplexFloat expectedValue;
    public string nextNodeID;
    // public Dictionary<string, string> eventChannels;
    // public Dictionary<string, object> properties;
    public Node_ValidationDTO(Node_Validation node)
    {
        id = node.name;
        conectionLine = NodeModel.SaveConectionLine(node.transform).First();
        transform = NodeModel.SetTransformProperty(node.transform);
        R = node.GetResistance();
        controlType = node.GetControlType();
        expectedValue = node.GetExpectedValue();
        nextNodeID = node.GetNextNode()?.name ?? "null";
    }

    public (Node_Validation, GameObject) DTO_to_Node()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/Control Panel");
        GameObject node = Object.Instantiate(prefab);
        node.name = id;
        node.transform.position = transform.position;
        node.transform.rotation = transform.rotation;
        Node_Validation node_Validation = node.GetComponent<Node_Validation>();
        LineRenderer.CreateLineFromKnots(node_Validation, "Room_", conectionLine, null);
        node_Validation.SetControlType(controlType);
        node_Validation.SetExpectedValue(expectedValue);
        node_Validation.SetResistance(R);
        return (node_Validation, node);
    }
}

[System.Serializable]
public class Node_GND_DTO
{
    public string id;
    public TransformProperty transform;

    public Node_GND_DTO(Node_GND node)
    {
        id = node.name;
        transform = NodeModel.SetTransformProperty(node.transform);
    }

    public (Node_GND, GameObject) DTO_to_Node()
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/GND");
        GameObject node = Object.Instantiate(prefab);
        node.name = id;
        node.transform.position = transform.position;
        node.transform.rotation = transform.rotation;
        node.AddComponent<Node_Validation>();
        Node_GND node_GND = node.GetComponent<Node_GND>();
        return (node_GND, node);
    }
}
