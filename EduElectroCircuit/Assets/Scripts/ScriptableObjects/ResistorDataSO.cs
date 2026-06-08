/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: ResistorDataSO
 */

using UnityEngine;

[CreateAssetMenu(fileName = "Resistor data", menuName = "ComponentData/Resistor data", order = 0)]
public class ResistorDataSO : ComponentDataSO
{
    public float resistance;
    public Color[] colorCode = new Color[5];

    [ContextMenu("Calculate color code")]
    public void CalculateColorCode()
    {
        int multiplier = 0;
        float R = resistance;
        while (resistance >= 1000)
        {
            resistance /= 10;
            multiplier++;
        }
        int firstDigit = (int)resistance / 100;
        int secondDigit = ((int)resistance / 10) - 10 * firstDigit;
        int thirdDigit = (int)(resistance - 100 * firstDigit - 10 * secondDigit);

        colorCode[0] = ResistorColorCode.GetColor(firstDigit);
        colorCode[1] = ResistorColorCode.GetColor(secondDigit);
        colorCode[2] = ResistorColorCode.GetColor(thirdDigit);
        colorCode[3] = ResistorColorCode.GetColor(multiplier);
        colorCode[4] = Color.black;
        resistance = R;
    }
}

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