using UnityEngine;

public interface INodeInteraction
{
    public (Vector3, Transform) GetTransform();
    public void SetComponentData(ComponentDataSO componentData, GameObject circuitComponent);
    public bool CanConnect();
    public bool CanGrab();
}