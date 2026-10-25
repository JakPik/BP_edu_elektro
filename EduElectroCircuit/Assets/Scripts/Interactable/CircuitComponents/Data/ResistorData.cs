/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: ResistorDataSO
 */

using UnityEngine;

[System.Serializable]
public class ResistorData : ComponentData
{
    [SerializeField] private ComplexFloat resistance;
    private Color[] colorCode = new Color[5];

    public ResistorData(ComplexFloat resistance)
    {
        this.resistance = resistance;
        CalculateColorCode();
        type = ComponentType.RESISTOR;
    }

    private void CalculateColorCode()
    {
        int multiplier = 0;
        float R = resistance.real;
        while (R >= 1000)
        {
            R /= 10;
            multiplier++;
        }
        int firstDigit = (int)R / 100;
        int secondDigit = ((int)R / 10) - 10 * firstDigit;
        int thirdDigit = (int)(R - 100 * firstDigit - 10 * secondDigit);

        colorCode[0] = ResistorColorCode.GetColor(firstDigit);
        colorCode[1] = ResistorColorCode.GetColor(secondDigit);
        colorCode[2] = ResistorColorCode.GetColor(thirdDigit);
        colorCode[3] = ResistorColorCode.GetColor(multiplier);
        colorCode[4] = Color.black;
    }

    public Color[] GetColorCode() => colorCode;

    public ComplexFloat GetResistance() => resistance;
}