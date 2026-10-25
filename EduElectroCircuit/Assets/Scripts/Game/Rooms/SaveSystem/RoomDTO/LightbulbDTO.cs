using UnityEngine;

[System.Serializable]
public class LightbulbDTO: ComponentDTO
{
    public LightbulbData lightbulbData;

    public LightbulbDTO(Lightbulb lightbulb)
    {
        name = lightbulb.transform.name;
        position = lightbulb.transform.localPosition;
        lightbulbData = lightbulb.GetLightbulbData();
    }

    protected override GameObject CreateInstance(Transform componentsParent)
    {
        GameObject obj = CreateComponent<Lightbulb>(componentsParent);
        ResetComponent(obj);
        return obj;
    }

    protected override void ResetComponent(GameObject obj)
    {
        Lightbulb lightbulbComponent = obj.GetComponent<Lightbulb>();
        lightbulbComponent.LoadLightbulb(lightbulbData);
    }
}