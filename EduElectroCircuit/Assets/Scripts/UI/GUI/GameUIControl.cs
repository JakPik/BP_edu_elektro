/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: GameUIControl
 */

using System.Collections;
using UnityEngine.SceneManagement;
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
    private Button _resumeButton;
    private Button _restartButton;
    private Button _settingsButton;
    private Button _mainMenuButton;

    private VisualElement _playerGUI;

    private VisualElement _fadePanel;

    private VisualElement _levelEndPanel;
    private Label _levelName;
    private Image _star1;
    private Image _star2;
    private Image _star3;
    private Label _timeValue;
    private Label _timeGrade;
    private Label _timeGradeImprove;
    private Label _roomsGrades;
    private Label _roomFinalGrade;
    private Label _roomGradeImprove;
    private Label _levelGrade;
    private Label _LevelGradeImprove;
    private Button _menu;
    private Button _nextLevel;

    private InteractionControl _interactionControl;

    private Coroutine coroutine;

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
        _levelEndPanel = uiDocument.rootVisualElement.Q<VisualElement>("LevelEnd");

        _resumeButton = uiDocument.rootVisualElement.Q<Button>("ResumeButton");
        _restartButton = uiDocument.rootVisualElement.Q<Button>("RestartButton");
        _settingsButton = uiDocument.rootVisualElement.Q<Button>("SettingsButton");
        _mainMenuButton = uiDocument.rootVisualElement.Q<Button>("MainMenuButton");

        _levelName = uiDocument.rootVisualElement.Q<Label>("LevelName");
        _star1 = uiDocument.rootVisualElement.Q<Image>("Star_1");
        _star2 = uiDocument.rootVisualElement.Q<Image>("Star_2");
        _star3 = uiDocument.rootVisualElement.Q<Image>("Star_3");
        _timeValue = uiDocument.rootVisualElement.Q<Label>("TimeValue");
        _timeGrade = uiDocument.rootVisualElement.Q<Label>("TimeGrade");
        _timeGradeImprove = uiDocument.rootVisualElement.Q<Label>("TimeGradeImprove");
        _roomsGrades = uiDocument.rootVisualElement.Q<Label>("RoomsGrades");
        _roomFinalGrade = uiDocument.rootVisualElement.Q<Label>("RoomFinalGrade");
        _roomGradeImprove = uiDocument.rootVisualElement.Q<Label>("RoomGradeImprove");
        _levelGrade = uiDocument.rootVisualElement.Q<Label>("LevelGrade");
        _LevelGradeImprove = uiDocument.rootVisualElement.Q<Label>("LevelGradeImprove");
        _menu = uiDocument.rootVisualElement.Q<Button>("Menu");
        _nextLevel = uiDocument.rootVisualElement.Q<Button>("NextLevel");

        _interactionControl = new InteractionControl(_playerGUI, this);
    }

    public Coroutine RunCoroutine(IEnumerator enumerator)
    {
        return StartCoroutine(enumerator);
    }

    public void TerminateCoroutine(Coroutine coroutine)
    {
        if(coroutine != null) StopCoroutine(coroutine);
    }

    void OnEnable()
    {
        pauseAction.action.started += OnPause;
        _resumeButton.clicked += OnResume;
        _restartButton.clicked += OnRestart;
        _settingsButton.clicked += OnSettings;
        _mainMenuButton.clicked += OnMainMenu;
        _menu.clicked += OnMainMenu;
        _nextLevel.clicked += OnNextLevel;
    }

    void OnDisable()
    {
        pauseAction.action.started -= OnPause;
        _resumeButton.clicked -= OnResume;
        _restartButton.clicked -= OnRestart;
        _settingsButton.clicked -= OnSettings;
        _mainMenuButton.clicked -= OnMainMenu;
        _menu.clicked -= OnMainMenu;
        _nextLevel.clicked -= OnNextLevel;
    }

    public void UpdateInteraction(InteractionControlData interact, InteractionControlData grab)
    {
        _interactionControl.UpdateInteraction(interact, grab);
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        actionMap.Disable();
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
        SwitchVisibility(true, _pauseGUI);
        SwitchVisibility(false, _playerGUI);
    }

    private void OnResume()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
        SwitchVisibility(true, _playerGUI);
        SwitchVisibility(false, _pauseGUI);
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

    private void OnMainMenu()
    {
        Time.timeScale = 1f;
        StartCoroutine(SceneTransition(PersistanceManager.GetMainMenuID()));
    }

    private void OnNextLevel()
    {
        Time.timeScale = 1f;
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;

        StartCoroutine(SceneTransition(PersistanceManager.GetLevelIDToLoad()));
    }

    public void OnLevelEnd(DifficultyData levelData, LevelSO levelSO)
    {
        actionMap.Disable();
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        _levelName.text = SceneManager.GetActiveScene().name;

        _timeValue.text = levelData.GetTimeToString();

        _timeGrade.text = EvaluationGradeExtensions.ToDisplayString(levelData._timeGrade);

        _roomsGrades.text = levelData.GetRoomGradesToString();
        _roomFinalGrade.text = EvaluationGradeExtensions.ToDisplayString(levelData._roomsGrade);

        _levelGrade.text = EvaluationGradeExtensions.ToDisplayString(levelData._difficultyGrade);

        PersistanceManager.OnLevelCompletion();

        coroutine = StartCoroutine(ShowLevelEndPanel(levelSO));
    }

    private IEnumerator ShowLevelEndPanel(LevelSO levelSO)
    {
        SwitchVisibility(false, _playerGUI);
        _levelEndPanel.style.display = DisplayStyle.Flex;
        SwitchVisibility(true, _levelEndPanel);

        
        if(levelSO.GetLevelData(LevelDifficulty.EASY)._star)
        {
            yield return new WaitForSeconds(0.7f);
            _star1.AddToClassList("star_visible");
        }
        
        if(levelSO.GetLevelData(LevelDifficulty.NORMAL)._star)
        {
            yield return new WaitForSeconds(0.7f);
            _star2.AddToClassList("star_visible");
        }
        
        if(levelSO.GetLevelData(LevelDifficulty.HARD)._star)
        {
            yield return new WaitForSeconds(0.7f);
            _star3.AddToClassList("star_visible");
        }
        
        if(levelSO.timeImporved)
        {
            _timeGradeImprove.RemoveFromClassList("update_tag_hidden");
        }
        
        if(levelSO.roomsGradeImproved)
        {
            yield return new WaitForSeconds(0.5f);
            _roomGradeImprove.RemoveFromClassList("update_tag_hidden");
        }
        
        if(levelSO.difficultyGradeImproved)
        {
            yield return new WaitForSeconds(0.5f);
            _LevelGradeImprove.RemoveFromClassList("update_tag_hidden");
        }
    }

    private IEnumerator SceneTransition(string scene)
    {
        SwitchVisibility(false, _levelEndPanel);
        yield return StartCoroutine(Fade(true));
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        SceneManager.LoadScene(scene);
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
        SwitchVisibility(true, _playerGUI);
        SwitchVisibility(false, _pauseGUI);
        reloadRequest?.RaiseEvent(new ReloadRequestEvent(true), this.name);
    }

    private void SwitchVisibility(bool visible, VisualElement element)
    {
        if(visible)
        {
             element.RemoveFromClassList("panel-hidden");
             element.AddToClassList("panel-visible");
        }
        else
        {
             element.RemoveFromClassList("panel-visible");
             element.AddToClassList("panel-hidden");
        }
       
    }
}