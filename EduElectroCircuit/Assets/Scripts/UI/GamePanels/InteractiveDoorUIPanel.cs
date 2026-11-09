/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: InteractiveDoorUIPanel
 */

using UnityEngine;
using UnityEngine.UIElements;

public class InteractiveDoorUIPanel : InteractiveUIPanel
{
    [SerializeField] private int _roomID;
    [SerializeField] private UIButton _uIButton;
    [SerializeField] private GenericEventChannel<RoomCommandEvent> _roomCommandEventChannel;
    private Label _stateLabel;
    private DoorLockControl _control;

    public void InitDoorUIPanel(DoorLockControl control)
    {
        _control = control;
        _stateLabel = display.rootVisualElement.Q<Label>("State");
        _uIButton.SetUpButton(display);
        UpdatePanelStyle();
        
        SetLocalCoordConverion();
    }

    private void StyleAsOpened()
    {
        UIStyleControler.StyleLabel(_stateLabel, LabelStyles.SUCCESS);
        _stateLabel.text = "Odemčeny";
    }

    private void StyleAsClosed()
    {
        UIStyleControler.StyleLabel(_stateLabel, LabelStyles.ERROR);
        _stateLabel.text = "Zamčeny";
    }

    public override bool SetData<T>(T data) => true;

    public override void OnInteract(bool resetInteraction = false)
    {
        if(currentUIElement != null) {
            currentUIElement.ButtonInteraction(this);
            UpdatePanelStyle();
            _control.LockStateChange(currentUIElement._toggleOn, this);
        }
    }
    public override void DisplayInfo(bool display, RaycastHit hit = default, Camera camera = null)
    {
        if(!display)
        {
            currentUIElement = null;
            _uIButton.Hover(false);
            return;
        }
        
        Vector2? panelPosition = GetPointOnPanel(hit.point, camera);
        if(panelPosition == null) return;
        
        bool found = _uIButton.CheckBounds(panelPosition.Value, worldToPanel);
        _uIButton.Hover(found);

        if(found)
        {
            currentUIElement = _uIButton;
        }
        else
        {
            currentUIElement = null;
        }
    }

    protected override void PanelButtonReset()
    {
        _uIButton.OnButtonReset(this);
        UpdatePanelStyle();
    }

    protected override void PanelButtonSave()
    {
        _uIButton.OnButtonStateSave();
    }

    protected override void PanelButtonReload()
    {
        _uIButton.OnButtonReload(this);
        UpdatePanelStyle();
    }

    public void OnEnable()
    {
        if(_roomCommandEventChannel == null) return;
        _roomCommandEventChannel.OnEventRaised += OnRoomCommand;
    }

    public void OnDisable()
    {
        if(_roomCommandEventChannel == null) return;
        _roomCommandEventChannel.OnEventRaised -= OnRoomCommand;
    }

    protected override void OnRoomCommand(RoomCommandEvent @event)
    {
        if(@event.RoomID != _roomID) return;
        switch(@event.Command)
        {
            case RoomCommand.RELOAD:
                PanelButtonReload();
                break;
            case RoomCommand.RESET:
                PanelButtonReset();
                break;
            case RoomCommand.SAVE:
                PanelButtonSave();
            break;
        }
    }

    public void UpdateLockState(bool unlocked)
    {
        _uIButton.ButtonStateUpdate(unlocked);
        UpdatePanelStyle();
    }

    private void UpdatePanelStyle()
    {
        if(_uIButton._toggleOn)
        {
            StyleAsOpened();
        }
        else
        {
            StyleAsClosed();
        }
    }
}