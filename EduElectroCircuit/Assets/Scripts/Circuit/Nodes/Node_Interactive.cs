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
    [Tooltip("If true, the reference object cannot be removed from this node. A reference must be assigned before the application starts.")]
    [SerializeField] private bool _referenceLock;
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
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;
    #endregion

    private MaterialPropertyBlock _materialPropertyBlock;
    private Renderer _renderer;
    private NodeConectionStates _nodeState;
    private CircuitComponent _circuitComponent;
    private Coroutine _coroutine;
    private ComponentData _componentData;
    private ComplexFloat _chargingCapacitorRezistance;
    private bool _chargingCapacitorStarted;

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

    private void ReferenceLock(ReloadEvent @event) {
       /* if(@event.LoadingState == LevelLoadingStates.LOADING_START &&
            _referenceLock)
        {
            CircuitComponent circuitComponent = _refConnected.GetComponent<CircuitComponent>();
            if(circuitComponent != null) {
                SetComponentData(circuitComponent.GetComponentData(), _refConnected);
                NodeStateChange(NodeConectionStates.PLACED_COMPONENT_LOCKED);
                circuitComponent.ForcePermanentNodeConnection(transform.position + _positionOffset, transform);
            }
        }*/
        if( @event.LoadingState == LevelLoadingStates.LOADING_FINISHED &&
            _referenceLock)
        {
            CircuitComponent circuitComponent = _refConnected.GetComponent<CircuitComponent>();
            if(circuitComponent != null) {
                SetComponentData(circuitComponent.GetComponentData(), _refConnected);
                NodeStateChange(NodeConectionStates.PLACED_COMPONENT_LOCKED);
                circuitComponent.ForcePermanentNodeConnection(transform.position + _positionOffset, transform, this);
            }
        }
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
        if(passValues.Ic.GetMagnitude() != 0)
        {
            NodeStateChange(NodeConectionStates.ACTIVE_CLOSED_CIRCUIT);
        }
        if(!_locked) return;

        (U, I) = NodeCalculationModel.CalculateNodeValues(passValues, R);
        Logger.LogNode(this, "U: " + U + "V, I: " + I + " mA, R: " + R, LogType.INFO);
        _circuitComponent?.SetData(ComponentControlState.ACTIVE, U, I, originValues.Rc, passValues.signalType == SignalType.DC);

        if (_nextNode != null) _nextNode.CalculateValues(passValues, originValues);
    }

    public override ComplexFloat GetResistanceSum(SignalType signalType, float frequency)
    {
        R = NodeCalculationModel.GetImpedance(_componentData, frequency, signalType == SignalType.DC);
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
            _nodeStateChange?.RaiseEvent(new NodeStateChange(_nodeCircuit_ID, false), this.name);
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
        _nodeStateChange?.RaiseEvent(new NodeStateChange(_nodeCircuit_ID, true), this.name);
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
            _circuitComponent?.SetData(ComponentControlState.INACTIVE, U, I, R, false);
            _circuitComponent = null;
            _nodeStateChange?.RaiseEvent(new NodeStateChange(_nodeCircuit_ID, false), this.name);
            _componentData = null;
        }
    }
    #endregion

    #region Event Handling Logic
    void OnEnable()
    {
        _circuitActiveState.OnEventRaised += CircuitActiveStateChange;
        reload.OnEventRaised += ReferenceLock;
    }

    void OnDisable()
    {
        _circuitActiveState.OnEventRaised -= CircuitActiveStateChange;
        reload.OnEventRaised -= ReferenceLock;
    }

    private void CircuitActiveStateChange(CircuitActiveStateEvent @event)
    {
        if(_nodeCircuit_ID != @event.nodeCircuit_ID) return;
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
            if(!_locked)
            {
                _circuitComponent?.SetData(ComponentControlState.INACTIVE,U, I, R, false);
            }
        }
    }
    #endregion

    #region Animation
    private void NodeStateChange(NodeConectionStates nodeState)
    {
        if(_nodeState == nodeState) return;
        if(_referenceLock)
        {
            if(nodeState == NodeConectionStates.ACTIVE_CLOSED_CIRCUIT)
            {
                _nodeState = nodeState;
            }
            else
            {
                _nodeState = NodeConectionStates.PLACED_COMPONENT_LOCKED;
            }
        }
        else {
            _nodeState = nodeState;
        }
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
