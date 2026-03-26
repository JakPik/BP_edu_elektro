using UnityEngine;

public enum LogType
{
    INFO,
    SUCCESS,
    WARNING,
    ERROR,
    EXCEPTION,
    EVENT
}
public static class Logger
{
    private static LoggerSO _instance;
    private static LoggerSO Instance
    {
        get
        {
            if (_instance == null)
                _instance = LoggerSO.LoadOrCreate(); // Loads or creates the singleton ScriptableObject
            return _instance;
        }
    }

    // Public logging methods
    public static void Log(string label, string msg, LogType type)
    {
        Instance.Log(label, msg, type);
    }

    public static void LogCalculation(string label, string msg, string input, string output)
    {
        Instance.LogCalculation(label, msg, input, output);
    }
}
