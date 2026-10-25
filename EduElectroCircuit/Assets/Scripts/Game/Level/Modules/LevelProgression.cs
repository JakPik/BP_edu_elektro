using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelProgression
{
    public GameObject curspawnPoint { get; private set; } = null;

    public int numOfRespawns { get; private set; } = 0;
    public int numOfActivations { get; private set; } = 0;
    public int numOfRoomRespawns = 0;
    public int numOfRoomActivations = 0;
    public int currentRoomID { get; private set; } = 0;
    public long startTime { get; private set; } = 0;
    public long endTime { get; private set; } = 0;
    public int highestRoomID { get; private set;} = 0;

    public void ResetLevelProgression(GameObject startSpawnPoint)
    {
        currentRoomID = 0;
        highestRoomID = 0;
        numOfRespawns = 0;
        numOfActivations = 0;
        numOfRoomRespawns = 0;
        numOfRoomActivations = 0;
        startTime = 0;

        curspawnPoint = startSpawnPoint;
    }

    public void UpdateLevelProgression(GameObject spawnPoint, int roomID)
    {
        currentRoomID = roomID;
        curspawnPoint = spawnPoint;
        highestRoomID = roomID > highestRoomID? roomID: highestRoomID;
    }

    public void UpdateLevelProgressionData()
    {
        numOfRespawns += numOfRoomRespawns;
        numOfActivations += numOfRoomActivations;
        numOfRoomRespawns = 0;
        numOfRoomActivations = 0;
    }

    public void LevelTime(bool start)
    {
        if(start)
        {
            startTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
        else
        {
            endTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }

    public DifficultyData GetFinalLevelData(RoomProgressControl roomProgressControl)
    {

        DateTime dateTime = DateTimeOffset
            .FromUnixTimeMilliseconds(endTime - startTime)
            .UtcDateTime;

        EvaluationGrade[] roomGrades = roomProgressControl.GetRoomsGrades();

        DifficultyData levelData = new DifficultyData(dateTime,roomGrades);

        Logger.Log(
            this,
            SceneManager.GetActiveScene().name,
            "Finished in: " + levelData.GetTimeToString() + "\n"
            + "Room Grades: " + levelData.GetRoomGradesToString() + "\n"
            + "Level Grade: " + EvaluationGradeExtensions.ToDisplayString(levelData._difficultyGrade) + "\n"
            + "NOA: " + numOfActivations.ToString() + "\n"
            + "NOR: " + numOfRespawns.ToString()
            , LogType.SUCCESS);

        return levelData;
    }

    public GameObject GetCurrentSpawnPoint() => curspawnPoint;
}