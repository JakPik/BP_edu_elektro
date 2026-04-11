using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Logger", menuName = "Utils/LoggerSO")]
public class LoggerSO: ScriptableObject
{
    [SerializeField] public Color info;
    [SerializeField] private Color success;
    [SerializeField] private Color warning;
    [SerializeField] private Color error;
    [SerializeField] private Color exception;
    [SerializeField] private Color @event;

    [SerializeField] private List<string> excludeLabels;
    [SerializeField] private List<LogType> excludeTypes;

    public void Log(string label, string msg, LogType type)
    {
        if(SkipLog(label, type)) return;
        Debug.Log($"<color=#{GetColor(type)}>[{type.ToString()}] - {label}</color><color=white>: {msg}</color>");
    }

    public void LogCalculation(string label, string msg, string input, string output)
    {
        Debug.Log($"<color=#fff600>[CALCULATION] - {label}</color><color=white>: {msg}\n{input}\n{output}</color>");
    }

        public string GetColor(LogType type)
    {
        return type switch
        {
            LogType.INFO => info.ToHexString(),
            LogType.SUCCESS => success.ToHexString(),
            LogType.WARNING => warning.ToHexString(),
            LogType.ERROR => error.ToHexString(),
            LogType.EXCEPTION => exception.ToHexString(),
            LogType.EVENT => @event.ToHexString(),
            _ => "white"
        };
    }

    private bool SkipLog(string label, LogType type)
    {
        if(excludeTypes.Contains(type)) return true;
        if(type == LogType.EVENT) return false;
        if(excludeLabels.Any<string>(item => label.ToLower().Contains(item))) return true;
        return false;
    }

    public static LoggerSO LoadOrCreate()
    {
        var path = "Assets/Resources/ScriptableObjects/Utils/LoggerSO.asset";
        var logger = UnityEditor.AssetDatabase.LoadAssetAtPath<LoggerSO>(path);
        if (logger == null)
        {
            logger = ScriptableObject.CreateInstance<LoggerSO>();
            UnityEditor.AssetDatabase.CreateAsset(logger, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }
        return logger;
    }
}