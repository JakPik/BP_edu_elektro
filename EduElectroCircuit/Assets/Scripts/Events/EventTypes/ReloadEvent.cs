using System.Numerics;
using UnityEngine;

public class ReloadEvent : EventBaseType
{
    public Transform SpawnTransform;
    public GameObject SpawnReference;
    public ReloadEvent(Transform spawnTransform, GameObject spawnReference)
    {
        SpawnTransform = spawnTransform;
        SpawnReference = spawnReference;
    }
    public override string DisplayData()
    {
        return "(Spawn position: " + SpawnTransform.position +
        ", Spawn Reference: " + SpawnReference?.name + ")";
    }
}