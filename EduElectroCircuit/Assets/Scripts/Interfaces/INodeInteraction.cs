using UnityEngine;

public interface INodeInteraction
{
    public Transform GetTransform();
    public void SetComponentData(ComponentDataSO componentData);
}