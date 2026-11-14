using UnityEngine;

[System.Serializable]
public class CapacitorDTO: ComponentDTO
{
    public float capacitance;

    public CapacitorDTO(Capacitor capacitor)
    {
        name = capacitor.transform.name;
        position = capacitor.transform.localPosition;
        capacitance = capacitor.GetCapacitance();
    }

    protected override GameObject CreateInstance(Transform componentsParent)
    {
        GameObject obj = CreateComponent<Capacitor>(componentsParent);
        ResetComponent(obj);
        return obj;
    }

    protected override void ResetComponent(GameObject obj)
    {
        Capacitor capacitorComponent = obj.GetComponent<Capacitor>();
        capacitorComponent.LoadCapacitor(capacitance);
    }
}