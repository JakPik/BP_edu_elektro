/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: Node_Interactive
 */

using UnityEngine;
using System;
using Unity.VisualScripting;
using System.Collections;
using UnityEngine.Rendering.Universal;
using System.Linq;

public class Node_Interactive : Node, INodeInteraction
{
    #region Variables
    [Space(10)]
    [Header("Control Value")]
    [SerializeField] private GameObject _refConnected;
    [Space(10)]
    [SerializeField] private DecalProjector _decalProjector;
    [SerializeField] private Material _decalMaterial;
    [SerializeField] private float _animDuration = 0.8f;
    [Space(10)]
    [SerializeField] private ComponentType[] _excludedComponents;
    [Space(10)]
    [SerializeField] private Vector3 _positionOffset;

    #region Events
    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<NodeStateChange> _nodeStateChange;
    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> _circuitActiveState;
    #endregion

    private MaterialPropertyBlock _materialPropertyBlock;
    private Renderer _renderer;
    private NodeConectionStates _nodeState;
    private CircuitComponent _circuitComponent;
    private Coroutine _coroutine;
    private ComponentData _componentData;
    private Coroutine _DC_CapacitorRoutine;
    private ComplexFloat _chargingCapacitorRezistance;
    private bool _chargingCapacitorStarted;

    const float ChargingMultiplier = 8;
    #endregion

    private void Awake() {
        _nodeState = NodeConectionStates.EMPTY;
        _renderer = this.GetComponent<Renderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _decalProjector.material = new Material(_decalMaterial);
        _materialPropertyBlock.SetColor("_StripColor", InteractiveNodeStatesExt.ToColor(_nodeState));
        _decalProjector.material.SetColor("_Color", InteractiveNodeStatesExt.ToColor(_nodeState));
        _decalProjector.material.SetFloat("_Intensity", 0f);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }

