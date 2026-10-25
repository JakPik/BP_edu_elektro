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
    [SerializeField] private UIButton _uIButton;
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
        _stateLabel.RemoveFromClassList("label-error");
        _stateLabel.AddToClassList("label-success");
        _stateLabel.text = "Odemčeny";
    }

    private void StyleAsClosed()
    {
        _stateLabel.RemoveFromClassList("label-success");
        _stateLabel.AddToClassList("label-error");
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

    public override void PanelButtonReset()
    {
        _uIButton.OnButtonReset(this);
        UpdatePanelStyle();
    }

    public override void PanelButtonSave()
    {
        _uIButton.OnButtonStateSave();
    }

    public override void PanelButtonReload(bool circuitActive)
    {
        _uIButton.OnButtonReload(this);
        UpdatePanelStyle();
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