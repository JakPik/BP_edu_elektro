using UnityEngine;

[System.Serializable]
public class InductorDTO: ComponentDTO
{
    public float inductance;
    public int numberOfTurns;

    public InductorDTO(Inductor inductor)
    {
        name = inductor.transform.name;
        position = inductor.transform.localPosition;
        inductance = inductor.GetInductance();
        numberOfTurns = inductor.GetNumberOfTurns();
    }

    protected override GameObject CreateInstance(Transform componentsParent)
    {
        GameObject obj = CreateComponent<Inductor>(componentsParent);
        ResetComponent(obj);
        return obj;
    }

    protected override void ResetComponent(GameObject obj)
    {
        Inductor inductorComponent = obj.GetComponent<Inductor>();
        inductorComponent.LoadInductor(inductance, numberOfTurns);
    }
}