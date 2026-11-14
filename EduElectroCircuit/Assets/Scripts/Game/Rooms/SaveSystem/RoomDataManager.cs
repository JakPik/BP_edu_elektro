using System.Collections.Generic;
using UnityEngine;

public static class RoomDataManager
{
    public static void SaveComponentSetUp(Transform componentContainer, RoomConfigurationDTO roomConfigurationDTO)
    {
        List<ResistorDTO> resistorDTOs = new List<ResistorDTO>();
        List<LightbulbDTO> lightbulbDTOs = new List<LightbulbDTO>();
        List<CapacitorDTO> capacitorDTOs = new List<CapacitorDTO>();
        List<InductorDTO> inductorDTOs = new List<InductorDTO>();
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
            if(child.TryGetComponent(out Capacitor capacitor))
            {
                capacitorDTOs.Add(new CapacitorDTO(capacitor));
            }
            if(child.TryGetComponent(out Inductor inductor))
            {
                inductorDTOs.Add(new InductorDTO(inductor));
            }
        }
        roomConfigurationDTO.resistors = resistorDTOs.ToArray();
        roomConfigurationDTO.lightbulbs = lightbulbDTOs.ToArray();
        roomConfigurationDTO.capacitors = capacitorDTOs.ToArray();
        roomConfigurationDTO.inductors = inductorDTOs.ToArray();
    }
    
    public static T[] LoadComponentSetUp<T, U>(U[] componentDTOs, Transform componentsContainer)
        where T: CircuitComponent
        where U: ComponentDTO
    {
        if(componentDTOs == null) return new T[0];
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

    public static void SaveComponentsCurrentState<T>(T[] components)
        where T: CircuitComponent
    {
        foreach(T component in components)
        {
            component.SaveCurrentState();
        }
    }
}