using UnityEngine;

public interface IInteractable
{
    void OnInteract();
    bool CanInteract();
    string GetInteractionInfo();
}
