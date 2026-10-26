
using System.Linq;
using UnityEngine.UIElements;

public static class UIStyleControler {
    public static void StyleLabel(Label label, LabelStyles labelStyle)
    {
        string[] classes = label.GetClasses().ToArray();
        foreach (string className in classes)
        {
            if (className != "text-base")
                label.RemoveFromClassList(className);
        }
        label.AddToClassList(LabelStylesExt.ToClassString(labelStyle));
    }
}