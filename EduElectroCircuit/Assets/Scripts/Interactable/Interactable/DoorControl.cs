/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: DoorControl
 */

using UnityEngine;
using System.Collections;

public abstract class DoorControl : ColliderDrawUtil
{
    [SerializeField] protected GameObject _enterPoint;
    [SerializeField] protected float _animSpeed;
    [SerializeField] private bool _twoWay = false;
    [SerializeField] private bool _locked = false;
    [Tooltip("Determins if components can be carried through the door. True => can be carried, Flase => can not be carried.")]
    [SerializeField] private bool _transportComponents = true;
    [SerializeField] private GenericEventChannel<NodeValidationEvent> _nodeValidation;
    protected Coroutine _curCoroutine = null;
    protected Vector3 _forward;
    private PlayerInteractionControl _playerInteraction = null;
    private int _numberOfComponents = 0;



    private void OnEnable()
    {
        if (_nodeValidation == null)
        {
            Logger.Log(this, "DOOR", "nodeValidation is null", LogType.WARNING);
            return;
        }
        _nodeValidation.OnEventRaised += LockState;
    }

    private void OnDisable()
    {
        if (_nodeValidation == null) return;
        _nodeValidation.OnEventRaised -= LockState;
    }

    public void LockState(NodeValidationEvent @event)
    {
        _locked = !@event.Valid;
        if(_locked)
        {
            StartAnimation(CloseDoor());
        }
        else if(_playerInteraction != null)
        {
            if(!_transportComponents && (_numberOfComponents > 0 || _playerInteraction.GetHoldingState())) return;
            StartAnimation(OpenDoor());
        }

    }

    private void StartAnimation(IEnumerator coroutine)
    {
        if (_curCoroutine != null) StopCoroutine(_curCoroutine);
        _curCoroutine = StartCoroutine(coroutine);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out CircuitComponent _) && !_transportComponents)
        {
            _numberOfComponents++;
            StartAnimation(CloseDoor());
            return;
        }
        if(!other.CompareTag("Player")) return;
        if(other.TryGetComponent(out PlayerInteractionControl playerInteractionControl))
        {
            _playerInteraction = playerInteractionControl;
            if((!_transportComponents && (_numberOfComponents > 0)) || _locked) {
                StartAnimation(CloseDoor());
                return;
            }
            Vector3 position = this.transform.position - other.transform.position;
            if (!_twoWay && Vector3.Dot(position, _forward) < 0) return;

            StartAnimation(OpenDoor());
        }        
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out CircuitComponent _))
        {
            _numberOfComponents--;
            if(_numberOfComponents == 0 && _playerInteraction != null && !_locked) StartAnimation(OpenDoor());
            return;
        }
        if(!other.CompareTag("Player")) return;
        _playerInteraction = null;
        StartAnimation(CloseDoor());
    }

    public abstract IEnumerator OpenDoor();

    public abstract  IEnumerator CloseDoor();
}
