/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: LevelControl
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelControl : MonoBehaviour
{
    [SerializeField] private RoomProgressControl roomProgressControl;
    [SerializeField] private LevelSO levelSO;
    [Header("Spawn points")]
    [SerializeField] private GameObject startSpawnPoint;

    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<ReloadEvent> reload;

    [Header("Event Channels")]
    [SerializeField] private GenericEventChannel<ReloadRequestEvent> reloadRequest;
    [SerializeField] private GenericEventChannel<CircuitActiveStateEvent> circuitActiveStateEventChannel;

    [Header("Control Variables")]

    public static LevelControl Instance;

    private Coroutine curCoroutine;
    private LevelProgression levelProgression;

    void Awake()
    {
        levelProgression = new LevelProgression();
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        PersistanceManager.InitPersistance();
        curCoroutine = StartCoroutine(LevelLoading(true));
    }
    public void EndLevel()
    {
        levelProgression.LevelTime(false);
        roomProgressControl.SaveRoomStates(
            levelProgression.currentRoomID,
            levelProgression.highestRoomID,
            levelProgression.numOfRoomActivations,
            levelProgression.numOfRoomRespawns,
            true
        );

        levelProgression.UpdateLevelProgressionData();
        DifficultyData levelData = levelProgression.GetFinalLevelData(roomProgressControl);

        levelSO.UpdateDifficulty(PersistanceManager.GetLevelDifficulty(), levelData);

        GameUIControl.Instance.OnLevelEnd(levelData, levelSO);
    }

    private IEnumerator LevelEnterInit()
    {
        yield return StartCoroutine(roomProgressControl.LoadRoomData());

        levelProgression.ResetLevelProgression(startSpawnPoint);
        reload.RaiseEvent(
            new ReloadEvent(
                LevelLoadingStates.LOADING_DATA_TRANSFER,
                levelProgression.curspawnPoint.transform,
                null),
            this.name
        );

        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(GameUIControl.Instance.Fade(false));
        levelProgression.LevelTime(true);
        curCoroutine = null;
        reload.RaiseEvent(
            new ReloadEvent(
                LevelLoadingStates.LOADING_FINISHED),
            this.name
        );
    }

    public void SetNextSpawnPoint(GameObject spawnPoint, int roomID)
    {
        int previousRoomID = levelProgression.currentRoomID;
        levelProgression.UpdateLevelProgression(spawnPoint, roomID);
        roomProgressControl.SaveRoomStates(
            previousRoomID,
            levelProgression.highestRoomID,
            levelProgression.numOfRoomActivations, 
            levelProgression.numOfRoomRespawns
        );

        levelProgression.UpdateLevelProgressionData();
    }

    private IEnumerator Respawn()
    {
        levelProgression.numOfRoomRespawns++;
        yield return StartCoroutine(GameUIControl.Instance.Fade(true));

        reload.RaiseEvent(
            new ReloadEvent(
                LevelLoadingStates.LOADING_DATA_TRANSFER,
                levelProgression.curspawnPoint.transform,
                levelProgression.curspawnPoint),
            this.name
        );
        StartCoroutine(roomProgressControl.ReloadRooms(levelProgression.highestRoomID));

        yield return new WaitForSeconds(0.8f);
        yield return StartCoroutine(GameUIControl.Instance.Fade(false));
        curCoroutine = null;
        reload.RaiseEvent(
            new ReloadEvent(
                LevelLoadingStates.RELOAD_FINISHED),
            this.name
        );
    }

    void OnEnable()
    {
        reloadRequest.OnEventRaised += OnReloadRequest;
        circuitActiveStateEventChannel.OnEventRaised += OnCircuitActive;
    }

    void OnDisable()
    {
        reloadRequest.OnEventRaised -= OnReloadRequest;
        circuitActiveStateEventChannel.OnEventRaised -= OnCircuitActive;
    }

    private void OnCircuitActive(CircuitActiveStateEvent @event)
    {
        if(@event.circuitActive && @event.nodeCircuit_ID.roomID == levelProgression.currentRoomID)
        {
             levelProgression.numOfRoomActivations++;
        }
    }

    private void OnReloadRequest(ReloadRequestEvent @event)
    {
        if (curCoroutine != null) return;
        curCoroutine = StartCoroutine(LevelLoading(@event.FirstLoad));
    }

    private IEnumerator LevelLoading(bool firstLoad)
    {
        reload.RaiseEvent(
            new ReloadEvent(
                LevelLoadingStates.LOADING_START),
            this.name
        );
        if (firstLoad)
        {
            yield return StartCoroutine(LevelEnterInit());
        }
        else
        {
            yield return StartCoroutine(Respawn());
        }
    }
}
