using UnityEngine;
using UnityEngine.UIElements;

public struct SourceData
{
    public string value;
    public string source;
    public string signal;
}

public class SourceUIPanel : UIPanel
{
    [SerializeField] private UIDocument uiDocument;

    private Label _sourceTypeLabel;
    private Label _signalTypeLabel;
    private Label _valueLabel;

    void Awake()
    {
        _sourceTypeLabel = uiDocument.rootVisualElement.Q<Label>("SourceType");
        _signalTypeLabel = uiDocument.rootVisualElement.Q<Label>("SignalType");
        _valueLabel = uiDocument.rootVisualElement.Q<Label>("Value");
    }
    public override void SetData<T>(T data)
    {
        if (data is SourceData sourceData)
        {
            _sourceTypeLabel.text = sourceData.source;
            _signalTypeLabel.text = sourceData.signal;
            _valueLabel.text = sourceData.value;
            Logger.LogUI(this, "Data loaded. ", LogType.SUCCESS);
            return;
        }
        Logger.LogUI(this, "Data could not be loaded. ", typeof(T) + " != SourceData");
    }


}