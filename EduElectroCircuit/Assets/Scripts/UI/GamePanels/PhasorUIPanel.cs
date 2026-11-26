/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: MeterUIPanel
 */

using UnityEngine;
using UnityEngine.UIElements;



public class PhasorUIPanel : UIPanel
{
    private Label _voltageLabel;
    private Label _currentLabel;
    private RadialPhasorViewer _radialPhasorViewer;

    void Awake()
    {
        var panel = display.rootVisualElement.Q<VisualElement>("VoltageAmplitude");
        _voltageLabel = panel.Q<Label>("Value");
        panel = display.rootVisualElement.Q<VisualElement>("CurrentAmplitude");
        _currentLabel = panel.Q<Label>("Value");
        _radialPhasorViewer = display.rootVisualElement.Q<RadialPhasorViewer>("Phasor");
    }

    public override bool SetData<T>(T data)
    {
        if (data is PhasorDiagramData phasorDiagramData)
        {
            _voltageLabel.text = phasorDiagramData.voltage;
            _currentLabel.text = phasorDiagramData.current;
            _radialPhasorViewer.UpdateVisibility(
                phasorDiagramData.visible,
                phasorDiagramData.voltageComplex,
                phasorDiagramData.currentComplex
                );
            Logger.LogUI(this, "Data loaded. ", LogType.SUCCESS);
            return true;
        }
        Logger.LogUI(this, "Data could not be loaded. ", typeof(T) + " != MeasurePointData");
        return false;
    }
}