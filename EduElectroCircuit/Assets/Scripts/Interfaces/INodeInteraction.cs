/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: INodeInteraction
 */

using UnityEngine;

public interface INodeInteraction
{
    public (Vector3, Transform) GetTransform();
    public void SetComponentData(ComponentDataSO componentData, GameObject circuitComponent);
    public bool CanConnect();
    public bool CanGrab();
}