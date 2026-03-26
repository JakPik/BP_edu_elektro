using UnityEngine;

public class Resistor : CircuitComponent, IInteractable
{
    [SerializeField] private string interactionInfo;
    public bool CanInteract()
    {
        return !interactionLocked;
    }

    public string GetInteractionInfo()
    {
        return interactionInfo;
    }

    public void OnInteract()
    {
        throw new System.NotImplementedException();
    }

    protected override void InteractionLock(CircuitUpdateEvent @event)
    {
        interactionLocked = @event.CircuitActive;
    }
}
