using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class MainMenu
{
    [SerializeField] private float _animDuration = 0.8f;
    private VisualElement _mainMenuContainer;
    private VisualElement _mainMenuPanel;

    
    private Button _playButton;
    private Button _quitButton;
    private Button _levelSelectButton;

    private MainMenuUIControl _mainMenuControl;
    public void InitMainMenu(VisualElement mainMenuContainer, MainMenuUIControl mainMenuControl)
    {
        _mainMenuControl = mainMenuControl;
        _mainMenuContainer = mainMenuContainer;

        _mainMenuPanel = _mainMenuContainer.Q<VisualElement>("MainMenuPanel");
        _mainMenuPanel.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        _mainMenuContainer.style.display = DisplayStyle.Flex;
        
        _playButton = _mainMenuContainer.Q<Button>("Play_Button");
        _quitButton = _mainMenuContainer.Q<Button>("Quit_Button");
        _levelSelectButton = _mainMenuContainer.Q<Button>("Levels");
        
        _playButton.clicked += OnPlayButtonClicked;
        _quitButton.clicked += OnQuitButtonClicked;
        _levelSelectButton.clicked += OnLevelSelectClicked;
    }

   private void OnPlayButtonClicked()
    {
        PersistanceManager.SetLoadLevel();
        _mainMenuControl.Transition(MainMenuTransitions.TO_GAME);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void OnLevelSelectClicked()
    {
        _mainMenuControl.Transition(MainMenuTransitions.MAIN_MENU_TO_LEVELS);
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        _mainMenuPanel.style.translate = new Translate(0,0, 0);
    }

    public IEnumerator AnimateTransition()
    {
        float start = _mainMenuPanel.resolvedStyle.translate.x;
        float end = start == 0f? -_mainMenuPanel.resolvedStyle.width: 0f;
        float elapsed = 0f;
        float t = 0f;

        yield return null;
        while(t < 1f)
        {
            elapsed += Time.deltaTime;
            t = Mathf.Clamp01(elapsed / _animDuration);

            _mainMenuPanel.style.translate = new Translate(
            Mathf.Lerp(start, end, t),
            0,
            0);
            yield return null;
        }

    }
}