using UnityEngine;

[CreateAssetMenu(fileName = "InteractionSO", menuName = "ComponentData/InteractionSO")]
public class InteractionSO : ScriptableObject
{
    [SerializeField] public string description;
    [SerializeField] public string interactionKey;
    [SerializeField] public float animationSpeed;
}