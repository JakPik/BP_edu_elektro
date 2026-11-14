/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: ReloadEvent
 */

using UnityEngine;

public class ReloadEvent : EventBaseType
{
    public LevelLoadingStates LoadingState;
    public Transform SpawnTransform;
    public GameObject SpawnReference;
    public ReloadEvent(LevelLoadingStates loadingState, Transform spawnTransform = null, GameObject spawnReference = null)
    {
        LoadingState = loadingState;
        SpawnTransform = spawnTransform;
        SpawnReference = spawnReference;
    }
    public override string DisplayData()
    {
        return "(Spawn position: " + SpawnTransform?.position +
        ", Spawn Reference: " + SpawnReference?.name + ")";
    }
}