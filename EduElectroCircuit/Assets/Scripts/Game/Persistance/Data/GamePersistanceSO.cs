using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "Game Persistance", menuName = "GameData/GamePersistance")]
public class GamePersistanceSO: ScriptableObject
{
    [SerializeField] private string _mainMenu;
    [SerializeField] private LevelSO _nextLevel;
    [SerializeField] private LevelSO _curLevel;
   // [SerializeField] private string _continueLevel;
    [SerializeField] private LevelDifficulty _LevelDifficulty;


    public void SetLoadLevel(LevelSO level, LevelDifficulty levelDifficulty)
    {
        _LevelDifficulty = levelDifficulty;
        _curLevel = level;
    }

    public void SetLoadLevel()
    {
        _LevelDifficulty = LevelDifficulty.EASY;
        _curLevel = _nextLevel;
    }

    public void SetLevelDifficutly(LevelDifficulty levelDifficulty)
    {
        _LevelDifficulty = levelDifficulty;
    }

    public string GetCurLevelID() => _curLevel?.levelID;
    public string GetNextLevelID() => _nextLevel?.levelID;
    public LevelDifficulty GetLevelDifficulty() => _LevelDifficulty;

    public void OnLevelCompletion()
    {
        if(_curLevel == _nextLevel)
        {
            int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;

            string nextScenePath = SceneUtility.GetScenePathByBuildIndex(nextIndex);
            if(string.IsNullOrEmpty(nextScenePath))
            {
                _nextLevel = null;
                return;
            }
            _nextLevel = Resources.Load<LevelSO>("ScriptableObjects/Levels/Level_" + nextIndex);
            _nextLevel.locked = false;
            _curLevel = _nextLevel;
        }
        else
        {
            int levelID = int.Parse(_curLevel.levelID.Split("_")[1]);
            _curLevel = Resources.Load<LevelSO>("ScriptableObjects/Levels/Level_" + (levelID + 1));
            _curLevel.locked = false;
        }
    }

    public string GetMainMenuID() => _mainMenu;
}