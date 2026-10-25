using System.Collections.Generic;
using UnityEngine;

public static class RoomDataManager
{

    public static Resistor ResetGrab(GameObject obj)
    {
        Resistor resistorComp = obj.GetComponent<Resistor>();
            if (resistorComp == null)
            {
                Logger.Log("JSON_RoomModule", "ResetGrab", "Resistor component not found on " + obj.name, LogType.ERROR);
                return null;
            }
        resistorComp.LockGrab(false);
        return resistorComp;
    }
    
    public static void SaveComponentSetUp(Transform componentContainer, ComponentsDTO componentsDTO)
    {
        List<ResistorDTO> resistorDTOs = new List<ResistorDTO>();
        List<LightbulbDTO> lightbulbDTOs = new List<LightbulbDTO>();
        foreach(Transform child in componentContainer)
        {
            if(child.TryGetComponent(out Resistor resistor))
            {
                resistorDTOs.Add(new ResistorDTO(resistor));
                continue;
            }
            if(child.TryGetComponent(out Lightbulb lightbulb))
            {
                lightbulbDTOs.Add(new LightbulbDTO(lightbulb));
            }
        }
        componentsDTO.resistors = resistorDTOs.ToArray();
        componentsDTO.lightbulbs = lightbulbDTOs.ToArray();
    }
    
    public static T[] LoadComponentSetUp<T, U>(U[] componentDTOs, Transform componentsContainer)
        where T: CircuitComponent
        where U: ComponentDTO
    {
        T[] components = new T[componentDTOs.Length];
        int componentLoadID = 0;
        foreach(U componentDTO in componentDTOs)
        {
            GameObject obj = componentDTO.ToGameObject(componentsContainer);
            components[componentLoadID] = obj.GetComponent<T>();
            componentLoadID++;
        }
        return components;
    }
}