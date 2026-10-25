using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class LevelTime
{
    [SerializeField] private int _hours;
    [SerializeField] private int _minutes;
    [SerializeField] private int _seconds;
    private string _time;

    public LevelTime(int seconds, int minutes, int hours)
    {
        this._seconds = seconds;
        this._minutes = minutes;
        this._hours = hours;

        _time = "";
        if(_hours > 0) _time += _hours + "h ";
        if(_minutes > 0) _time += _minutes + "min ";
        if(_seconds > 0) _time += _seconds + "s ";
    }

    public string GetTime() => _time;

    public int GetHours() => _hours;
    public int GetMinutes() => _minutes;
    public int GetSeconds() => _seconds;

    public void ImproveTime(LevelTime newTime)
    {
        _hours = newTime.GetHours();
        _minutes = newTime.GetMinutes();
        _seconds = newTime.GetSeconds();
    }
    public bool GetTimeImproved(LevelTime newLevelTime)
    {
        if(GetIsNewTime()) return true;
        if(_hours > newLevelTime.GetHours()) return true;
        else if(_hours != newLevelTime.GetHours()) return false;

        if(_minutes > newLevelTime.GetMinutes()) return true;
        else if(_minutes != newLevelTime.GetMinutes()) return false;
            
        if(_seconds > newLevelTime.GetSeconds()) return true;
        return false;
    }

    private bool GetIsNewTime()
    {
        if(_hours == 0 && _minutes == 0 && _seconds == 0) return true;
        return false;
    }
}