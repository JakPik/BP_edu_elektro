/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: InteractionSO
 */

using UnityEngine;

[CreateAssetMenu(fileName = "InteractionSO", menuName = "ComponentData/InteractionSO")]
public class InteractionSO : ScriptableObject
{
    [SerializeField] public string description;
    [SerializeField] public string interactionKey;
    [SerializeField] public float animationSpeed;
}