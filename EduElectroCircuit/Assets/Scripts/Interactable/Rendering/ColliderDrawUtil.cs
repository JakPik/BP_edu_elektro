/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: ColliderDrawUtil
 */

using UnityEngine;

public class ColliderDrawUtil : MonoBehaviour
{
    [SerializeField] protected Collider objectCollider;
    [SerializeField] protected TriggerType triggerType;
    [SerializeField] protected Vector3 boundsSize = new Vector3(1, 1, 1);
    [SerializeField] protected Vector3 boundsCenter = Vector3.zero;

    protected virtual void OnDrawGizmos()
    {
        if (objectCollider != null && objectCollider.enabled)
        {
            boundsSize = objectCollider.bounds.size;
            boundsCenter = objectCollider.bounds.center;
        }
        TriggerDrawTool.DrawTriggerGizmos(triggerType, boundsCenter, boundsSize);
    }
}