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
    [SerializeField] private Node_Source node_Source;
    [SerializeField] private UIButton _uIButton;
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

    public override void PanelButtonReset()
    {
        _uIButton.OnButtonReset(this);
    }

    public override void PanelButtonSave()
    {
        _uIButton.OnButtonStateSave();
    }

    public override void PanelButtonReload(bool circuitActive)
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
}