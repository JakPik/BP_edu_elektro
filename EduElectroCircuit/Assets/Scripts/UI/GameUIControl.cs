using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class GameUIControl : MonoBehaviour
{
    public static GameUIControl Instance;

    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private float animSpeedIn = 3.5f;
    [SerializeField] private float animSpeedOut = 1.0f;
    [SerializeField] private InputActionAsset actionMap;
    [SerializeField] private InputActionReference pauseAction;
    [SerializeField] private GenericEventChannel<ReloadRequestEvent> reloadRequest;


    private VisualElement _pauseGUI;
    private VisualElement _playerGUI;
    private VisualElement _fadePanel;

    private Button _resumeButton;
    private Button _restartButton;
    private Button _settingsButton;
    private Button _mainMenuButton;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
        SetUpGUI();
    }

    public void SetUpGUI()
    {
        _pauseGUI = uiDocument.rootVisualElement.Q<VisualElement>("PauseGUI");
        _playerGUI = uiDocument.rootVisualElement.Q<VisualElement>("PlayerGUI");
        _fadePanel = uiDocument.rootVisualElement.Q<VisualElement>("Fade");

        _resumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        _restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        _settingsButton = uiDocument.rootVisualElement.Q<Button>("SettingsButton");
        _mainMenuButton = uiDocument.rootVisualElement.Q<Button>("MainMenuButton");
    }

    void OnEnable()
    {
        pauseAction.action.started += OnPause;
        _resumeButton.clicked += OnResume;
        _restartButton.clicked += OnRestart;
        _settingsButton.clicked += OnSettings;
        _mainMenuButton.clicked += OnMainMenu;
    }

    void OnDisable()
    {
        pauseAction.action.started -= OnPause;
        _resumeButton.clicked -= OnResume;
        _restartButton.clicked -= OnRestart;
        _settingsButton.clicked -= OnSettings;
        _mainMenuButton.clicked -= OnMainMenu;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        actionMap.Disable();
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        _playerGUI.RemoveFromClassList("panel-visible");
        _pauseGUI.AddToClassList("panel-visible");
    }

    private void OnResume()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        _playerGUI.AddToClassList("panel-visible");
        _pauseGUI.RemoveFromClassList("panel-visible");
        actionMap.Enable();
    }

    private void OnRestart()
    {
        Time.timeScale = 1f;

        StartCoroutine(Restart());
    }

    private void OnSettings()
    {

    }

    public void OnMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(MainMenuTransition());

    }

    private IEnumerator MainMenuTransition()
    {
        yield return StartCoroutine(Fade(true));
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }

    public IEnumerator Fade(bool fadeIn)
    {
        StyleFloat target = fadeIn ? 1f : 0f;
        if (fadeIn)
        {
            _fadePanel.AddToClassList("fade-in");
        }
        else
        {
            _fadePanel.RemoveFromClassList("fade-in");
        }
        while (_fadePanel.resolvedStyle.opacity != target)
        {
            yield return null;
        }
        if (fadeIn)
        {
            actionMap.Disable();
        }
        else
        {
            actionMap.Enable();
        }
    }

    private IEnumerator Restart()
    {
        yield return StartCoroutine(Fade(true));
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        _playerGUI.AddToClassList("panel-visible");
        _pauseGUI.RemoveFromClassList("panel-visible");
        reloadRequest?.RaiseEvent(new ReloadRequestEvent(true), this.name);
    }
}