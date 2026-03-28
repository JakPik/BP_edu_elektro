using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    [SerializeField] private GenericVoidEventChannel interactionEventChannel;
    [SerializeField] private string interactionInfo;
    public bool CanInteract()
    {
        throw new System.NotImplementedException();
    }

    public string GetInteractionInfo()
    {
        return interactionInfo;
    }

    public void OnInteract()
    {
        interactionEventChannel?.RaiseEvent(this.name);
    }
}
