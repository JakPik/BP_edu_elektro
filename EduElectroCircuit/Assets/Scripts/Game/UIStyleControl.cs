using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public static class UIStyleControl
{
    private static ColorSchemeSO _instance;
    private static ColorSchemeSO Instance
    {
        get
        {
            if (_instance == null)
                _instance = ColorSchemeSO.LoadOrCreate(); // Loads or creates the singleton ScriptableObject
            return _instance;
        }
    }

    public static Label StyleAsPrimary(Label label)
    {
        label.style.color = Instance.textPrimary;
        label.style.unityTextOutlineColor = Instance.textOutlinePrimary;
        return label;
    }

    public static Label StyleAsSecondary(Label label)
    {
        label.style.color = Instance.textSecondary;
        label.style.unityTextOutlineColor = Instance.textOutlineSecondary;
        return label;
    }

    public static Label StyleAsSuccess(Label label)
    {
        label.style.color = Instance.textSuccess;
        label.style.unityTextOutlineColor = Instance.textOutlineSuccess;
        return label;
    }

    public static Label StyleAsError(Label label)
    {
        label.style.color = Instance.textError;
        label.style.unityTextOutlineColor = Instance.textOutlineError;
        return label;
    }
}