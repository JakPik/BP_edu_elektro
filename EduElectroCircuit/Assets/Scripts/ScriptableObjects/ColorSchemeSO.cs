using UnityEngine;

[CreateAssetMenu(fileName = "ColorScheme", menuName = "UI/ColorScheme")]
public class ColorSchemeSO: ScriptableObject
{
    public Color textPrimary;
    public Color textOutlinePrimary;
    public Color borderPrimary;

    public Color textSecondary;
    public Color textOutlineSecondary;
    public Color borderSecondary;

    public Color Background;

    public Color textSuccess;
    public Color textOutlineSuccess;
    public Color textError;
    public Color textOutlineError;

    public static ColorSchemeSO LoadOrCreate()
    {
        var path = "Assets/Resources/ScriptableObjects/Utils/ColorSchemeSO.asset";
        var colorSchemeSO = UnityEditor.AssetDatabase.LoadAssetAtPath<ColorSchemeSO>(path);
        if (colorSchemeSO == null)
        {
            colorSchemeSO = ScriptableObject.CreateInstance<ColorSchemeSO>();
            UnityEditor.AssetDatabase.CreateAsset(colorSchemeSO, path);
            UnityEditor.AssetDatabase.SaveAssets();
        }
        return colorSchemeSO;
    }
}