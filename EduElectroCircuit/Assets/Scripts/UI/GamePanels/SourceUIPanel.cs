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
    [SerializeField] private bool _isOfflineDefault;
    [SerializeField] private Node_Source node_Source;
    [SerializeField] private UIButton _uIButton;
    [SerializeField] private GenericEventChannel<RoomCommandEvent> _roomCommandEventChannel;
    [SerializeField] private GenericEventChannel<NodeValidationEvent> _validationChannel;

    private bool _offlineSaveState;
    private bool _isOffline;

    private Label _sourceTypeLabel;
    private Label _signalTypeLabel;
    private Label _valueLabel;
    private Label _frequencyLabel;
    private Label _offlineLabel;
    
    private VisualElement _frequencyPanel;
    private VisualElement _sourcePanel;
    private VisualElement _valuePanel;
    private VisualElement _signalPanel;
    private bool _isDC;

    void Awake()
    {
        _sourceTypeLabel = display.rootVisualElement.Q<Label>("SourceType");
        _signalTypeLabel = display.rootVisualElement.Q<Label>("SignalType");
        _valueLabel = display.rootVisualElement.Q<Label>("Value");
        _frequencyLabel = display.rootVisualElement.Q<Label>("Frequency");
        _offlineLabel = display.rootVisualElement.Q<Label>("Offline");

        _frequencyPanel = display.rootVisualElement.Q<VisualElement>("FrequencyPanel");
        _sourcePanel = display.rootVisualElement.Q<VisualElement>("SourcePanel");
        _valuePanel = display.rootVisualElement.Q<VisualElement>("ValuePanel");
        _signalPanel = display.rootVisualElement.Q<VisualElement>("SignalPanel");
        _offlineSaveState = _isOfflineDefault;
        _isOffline = _isOfflineDefault;


        _uIButton.SetUpButton(display);

        SetLocalCoordConverion();
        DisplayChange();
        PanelButtonSave();

    }
    public override bool SetData<T>(T data)
    {
        if (data is SourceData sourceData)
        {
            _sourceTypeLabel.text = sourceData.sourceType.ToString();
            _signalTypeLabel.text = sourceData.signalType.ToString();
            _valueLabel.text = NodeCalculationModel.FormatValue(sourceData.sourceValue,sourceData.sourceType);
            _frequencyLabel.text = NodeCalculationModel.FormatValue(sourceData.frequency, ElectricalQuantityType.FREQUENCY);
            Logger.LogUI(this, "Data loaded. ", LogType.SUCCESS);
            _isDC = sourceData.signalType == SignalType.DC;
            DisplayChange();
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
        _isOffline = _isOfflineDefault;
        _offlineSaveState = _isOffline;
        _uIButton.OnButtonReset(this);
    }

    protected override void PanelButtonSave()
    {
        _offlineSaveState = _isOffline;
        _uIButton.OnButtonStateSave();
    }

    protected override void PanelButtonReload()
    {
        _isOffline = _offlineSaveState;
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
        if(_validationChannel == null) return;
        _validationChannel.OnEventRaised += OnValidation;
    }

    public void OnDisable()
    {
        _roomCommandEventChannel.OnEventRaised -= OnRoomCommand;
        if(_validationChannel == null) return;
        _validationChannel.OnEventRaised -= OnValidation;
    }

    private void OnValidation(NodeValidationEvent @event)
    {
        _isOffline = !@event.Valid;
        DisplayChange();
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

    private void UpdateOfflineStatus(bool isOffline)
    {
        _isOffline = isOffline;
    }

    private void DisplayChange()
    {
        if(_isOffline)
        {
            _frequencyPanel.style.display = DisplayStyle.None;
            _signalPanel.style.display = DisplayStyle.None;
            _valuePanel.style.display = DisplayStyle.None;
            _sourcePanel.style.display = DisplayStyle.None;
            _offlineLabel.style.display = DisplayStyle.Flex;
            _uIButton.ChangeButtonEnableState(false);
        }
        else
        {
            _signalPanel.style.display = DisplayStyle.Flex;
            _valuePanel.style.display = DisplayStyle.Flex;
            _sourcePanel.style.display = DisplayStyle.Flex;
            _offlineLabel.style.display = DisplayStyle.None;
            _uIButton.ChangeButtonEnableState(true);

            _frequencyPanel.style.display = _isDC? DisplayStyle.None : DisplayStyle.Flex;
        }
    }

    [ContextMenu("Display Change State")]
    public void DisplayChangeState()
    {
        DisplayChange();
    }
}