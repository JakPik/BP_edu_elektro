using UnityEngine;

public struct PortData
{
    public Vector3 portDirection;
    public Vector3 portPosition;
    public Vector3? portLocalPosition;

    public PortData(Vector3 portDirection, Vector3 portPosition, Vector3? portLocalPosition)
    {
        this.portDirection = portDirection;
        this.portPosition = portPosition;
        this.portLocalPosition = portLocalPosition;
    }
}