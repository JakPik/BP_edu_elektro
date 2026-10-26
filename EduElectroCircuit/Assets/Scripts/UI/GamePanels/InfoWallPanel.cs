using UnityEngine;
using UnityEngine.UIElements;

public class InfoWallPanel: MonoBehaviour
{
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private Texture2D _panelImage;

    private Image _image;

    private void Awake() {
        _image = _uiDocument.rootVisualElement.Q<Image>("Image");
        _image.image = _panelImage;
    }
}