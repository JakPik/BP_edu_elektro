using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TriggerSO", menuName = "Utils/TriggerSO")]
public class TriggerSO : ScriptableObject
{
    [SerializeField] public TriggerData[] triggerData;

    public static TriggerSO LoadOrCreate()
    {
        var path = "Assets/Resources/ScriptableObjects/Utils/TriggerSO.asset";
        var triggerSO = UnityEditor.AssetDatabase.LoadAssetAtPath<TriggerSO>(path);
        if (triggerSO == null)
        {
            triggerSO = ScriptableObject.CreateInstance<TriggerSO>();
            UnityEditor.AssetDatabase.CreateAsset(triggerSO, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }
        return triggerSO;
    }

    public TriggerData? GetTriggerData(TriggerType triggerType)
    {
        foreach (var data in triggerData)
        {
            if (data.triggerType == triggerType)
                return data;
        }
        return null;
    }

    [Serializable]
    public struct TriggerData
    {
        public TriggerType triggerType;
        public Color boxColor;
        public Color borderColor;
        public bool showInEditor;
    }
}




