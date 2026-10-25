using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public static class PersistanceManager
{
    private static GamePersistanceSO _gamePersistance;

    public static void InitPersistance()
    {
        _gamePersistance = Resources.Load<GamePersistanceSO>("ScriptableObjects/Game/GamePersistance");
    }

    public static void SetLoadLevel(LevelSO level, LevelDifficulty levelDifficulty)
    {
        _gamePersistance.SetLoadLevel(level, levelDifficulty);
    }

    public static void SetLoadLevel()
    {
        _gamePersistance.SetLoadLevel();
    }

    public static string GetLevelIDToLoad()
    {
        string levelID = _gamePersistance.GetCurLevelID();
        string nextLevelID = _gamePersistance.GetNextLevelID();
        if(levelID == null) return _gamePersistance.GetMainMenuID();
        if(nextLevelID != null  && levelID.Contains(nextLevelID))
        {
            _gamePersistance.SetLevelDifficutly(LevelDifficulty.EASY);
        }
        return levelID;
    }

    public static LevelDifficulty GetLevelDifficulty() => _gamePersistance.GetLevelDifficulty();

    public static void OnLevelCompletion()
    {
        _gamePersistance.OnLevelCompletion();
    }

    public static string GetMainMenuID() => _gamePersistance.GetMainMenuID();
}