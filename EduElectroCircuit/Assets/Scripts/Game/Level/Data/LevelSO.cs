/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: LightbulbDataSO
 */

using UnityEngine;

[CreateAssetMenu(fileName = "LevelSO", menuName = "LevelData/LevelSO")]

public class LevelSO : ScriptableObject
{
    public string levelName;
    public string levelID;
    public bool fullComplete;
    public bool locked = true;

    public bool timeImporved;
    public bool roomsGradeImproved;
    public bool difficultyGradeImproved;
    [SerializeField] private DifficultyData bestEasy = new DifficultyData();
    [SerializeField] private DifficultyData bestNormal = new DifficultyData();
    [SerializeField] private DifficultyData bestHard = new DifficultyData();
    public EvaluationGrade levelGrade = EvaluationGrade.F;

    public void UpdateDifficulty(LevelDifficulty levelDifficulty, DifficultyData currentLevelData)
    {
        switch(levelDifficulty)
        {
            case LevelDifficulty.EASY:
                UpdateLevelData(bestEasy, currentLevelData);
                break;
            case LevelDifficulty.NORMAL:
                UpdateLevelData(bestNormal, currentLevelData);
                break;
            case LevelDifficulty.HARD:
                UpdateLevelData(bestHard, currentLevelData);
                break;
        }
        EvaluationGrade[] difficultyGrades = {
            bestEasy._difficultyGrade,
            bestNormal._difficultyGrade,
            bestHard._difficultyGrade
        };
        levelGrade = LevelEvaluation.GetGradeAverage(difficultyGrades);
        if(levelGrade == EvaluationGrade.S)
        {
            fullComplete = true;
        }
    }

    private void UpdateLevelData(DifficultyData levelData, DifficultyData currentLevelData)
    {
        difficultyGradeImproved = false;
        roomsGradeImproved = false;
        timeImporved = false;

        if(LevelEvaluation.GetGradeImproved(levelData._difficultyGrade, currentLevelData._difficultyGrade))
        {
            difficultyGradeImproved = true;
            levelData._difficultyGrade = currentLevelData._difficultyGrade;
        }

        if(!levelData._star && levelData._difficultyGrade == EvaluationGrade.S)
        {
            levelData._star = true;
        }

        if(LevelEvaluation.GetGradeImproved(levelData._roomsGrade, currentLevelData._roomsGrade))
        {
            roomsGradeImproved = true;
            levelData._roomsGrade = currentLevelData._roomsGrade;
        }

        if(LevelEvaluation.GetGradeImproved(levelData._timeGrade, currentLevelData._timeGrade))
        {
            timeImporved = true;
            levelData._timeGrade = currentLevelData._timeGrade;
        }

        if(levelData?.GetTime().GetTimeImproved(currentLevelData.GetTime()) ?? true)
        {
            levelData.ImproveTime(currentLevelData);
        }

        if(roomsGradeImproved)
        {
            levelData.ImproveRoomGrades(currentLevelData.GetRoomGrades());
        } 
    }

    public DifficultyData GetLevelData(LevelDifficulty levelDifficulty) => levelDifficulty switch
    {
        LevelDifficulty.EASY => bestEasy,
        LevelDifficulty.NORMAL => bestNormal,
        LevelDifficulty.HARD => bestHard,
        _ => null,
    };

    [ContextMenu("Clear Progress")]
    public void ClearProgress()
    {
        fullComplete = false;
        locked = true;
        bestEasy = new DifficultyData();
        bestNormal = new DifficultyData();
        bestHard = new DifficultyData();
        levelGrade = EvaluationGrade.N_A;
    }
}