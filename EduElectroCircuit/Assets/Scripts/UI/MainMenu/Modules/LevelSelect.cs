using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class LevelSelect
{
    [SerializeField] private VisualTreeAsset levelSelectTab;
    [SerializeField] private Texture _lockUnlocked;
    [SerializeField] private Texture _lockLocked;
    [SerializeField] private float _animDuration = 0.8f;

    private Label _levelName;
    private Label _levelGrade;
    private DifficultyPanel _easyPanel;
    private DifficultyPanel _normalPanel;
    private DifficultyPanel _hardPanel;
    private ScrollView _scrollView;
    private LevelSO[] _levelSO;
    private Button _selectedButton;
    private Button _levelStart;
    private Button _back;

    private DropdownField _difficultyField;

    private VisualElement _levelSelectContainer;
    private VisualElement _levelSelectPanel;

    private MainMenuUIControl _mainMenuControl;
    private LevelSO _selectedLevel;

    public void InitLevelSelect(VisualElement levelSelectContainer, MainMenuUIControl mainMenuControl)
    {
        _mainMenuControl = mainMenuControl;
        _levelSelectContainer = levelSelectContainer;
        _levelSelectPanel = _levelSelectContainer.Q<VisualElement>("LevelSelectPanel");
        _levelSelectPanel.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);

        _levelName = _levelSelectContainer.Q<Label>("LevelName");
        _levelGrade = _levelSelectContainer.Q<Label>("LevelGrade");

        _easyPanel = DifficultyPanelLoader(_levelSelectContainer.Q<VisualElement>("EasyPanel"));
        _normalPanel = DifficultyPanelLoader(_levelSelectContainer.Q<VisualElement>("NormalPanel"));
        _hardPanel = DifficultyPanelLoader(_levelSelectContainer.Q<VisualElement>("HardPanel"));

        _scrollView = _levelSelectContainer.Q<ScrollView>("ScrollView");

        _difficultyField = _levelSelectContainer.Q<DropdownField>("DifficultyField");

        _levelStart =_levelSelectContainer.Q<Button>("LevelStart");
        _back =_levelSelectContainer.Q<Button>("Back");
        _levelStart.clicked += OnLevelStart;
        _back.clicked += OnMenuBack;

        _levelSO = Resources.LoadAll<LevelSO>("ScriptableObjects/Levels");

        _difficultyField.RegisterValueChangedCallback(evt => {
            if(!Enum.TryParse(evt.newValue, out LevelDifficulty _))
            {
                Logger.Log(this, "Level Difficulty Parsing", "Value \"" + evt.newValue +"\" is not a defined key in => LevelDifficulty enum", LogType.ERROR);
                throw new Exception("Could not Parse Level Difficulty");
            }
        });
    }

    private void OnGeometryChanged(GeometryChangedEvent evt)
    {
        _levelSelectContainer.style.translate = new Translate(0,
            -_levelSelectContainer.resolvedStyle.height, 0);
    }

    public IEnumerator AnimateTransition()
    {
        float start = _levelSelectContainer.resolvedStyle.translate.y;
        float end = start == 0f? -_levelSelectContainer.resolvedStyle.height: 0f;
        float elapsed = 0f;
        float t = 0f;

        yield return null;
        while(t < 1f)
        {
            elapsed += Time.deltaTime;
            t = Mathf.Clamp01(elapsed / _animDuration);

            _levelSelectContainer.style.translate = new Translate(
            0,
            Mathf.Lerp(start, end, t),
            0);
            yield return null;
        }
    }

    private void OnMenuBack()
    {
        _mainMenuControl.Transition(MainMenuTransitions.LEVELS_TO_MAIN_MENU);
    }

    private void OnLevelStart()
    {
        PersistanceManager.SetLoadLevel(_selectedLevel, Enum.Parse<LevelDifficulty>(_difficultyField.value));
        _mainMenuControl.Transition(MainMenuTransitions.TO_GAME);
    }

    private DifficultyPanel DifficultyPanelLoader(VisualElement panel)
    {
        DifficultyPanel difficultyPanel = new DifficultyPanel
        {
            _panel = panel,

            _star = panel.Q<Image>("Star"),

            _difficultyGrade = panel.Q<Label>("DifficultyGrade"),

            _time = panel.Q<Label>("Time"),
            _timeGrade = panel.Q<Label>("TimeGrade"),

            _roomGrades = panel.Q<Label>("RoomsGrades"),
            _roomsGrade = panel.Q<Label>("RoomFinalGrade")
        };
        return difficultyPanel;
    }

    public void PopulateLevelSelect()
    {
        for(int i = 0; i < _levelSO.Length; i++)
        {
            var item = levelSelectTab.Instantiate();

            var lv_num = item.Q<Label>("LV_Number");
            var lv_name = item.Q<Label>("LevelName");
            var lv_lock = item.Q<Image>("Lock");
            var button = item.Q<Button>("Button");

            int index = 1 + i;
            int id = i;
            lv_num.text = "Lv." + index.ToString();
            lv_name.text = _levelSO[i].levelName;
            lv_lock.image = _levelSO[i].locked? _lockLocked: _lockUnlocked;

            button.RemoveFromClassList("levelTabButton_silver");

            if(_levelSO[i].locked)
            {
                button.AddToClassList("levelTabButton_locked");
            }
            else
            {
                if(_levelSO[i].fullComplete)
                {
                    button.AddToClassList("levelTabButton_gold");
                }
                else
                {
                    button.AddToClassList("levelTabButton_silver");
                }
                if(i == 0)
                {
                    button.AddToClassList("selected");
                    _selectedButton = button;
                    LoadLevelDetail(_levelSO[0]);
                }
            }

            button.clicked += () =>
            {
                OnLevelTabClicked(id, button);
            };

            _scrollView.Add(item);
        }
    }

    private void OnLevelTabClicked(int index, Button button)
    {
        
        if(_levelSO[index].locked || button == _selectedButton) return;
        button.AddToClassList("selected");
        _selectedButton.RemoveFromClassList("selected");
        _selectedButton = button;
        
        LoadLevelDetail(_levelSO[index]);
    }

    private void LoadLevelDetail(LevelSO levelSO)
    {
        _selectedLevel = levelSO;
        _levelName.text = levelSO.levelName;
        _difficultyField.value = LevelDifficulty.EASY.ToString();

        _levelGrade.text = EvaluationGradeExtensions.ToDisplayString(levelSO.levelGrade);
        LoadDifficultyDetail(_easyPanel, levelSO.GetLevelData(LevelDifficulty.EASY));
        LoadDifficultyDetail(_normalPanel, levelSO.GetLevelData(LevelDifficulty.NORMAL));
        LoadDifficultyDetail(_hardPanel, levelSO.GetLevelData(LevelDifficulty.HARD));

       // Texture2D texture2D = Resources.Load<Textture2D>("Textures/Level_1_thumbnail")
    }

    private void LoadDifficultyDetail(DifficultyPanel difficultyPanel, DifficultyData levelData)
    {
        difficultyPanel._star.style.display = levelData._star? DisplayStyle.Flex: DisplayStyle.None;
        difficultyPanel._difficultyGrade.text = EvaluationGradeExtensions.ToDisplayString(levelData._difficultyGrade);

        difficultyPanel._time.text = levelData.GetTimeToString();
        difficultyPanel._timeGrade.text = EvaluationGradeExtensions.ToDisplayString(levelData._timeGrade);

        difficultyPanel._roomGrades.text = levelData.GetRoomGradesToString();
        difficultyPanel._roomsGrade.text = EvaluationGradeExtensions.ToDisplayString(levelData._roomsGrade);
    }
}