/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: SourceUIPanel
 */

using UnityEngine;
using UnityEngine.UIElements;

public class SourceUIPanel : InteractiveUIPanel
{
    [SerializeField] private int _roomID;
    [SerializeField] private Node_Source node_Source;
    [SerializeField] private UIButton _uIButton;
    [SerializeField] private GenericEventChannel<RoomCommandEvent> _roomCommandEventChannel;
    private Label _sourceTypeLabel;
    private Label _signalTypeLabel;
    private Label _valueLabel;
    private Label _period;
    
    private VisualElement _periodPanel;

    void Awake()
    {
        _sourceTypeLabel = display.rootVisualElement.Q<Label>("SourceType");
        _signalTypeLabel = display.rootVisualElement.Q<Label>("SignalType");
        _valueLabel = display.rootVisualElement.Q<Label>("Value");
        _period = display.rootVisualElement.Q<Label>("Period");
        _periodPanel = display.rootVisualElement.Q<VisualElement>("PeriodPanel");

        _uIButton.SetUpButton(display);

        SetLocalCoordConverion();
    }
    public override bool SetData<T>(T data)
    {
        if (data is SourceData sourceData)
        {
            _sourceTypeLabel.text = sourceData.sourceType.ToString();
            _signalTypeLabel.text = sourceData.signalType.ToString();
            _valueLabel.text = NodeCalculationModel.FormatValue(sourceData.sourceValue,sourceData.sourceType);
            Logger.LogUI(this, "Data loaded. ", LogType.SUCCESS);
            if(sourceData.signalType == SignalType.DC) {
                _periodPanel.style.display = DisplayStyle.None;
            }
            return true;
        }
        Logger.LogUI(this, "Data could not be loaded. ", typeof(T) + " != SourceData");
        return false;
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
    }

    protected override void PanelButtonSave()
    {
        _uIButton.OnButtonStateSave();
    }

    protected override void PanelButtonReload()
    {
        _uIButton.OnButtonReload(this);
    }

    public void TurnCircuitOff()
    {
        if(_uIButton._toggleOn)
        {
            _uIButton.ButtonInteraction(this);
        }
    }

    public void OnEnable()
    {
        _roomCommandEventChannel.OnEventRaised += OnRoomCommand;
    }

    public void OnDisable()
    {
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

}