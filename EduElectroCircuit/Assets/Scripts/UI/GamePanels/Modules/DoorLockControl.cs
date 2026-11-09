using UnityEngine;
using UnityEngine.UIElements;

public class DoorLockControl: MonoBehaviour
{
    [SerializeField] private int _roomID;
    [SerializeField] private InteractiveDoorUIPanel[] _interactiveDoorUIPanel;
    [SerializeField] private GenericEventChannel<NodeValidationEvent> _doorSignal;
    [SerializeField] private bool _defaulUnlockState;
    [SerializeField] private GenericEventChannel<RoomCommandEvent> _roomCommandEventChannel;
    private bool _unlocked;
    private bool _unlockedSave;

    void Awake()
    {
        _unlocked = _defaulUnlockState;
        _unlockedSave = _defaulUnlockState;
        foreach(InteractiveDoorUIPanel doorUIPanel in _interactiveDoorUIPanel)
        {
            doorUIPanel.InitDoorUIPanel(this);
        }
        ChangeDoorLockState(_unlocked);
        LockStateChange(_unlocked, null);
    }

    public void ChangeDoorLockState(bool unlocked)
    {
        _doorSignal?.RaiseEvent(new NodeValidationEvent(unlocked), this.name);
    }

    public void LockStateChange(bool unlocked, InteractiveDoorUIPanel initiator)
    {
        foreach(InteractiveDoorUIPanel doorUIPanel in _interactiveDoorUIPanel)
        {
            if(doorUIPanel != initiator) {
                doorUIPanel.UpdateLockState(unlocked);
            }
        }
    }

    public void LockStateSave()
    {
        _unlockedSave = _unlocked;
    }

    public void LockStateReload()
    {
        _unlocked = _unlockedSave;
        ChangeDoorLockState(_unlocked);
        LockStateChange(_unlocked, null);
    }

    public void LockStateReset()
    {
        _unlocked = _defaulUnlockState;
        _unlockedSave = _defaulUnlockState;
        ChangeDoorLockState(_unlocked);
        LockStateChange(_unlocked, null);
    }

    public void OnEnable()
    {
        _roomCommandEventChannel.OnEventRaised += OnRoomCommand;
    }

    public void OnDisable()
    {
        _roomCommandEventChannel.OnEventRaised -= OnRoomCommand;
    }

    protected void OnRoomCommand(RoomCommandEvent @event)
    {
        if(@event.RoomID != _roomID) return;
        switch(@event.Command)
        {
            case RoomCommand.RELOAD:
                LockStateReload();
                break;
            case RoomCommand.RESET:
                LockStateReset();
                break;
            case RoomCommand.SAVE:
                LockStateSave();
            break;
        }
    }



    
}