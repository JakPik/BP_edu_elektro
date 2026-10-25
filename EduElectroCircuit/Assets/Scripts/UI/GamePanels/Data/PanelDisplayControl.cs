using UnityEngine;

public struct PanelDisplayControl
{
    public bool show;
    public Vector3 worldHitPoint;
    public Camera camera;

    public PanelDisplayControl(bool show, Vector3 worldHitPoint, Camera camera)
    {
        this.show = show;
        this.worldHitPoint = worldHitPoint;
        this.camera = camera;
    }
}