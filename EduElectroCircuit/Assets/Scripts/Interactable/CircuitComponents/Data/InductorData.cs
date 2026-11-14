/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: CapacitorData
 */

using UnityEngine;

[System.Serializable]
public class InductorData : ComponentData
{
    [SerializeField] private float inductance;
    [SerializeField] private int numberOfTurns;
    public float Inductance => inductance;
    public int NumberOfTurns => numberOfTurns;

    public InductorData(float inductance, int numberOfTurns)
    {
        this.inductance = inductance;
        this.numberOfTurns = numberOfTurns;
        type = ComponentType.INDUCTOR;
    }
}