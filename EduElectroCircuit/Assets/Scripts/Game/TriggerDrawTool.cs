using System.Linq;
using UnityEngine;

public enum TriggerType
{
    SPAWN_POINT,
    ANIM_TRIGGER,
    INTERACTION_CLICK,
    INTERACTION_PROXIMITY,
    OTHER
}

public static class TriggerDrawTool
{
    private static TriggerSO _instance;
    private static TriggerSO Instance
    {
        get
        {
            if (_instance == null)
                _instance = TriggerSO.LoadOrCreate(); // Loads or creates the singleton ScriptableObject
            return _instance;
        }
    }

    public static void DrawTriggerGizmos(TriggerType triggerType, Vector3 center, Vector3 size)
    {
        var triggerData = Instance.GetTriggerData(triggerType);
        if (triggerData != null && triggerData.Value.showInEditor)
        {
            Gizmos.color = triggerData.Value.boxColor;
            Gizmos.DrawCube(center, size);

            Gizmos.color = triggerData.Value.borderColor;
            Gizmos.DrawWireCube(center, size);
        }
    }
}