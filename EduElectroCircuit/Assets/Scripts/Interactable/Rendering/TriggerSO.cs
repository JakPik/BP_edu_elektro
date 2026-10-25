/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: TriggerSO
 */

using System;
using UnityEngine;

[CreateAssetMenu(fileName = "TriggerSO", menuName = "Utils/TriggerSO")]
public class TriggerSO : ScriptableObject
{
    [SerializeField] public TriggerData[] triggerData;

    public static TriggerSO LoadOrCreate()
    {
        TriggerSO triggerSO = Resources.Load<TriggerSO>("ScriptableObjects/Utils/TriggerSO");
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




