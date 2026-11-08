using UnityEngine;
using UnityEngine.Events;

public class ComponentTriggerCounter: ColliderDrawUtil
{
    [SerializeField] private int _roomID;
    [SerializeField] private int _maxNumberOfComponents;
    [SerializeField] private int _startValue;
    [SerializeField] private GenericEventChannel<RoomCommandEvent> _roomCommandChannel;
    [SerializeField] private UnityEvent<string, LabelStyles> _countUpdate;
    private int _savedValue;
    private int _currentCount;

    public void Start()
    {
        _currentCount = _startValue;
        CountUpdate();
    }

    public void OnEnable()
    {
        _roomCommandChannel.OnEventRaised +=OnRoomCommand;
    }

    public void OnDisable()
    {
        _roomCommandChannel.OnEventRaised -= OnRoomCommand;
    }

    private void OnRoomCommand(RoomCommandEvent @event)
    {
        if(@event.RoomID != _roomID) return;
        switch(@event.Command)
        {
            case RoomCommand.RELOAD:
                OnReload();
                break;
            case RoomCommand.RESET:
                OnReset();
                break;
            case RoomCommand.SAVE:
                OnSave();
            break;
        }
    }

    private void OnReset()
    {
        _currentCount = _startValue;
        CountUpdate();
    }

    private void OnReload()
    {
        _currentCount = _savedValue;
        CountUpdate();
    }

    private void OnSave()
    {
        _savedValue = _currentCount;
    }

    private void CountUpdate()
    {
        LabelStyles labelStyle = _currentCount == 0? LabelStyles.ERROR:
                                 _currentCount < _maxNumberOfComponents? LabelStyles.PARAMETER: LabelStyles.SUCCESS;
        _countUpdate.Invoke(_currentCount.ToString() + "/" + _maxNumberOfComponents.ToString(), labelStyle);
    }

    void OnTriggerExit(Collider other)
    {
        if(other.tag != "Component") return;
        Vector3 directionOfExit = other.transform.position - transform.position;
        directionOfExit = directionOfExit.normalized;
        float scalar = Vector3.Dot(directionOfExit, transform.forward) / Vector3.Dot(transform.forward, transform.forward);
        directionOfExit = (scalar * transform.forward).normalized;
        if(Vector3.Dot(directionOfExit, transform.forward) > 0)
        {
            _currentCount++;
        }
        else
        {
            _currentCount--;
        }
        CountUpdate();
    }
}