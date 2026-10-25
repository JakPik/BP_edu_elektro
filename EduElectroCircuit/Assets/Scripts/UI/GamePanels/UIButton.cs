using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[System.Serializable]
public class UIButton
{
    [SerializeField] private string _buttonName;
    private Button _button;
    [SerializeField] private bool _isEnabled = true;
    public bool _toggleOn {get; private set; }
    [SerializeField] private string _toggleOnText;
    [SerializeField] private string _toggleOffText;
    [SerializeField] private UnityEvent<bool> _onInteraction;
    private bool _defaultEnable;
    private bool _defaultToogle;
    private bool _savedToggle;

    private const string StyleToggleON = "button-on";
    private const string StyleToggleOFF = "button-off";
    private const string StyleButtonClick = "active";
    private const string StyleButtonHover = "hover";
    private const float ClickTransition = 0.2f;

    public void SetUpButton(UIDocument display)
    {
        _button = display.rootVisualElement.Q<Button>(_buttonName);
        _defaultEnable = _isEnabled;
        _defaultToogle = _toggleOn;
        _button.RemoveFromClassList(_toggleOn? StyleToggleOFF: StyleToggleON);
        _button.AddToClassList(_toggleOn? StyleToggleON: StyleToggleOFF);
        _button.text = _toggleOn? _toggleOnText: _toggleOffText;
        _button.SetEnabled(_isEnabled);
        _button.style.display = _isEnabled? DisplayStyle.Flex : DisplayStyle.None;
    }

    public void Hover(bool hover)
    {
        if(hover)
        {
            _button.AddToClassList(StyleButtonHover);
        }
        else
        {
            _button.RemoveFromClassList(StyleButtonHover);
            
        }
    }

    public void OnButtonReset(bool changed, MonoBehaviour coroutineOwner)
    {
        _toggleOn = changed? !_defaultToogle : _defaultToogle;
        _isEnabled = _defaultEnable;
        _onInteraction.Invoke(_toggleOn);
        coroutineOwner.StartCoroutine(ButtonClick());
    }

    public void OnButtonReset(MonoBehaviour coroutineOwner)
    {
        _toggleOn = _defaultToogle;
        _savedToggle = _defaultToogle;
        _isEnabled = _defaultEnable;
        _onInteraction.Invoke(_toggleOn);
        coroutineOwner.StartCoroutine(ButtonClick());
    }

    public void OnButtonReload(MonoBehaviour coroutineOwner)
    {
        _toggleOn = _savedToggle;
        _isEnabled = _defaultEnable;
        _onInteraction.Invoke(_toggleOn);
        coroutineOwner.StartCoroutine(ButtonClick());
    }

    public void OnButtonStateSave()
    {
        _savedToggle = _toggleOn;
    }

    public void ButtonStateUpdate(bool toggleOn)
    {
        _toggleOn = toggleOn;
        StyleButton();
    }

    public void ButtonInteraction(MonoBehaviour coroutineOwner)
    {
        _toggleOn = !_toggleOn;
        _onInteraction.Invoke(_toggleOn);
        coroutineOwner.StartCoroutine(ButtonClick());
    }

    public IEnumerator ButtonClick()
    {
        _button.AddToClassList(StyleButtonClick);
        yield return new WaitForSeconds(ClickTransition);
        _button.RemoveFromClassList(StyleButtonClick);

        StyleButton();
    }

    private void StyleButton()
    {
        _button.RemoveFromClassList(_toggleOn? StyleToggleOFF: StyleToggleON);
        _button.AddToClassList(_toggleOn? StyleToggleON: StyleToggleOFF);
        _button.text = _toggleOn? _toggleOnText: _toggleOffText;
    }

    public bool CheckBounds(Vector2 position, Vector2 worldToPanel)
    {
        if(!_isEnabled) return false;
        Rect rect = _button.worldBound;

        float x = rect.x * worldToPanel.x;
        float y = rect.y * worldToPanel.y;
        float width = rect.width * worldToPanel.x;
        float height = rect.height * worldToPanel.y;

        if(position.y < y || position.y > y + height) return false;
        if(position.x < x || position.x > x + width) return false;
        return true;
    }
}