/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: ComponentDataSO
 */

using UnityEngine;

[CreateAssetMenu(fileName = "ComponentData", menuName = "ComponentData", order = 0)]
public class ComponentDataSO : ScriptableObject
{
    public ComponentType type;
}