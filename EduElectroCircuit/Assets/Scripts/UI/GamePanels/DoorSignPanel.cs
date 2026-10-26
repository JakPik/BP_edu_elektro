using UnityEngine;
using UnityEngine.UIElements;

public class DoorSignPanel: MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private string _labelText;
    [SerializeField] private LabelStyles _labelStyle;
    [SerializeField] private GameObject _frame_LD;
    [SerializeField] private GameObject _frame_RU;

    private Label _label;
    private VisualElement _panel;

    private void Awake() {
        _panel = _uiDocument.rootVisualElement.Q<VisualElement>("Panel");
        _label = _uiDocument.rootVisualElement.Q<Label>("Label");
        _label.text = _labelText;
        UIStyleControler.StyleLabel(_label, _labelStyle);
    }

    private void Start() {
        _panel.RegisterCallback<GeometryChangedEvent>(_ => {
            float width = _panel.resolvedStyle.width;
           /* if(width > 160)
            {
                _label.style.whiteSpace = WhiteSpace.Normal;
            }*/
            Vector3 xOffset = new Vector3(width/200, 0,0);
            _frame_LD.transform.localPosition = _frame_LD.transform.localPosition + xOffset;
            _frame_RU.transform.localPosition = _frame_RU.transform.localPosition - xOffset;
        });
    }
}