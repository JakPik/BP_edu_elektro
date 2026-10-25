using UnityEngine;

[System.Serializable]
public class ResistorDTO: ComponentDTO
{
    public ComplexFloat resistance;

    public ResistorDTO(Resistor resistor)
    {
        name = resistor.transform.name;
        position = resistor.transform.localPosition;
        resistance = resistor.GetResistorResistance();
    }

    protected override GameObject CreateInstance(Transform componentsParent)
    {
        GameObject obj = CreateComponent<Resistor>(componentsParent);
        ResetComponent(obj);
        return obj;
    }

    protected override void ResetComponent(GameObject obj)
    {
        Resistor resistorComponent = obj.GetComponent<Resistor>();
        resistorComponent.LoadResistor(resistance);
    }
}