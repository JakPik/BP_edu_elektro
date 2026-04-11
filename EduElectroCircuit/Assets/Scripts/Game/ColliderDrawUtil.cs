using UnityEngine;

public abstract class ColliderDrawUtil : MonoBehaviour
{
    [SerializeField] protected Collider spawnPointCollider;
    [SerializeField] protected TriggerType triggerType;
    protected Vector3 boundsSize = new Vector3(1, 1, 1);
    protected Vector3 boundsCenter = Vector3.zero;

    protected virtual void OnDrawGizmos() {
        if (spawnPointCollider != null)
        {
            boundsSize = spawnPointCollider.bounds.size;
            boundsCenter = spawnPointCollider.bounds.center;
        }
        TriggerDrawTool.DrawTriggerGizmos(triggerType, boundsCenter, boundsSize);
    }
}