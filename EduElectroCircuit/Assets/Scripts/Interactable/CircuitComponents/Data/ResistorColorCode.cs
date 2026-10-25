using UnityEngine;

public static class ResistorColorCode
{
    public static Color GetColor(int digit)
    {
        return digit switch
        {
            0 => Color.black,
            1 => new Color(0.59f, 0.35f, 0.21f),
            2 => Color.red,
            3 => new Color(0.8f, 0.36f, 0.06f), // orange
            4 => Color.yellow,
            5 => Color.green,
            6 => Color.blue,
            7 => new Color(0.91f, 0.11f, 0.91f), // violet
            8 => Color.gray,
            9 => Color.white,
            _ => throw new System.ArgumentException("Invalid digit for color code")
        };
    }
}