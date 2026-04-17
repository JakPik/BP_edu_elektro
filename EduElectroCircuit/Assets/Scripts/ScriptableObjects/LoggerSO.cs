using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(fileName = "Logger", menuName = "Utils/LoggerSO")]
public class LoggerSO : ScriptableObject
{
    [SerializeField] public Color info;
    [SerializeField] private Color success;
    [SerializeField] private Color warning;
    [SerializeField] private Color error;
    [SerializeField] private Color exception;
    [SerializeField] private Color @event;
    [SerializeField] private Color calculation;

    [SerializeField] private List<string> excludeLabels;
    [SerializeField] private List<LogType> excludeTypes;

    private static string filePath = null;

    public void Log(string label, string msg, LogType type)
    {
        if (filePath != null) WriteToLog($"<span style=\"color:#{GetColor(type)}\">[{type}] - {label}</span><span style=\"color:white\">: {msg}</span>");

        if (SkipLog(label, type)) return;
        Debug.Log($"<color=#{GetColor(type)}>[{type}] - {label}</color><color=white>: {msg}</color>");
    }

    public void LogEvent(string msg)
    {
        LogType type = LogType.EVENT;
        if (filePath != null) WriteToLog($"<span style=\"color:#{GetColor(type)}\">[{type}]</span><span style=\"color:white\">: {msg}</span>");

        if (SkipLog(null, type)) return;
        Debug.Log($"<color=#{GetColor(type)}>[{type}]</color><color=white>: {msg}</color>");
    }

    public void LogUI(string msg, string compare, string origin)
    {
        LogType type = LogType.ERROR;
        if (filePath != null) WriteToLog($"<span style=\"color:#{GetColor(type)}\">[{type}] - UI</span><span style=\"color:white\">: {msg}</span><span style=\"color:#{GetColor(type)}\">{compare}</span><span style=\"color:white\">:\n{origin}</span>");

        if (SkipLog(null, type)) return;
        Debug.Log($"<color=#{GetColor(type)}>[{type}] - UI</color><color=white>: {msg} </color><color=#{GetColor(type)}>{compare}</color><color=white>:\n{origin}</color>");
    }

    public void LogCalculation(string label, string msg, string input, string output)
    {
        LogType type = LogType.CALCULATION;
        if (filePath != null) WriteToLog($"<span style=\"color:#{GetColor(type)}\">[{type}] - {label}</span><span style=\"color:white\">: {msg}\n{input}\n{output}</span>");

        if (SkipLog(null, type)) return;
        Debug.Log($"<color=#{GetColor(type)}>[{type}] - {label}</color><color=white>: {msg}\n{input}\n{output}</color>");
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
            LogType.CALCULATION => calculation.ToHexString(),
            _ => "white"
        };
    }

    private bool SkipLog(string label, LogType type)
    {
        if (type == LogType.ERROR || type == LogType.EXCEPTION) return false;
        if (excludeTypes.Contains(type)) return true;
        if (label != null && excludeLabels.Any<string>(item => label.ToUpper().Contains(item))) return true;
        return false;
    }

    public static void StartLogging()
    {
        filePath = Path.Combine(Application.persistentDataPath, "log.md");

        File.WriteAllText(filePath, "# Game Log\n");
    }

    public static void WriteToLog(string log)
    {
        string time = System.DateTime.Now.ToString("HH:mm:ss");
        string line = "\n- **[" + time + "]** - " + log;
        File.AppendAllText(filePath, line);
    }

    public static LoggerSO LoadOrCreate()
    {
        LoggerSO logger = Resources.Load<LoggerSO>("ScriptableObjects/Utils/LoggerSO");
        StartLogging();
        /*var path = "Assets/Resources/ScriptableObjects/Utils/LoggerSO.asset";
        var logger = UnityEditor.AssetDatabase.LoadAssetAtPath<LoggerSO>(path);
        if (logger == null)
        {
            logger = ScriptableObject.CreateInstance<LoggerSO>();
            UnityEditor.AssetDatabase.CreateAsset(logger, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }*/
        return logger;
    }
}