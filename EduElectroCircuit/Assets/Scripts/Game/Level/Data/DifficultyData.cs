using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class DifficultyData
{
    public bool _star = false;
    public EvaluationGrade _timeGrade = EvaluationGrade.F;
    [SerializeField] private LevelTime _levelTime = null;

    [SerializeField] private EvaluationGrade[] _roomGrades;
    public EvaluationGrade _roomsGrade = EvaluationGrade.F;

    public EvaluationGrade _difficultyGrade = EvaluationGrade.F;

    public DifficultyData()
    {
        _timeGrade = EvaluationGrade.N_A;
        _levelTime = null;

        _roomGrades = null;
        _roomsGrade = EvaluationGrade.N_A;;

        _difficultyGrade = EvaluationGrade.N_A;;
    }

    public DifficultyData(DateTime dateTime, EvaluationGrade[] roomGrades)
    {
        _timeGrade = EvaluationGrade.S;
        _levelTime = new LevelTime(dateTime.Second, dateTime.Minute, dateTime.Hour);

        _roomGrades = roomGrades;
        _roomsGrade = LevelEvaluation.GetGradeAverage(roomGrades);

        _difficultyGrade = LevelEvaluation.GetGradeAverage(roomGrades, EvaluationGrade.S);
    }

    public string GetRoomGradesToString()
    {
        List<string> items = _roomGrades.Select(x => EvaluationGradeExtensions.ToDisplayString(x)).ToList();
        return string.Join(", ", items);
    }

    public EvaluationGrade[] GetRoomGrades() => _roomGrades;

    public string GetTimeToString() => _levelTime.GetTime();
    public LevelTime GetTime() => _levelTime;
    public void ImproveRoomGrades(EvaluationGrade[] newRoomGrades)
    {
        if(_roomGrades.Length != newRoomGrades.Length)
        {
            _roomGrades = new EvaluationGrade[newRoomGrades.Length];
            Array.Fill(_roomGrades, EvaluationGrade.N_A);
        }
        for(int i = 0; i < _roomGrades.Length; i++)
        {
            if(LevelEvaluation.GetGradeImproved(_roomGrades[i], newRoomGrades[i]))
            {
                _roomGrades[i] = newRoomGrades[i];
            }
        }
    }

    public void ImproveTime(DifficultyData newLevelData)
    {
        if(_levelTime == null)
        {
            _levelTime = newLevelData.GetTime();
        }
        else
        {
            _levelTime.ImproveTime(newLevelData.GetTime());
        }
    }
}
