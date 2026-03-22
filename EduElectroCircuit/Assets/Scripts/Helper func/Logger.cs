using UnityEngine;

public enum Log_Type
{
    INFO,
    SUCCESS,
    WARNING,
    ERROR,
    EXCEPTION
}
public static class Logger
{
    public static void Log(string label, string msg, Log_Type type)
    {
        Debug.Log($"<color={GetColor(type)}>[{type.ToString()}] - {label}</color><color=white>: {msg}</color>");
    }

    public static void LogCalculation(string label, string msg, string input, string output)
    {
        Debug.Log($"<color=#fff600>[CALCULATION] - {label}</color><color=white>: {msg}\n{input}\n{output}</color>");
    }

        public static string GetColor(Log_Type type)
    {
        return type switch
        {
            Log_Type.INFO => "cyan",
            Log_Type.SUCCESS => "#05e841",
            Log_Type.WARNING => "orange",
            Log_Type.ERROR => "red",
            Log_Type.EXCEPTION => "#bb0000",
            _ => "white"
        };
    }
}
