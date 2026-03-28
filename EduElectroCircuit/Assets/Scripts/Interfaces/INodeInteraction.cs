using UnityEngine;

public interface INodeInteraction
{
    public (Vector3, Transform) GetTransform();
    public void SetComponentData(ComponentDataSO componentData, CircuitComponent circuitComponent);
    public bool CanConnect();
}