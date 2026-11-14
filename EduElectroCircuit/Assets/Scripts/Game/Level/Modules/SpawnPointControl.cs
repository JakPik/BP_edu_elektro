/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: SpawnPointControl
 */

using System.Text.RegularExpressions;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class SpawnPointControl : ColliderDrawUtil
{
    [SerializeField] private int roomID;
    [SerializeField] private bool _startActive = true;
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;
    [SerializeField] private GenericEventChannel<NodeValidationEvent> _validationChannel;
    [SerializeField] private bool multiTrigger = false;

    private bool _activeState;
    private bool _disabled;

    void Awake()
    {
        _activeState = _startActive;
        _disabled = false;
        try {
            roomID = int.Parse(Regex.Match(this.transform.parent.name, @"-?\d+").Value);
        }
        catch
        {
            roomID = 0;
        }
        SetColliderState(_activeState);
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LevelControl.Instance.SetNextSpawnPoint(this.gameObject,roomID);
            if(!multiTrigger) {
                SetColliderState(false);
                _disabled = true;
            }
        }
    }

    void OnEnable()
    {
        reload.OnEventRaised += OnReload;
        if(_validationChannel == null) return;
        _validationChannel.OnEventRaised += OnValidation;
    }

    void OnDisable()
    {
        reload.OnEventRaised -= OnReload;
        if(_validationChannel == null) return;
        _validationChannel.OnEventRaised -= OnValidation;
    }

    private void OnValidation(NodeValidationEvent @event)
    {
        _activeState = @event.Valid;
        SetColliderState(_activeState);
    }

    private void SetColliderState(bool enabled)
    {
        if(_disabled) return;
        this.GetComponent<BoxCollider>().enabled = enabled;
    }

    private void OnReload(ReloadEvent @event)
    {
        if (@event.LoadingState == LevelLoadingStates.LOADING_DATA_TRANSFER && @event.SpawnReference == null)
        {
            _activeState = _startActive;
            _disabled = false;
            SetColliderState(_activeState);
        }
    }
}