    #region Node class Implementation
    #region Calculation
    /// <summary>
    /// <para>Calculates based on the type of the node:</para>
    /// <list type="bullet">
    ///   <item>NodeType.NODE_PASSIVE => Reads the originValues and assignes it as local R,U,I.</item>
    ///   <item>NodeType.NODE_ACTIVE  => Calculates the local U,I.</item>
    ///   <item>NodeType.NODE_CONTROL => Calculates local U,I and Invokes NodeValidationEvent.</item>
    /// </list>
    /// </summary>
    /// <inheritdoc />
    public override void CalculateValues(NodeDataModel passValues, NodeDataModel originValues)
    {
        if(_componentData.type == ComponentType.CAPACITOR && !_chargingCapacitorStarted)
        {
            _chargingCapacitorStarted = true;
            _DC_CapacitorRoutine = StartCoroutine(StartCapacitorCharging());
        }
        if(passValues.Ic.GetMagnitude() != 0)
        {
            NodeStateChange(NodeConectionStates.ACTIVE_CLOSED_CIRCUIT);
        }
        if(!_locked) return;

        (U, I) = NodeCalculationModel.CalculateNodeValues(passValues, R);
        Logger.LogNode(this, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        _circuitComponent?.SetData(ComponentControlState.ACTIVE, U, I);

        if (_nextNode != null) _nextNode.CalculateValues(passValues, originValues);
    }

    public override ComplexFloat GetResistanceSum(SignalType signalType, float frequency)
    {
        if(signalType == SignalType.DC)
        {
            R = NodeCalculationModel.GetImpedanceDC(_componentData);
            if(_chargingCapacitorStarted)
            {
                R = _chargingCapacitorRezistance;
            }
        }
        else
        {
            
        }
       /* switch(_componentData.type)
        {
            case ComponentType.RESISTOR:
                R = ((ResistorData)_componentData).GetResistance();
                break;
            case ComponentType.LIGHTBULB:
                R = ((LightbulbData)_componentData).GetResistance();
                break;
        }*/
        Logger.LogNode(this, "Local R: " + R, LogType.INFO);
        return _nextNode == null? R : R + _nextNode.GetResistanceSum(signalType, frequency);
    }

    private IEnumerator StartCapacitorCharging()
    {
        float elapsed = 0f;
        float capacitance = 0.001f;
        float totalRezistance = 255;
        while(_chargingCapacitorRezistance.GetMagnitude() < 100000)
        {
            yield return new WaitForSeconds(0.5f);
            elapsed += 0.5f;
            float divider = capacitance * totalRezistance * ChargingMultiplier;
            _chargingCapacitorRezistance.real += Mathf.Exp(elapsed/divider);
            R = _chargingCapacitorRezistance;
        }
        _chargingCapacitorRezistance.SetNumberAsInf(true);
    }

    #endregion

    #region Node Conections
    public override bool CheckConection()
    {
        if(_nextNode == null) return _connected;

        var nextConnected = _nextNode.CheckConection();

        return nextConnected ? _connected : nextConnected;
    }
    #endregion

    #region Conection Construction
    public override void BuildConections(Node branchInRef, int branchId, string containerName)
    {
        if (_nextNode == null && branchInRef == null)
        {
            Logger.LogNode(this, "Line construction done. No nodes to connect to.", LogType.SUCCESS);
            return;
        }
        try
        {
            if (_nextNode != null)
            {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), _nextNode.GetInPortPosition(0), containerName);
                _nextNode.BuildConections(branchInRef, branchId, containerName);
            }
            else
            {
                LineRenderer.BuildLine(this, GetOutPortPosition(0), branchInRef.GetInPortPosition(branchId), containerName);
            }
        }
        catch (Exception e)
        {
            Logger.LogNode(this, e.Message, LogType.ERROR);
            return;
        }
    }
    #endregion
    #endregion

    #region INodeInteraction
    public (Vector3, Transform) GetTransform() => (transform.position + _positionOffset, transform);

    public void SetComponentData(ComponentData componentData, GameObject obj)
    {
        if (componentData == null)
        {
            ClearNode(obj);
            return;
        }

        _componentData = componentData;

        NodeStateChange(NodeConectionStates.PLACED_COMPONENT);
        _refConnected = obj;
        _connected = true;
        /*switch(componentData.type)
        {
            case ComponentType.RESISTOR:
                R = ((ResistorData)componentData).GetResistance();
                break;
            case ComponentType.LIGHTBULB:
                R = ((LightbulbData)componentData).GetResistance();
                break;
        }*/
        _circuitComponent = obj.GetComponent<CircuitComponent>();
        _nodeStateChange?.RaiseEvent(new NodeStateChange(_nodeCircuit_ID), this.name);
    }

    public bool CanConnect(ComponentType type) => !_excludedComponents.Contains(type) && !_connected && !_locked;

    public bool CanGrab() => !_locked;
    #endregion

    #region Node_Interactive local logic
    void OnTriggerExit(Collider other)
    {
        ClearNode(other.gameObject);
    }
    private void ClearNode(GameObject caller)
    {
        if (caller == _refConnected)
        {
            NodeStateChange(NodeConectionStates.EMPTY);
            _refConnected = null;
            _connected = false;
            RefreshValues();
            _circuitComponent?.SetData(ComponentControlState.INACTIVE, U, I);
            _circuitComponent = null;
            _nodeStateChange?.RaiseEvent(new NodeStateChange(_nodeCircuit_ID), this.name);
            _componentData = null;
        }
    }
    #endregion

    #region Event Handling Logic
    void OnEnable()
    {
        _circuitActiveState.OnEventRaised += CircuitActiveStateChange;
    }

    void OnDisable()
    {
        _circuitActiveState.OnEventRaised -= CircuitActiveStateChange;
    }

    private void CircuitActiveStateChange(CircuitActiveStateEvent @event)
    {
        if(!_nodeCircuit_ID.RoomMatch(@event.nodeCircuit_ID)) return;
        _locked = @event.circuitActive;
        _chargingCapacitorStarted = false;
        if(_locked)
        {
            _chargingCapacitorRezistance = new ComplexFloat();
            NodeStateChange(NodeConectionStates.ACTIVE_OPEN_CIRCUIT);
        }
        else
        {
            NodeStateChange(_refConnected == null? NodeConectionStates.EMPTY: NodeConectionStates.PLACED_COMPONENT);
        }
        if (_refConnected != null && _refConnected.TryGetComponent(out IGrabable grabable))
        {
            Logger.LogNode(this, "Grab lock state changed for: " + _refConnected.name, LogType.INFO);
            grabable.LockGrab(_locked);
            _circuitComponent?.SetData(ComponentControlState.INACTIVE,U, I);
        }
    }
    #endregion

    #region Animation
    private void NodeStateChange(NodeConectionStates nodeState)
    {
        if(_nodeState == nodeState) return;
        _nodeState = nodeState;
        if(_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(AnimateColorStrip());
    }

    private IEnumerator AnimateColorStrip()
    {
        Color start = _materialPropertyBlock.GetColor("_StripColor");
        Color end = InteractiveNodeStatesExt.ToColor(_nodeState);
        Color blend = start;
        float startFloat = _decalProjector.material.GetFloat("_Intensity");
        float endFloat = _nodeState == NodeConectionStates.EMPTY? 0f: 0.3f;
        yield return null;
        float elapsed = 0f;
        while(blend != end)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / _animDuration);
            blend = Color.Lerp(start, end, t);
            _materialPropertyBlock.SetColor("_StripColor", blend);
            _decalProjector.material.SetColor("_Color", blend);
            _decalProjector.material.SetFloat("_Intensity", Mathf.Lerp(startFloat, endFloat, t));
            _renderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }
        _coroutine = null;
    }
    #endregion
}
