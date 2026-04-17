using UnityEngine;

public enum LogType
{
    INFO,
    SUCCESS,
    WARNING,
    ERROR,
    EXCEPTION,
    EVENT,
    CALCULATION
}
public static class Logger
{
    private static LoggerSO _instance;
    private static LoggerSO Instance
    {
        get
        {
            if (_instance == null)
                _instance = LoggerSO.LoadOrCreate();
            return _instance;
        }
    }

    public static void Log<T>(T obj, string label, string msg, LogType type)
    {
        Instance.Log(label, msg + "\n" + obj.ToString(), type);
    }

    public static void LogCalculation(string label, string msg, string input, string output)
    {
        Instance.LogCalculation(label, msg, input, output);
    }

    public static void LogNode<T>(T obj, string msg, LogType type)
    {
        Instance.Log("NODE", msg + "\n" + obj.ToString(), type);
    }

    public static void LogUI<T>(T obj, string msg, string compare)
    {
        Instance.LogUI(msg, compare, obj.ToString());
    }

    public static void LogUI<T>(T obj, string msg, LogType type)
    {
        Instance.Log("UI", msg + "\n" + obj.ToString(), type);
    }

    public static void LogEvent<T>(T obj, string msg)
    {
        Instance.LogEvent(msg + "\n" + obj.ToString());
    }
}
