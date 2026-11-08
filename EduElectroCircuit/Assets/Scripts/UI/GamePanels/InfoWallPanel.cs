using UnityEngine;
using UnityEngine.UIElements;

public class InfoWallPanel: MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;

    [TextArea]
    [SerializeField] private string _textTop;
    [SerializeField] private LabelStyles _topLabelStyle;

    [TextArea]
    [SerializeField] private string _textBottom;
    [SerializeField] private LabelStyles _bottomLabelStyle;
    [SerializeField] private Texture2D _panelImage;

    private Image _image;
    private Label _labelTop;
    private Label _labelBottom;

    private void Awake() {
        UpdateUI();
    }

    public void UpdateTextBottom(string text, LabelStyles labelStyle)
    {
        _textBottom = text;
        _labelBottom.text = _textBottom;
        if(_textBottom == "")
        {
            _labelBottom.style.display = DisplayStyle.None;
        }
        else
        {
            UIStyleControler.StyleLabel(_labelBottom, labelStyle);
            _labelBottom.style.display = DisplayStyle.Flex;
        }
    }

    [ContextMenu("Update UI")]
    public void UpdateUI()
    {
        _image = _uiDocument.rootVisualElement.Q<Image>("Image");
        _labelTop = _uiDocument.rootVisualElement.Q<Label>("Label_top");
        _labelBottom = _uiDocument.rootVisualElement.Q<Label>("Label_bottom");
        _labelBottom.text = _textBottom;
        _labelTop.text = _textTop;
        if(_panelImage == null)
        {
            _image.style.display = DisplayStyle.None;
        }
        else
        {
            _image.image = _panelImage;
        }
        if(_textBottom == "")
        {
            _labelBottom.style.display = DisplayStyle.None;
        }
        else
        {
            UIStyleControler.StyleLabel(_labelBottom, _bottomLabelStyle);
        }
        UIStyleControler.StyleLabel(_labelTop, _topLabelStyle);
    }
}