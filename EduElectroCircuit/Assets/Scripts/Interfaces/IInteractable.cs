using UnityEngine;

public interface IInteractable
{
    void OnInteract();
    bool CanInteract();
    void DisplayInfo(bool display);
}
