using System.Collections;
using UnityEngine;

public class ButtonObject : MonoBehaviour, IInteractable
{
    [SerializeField] private GenericEventChannel<ButtonPressedEvent> interactionEventChannel;
    [SerializeField] private string interactionInfo;
    [SerializeField] private bool canInteract;
    [SerializeField] private bool isPressed;
    [SerializeField] private Material material;
    public bool CanInteract() => canInteract;

    public string GetInteractionInfo() => interactionInfo;

    public void OnInteract()
    {
        if(isPressed)
        {
            interactionEventChannel?.RaiseEvent(new ButtonPressedEvent(false), this.name);
            StartCoroutine(CollorChnage(false));
            isPressed = false;
        }
        else
        {
            interactionEventChannel?.RaiseEvent(new ButtonPressedEvent(true), this.name);
            StartCoroutine(CollorChnage(true));
            isPressed = true;
        }
        
    }

    private IEnumerator CollorChnage(bool pressed)
    {
        canInteract = false;
        if(pressed) {
            material.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            material.color = Color.green;
        }
        else
        {
            material.color = Color.yellow;
            yield return new WaitForSeconds(0.5f);
            material.color = Color.red;
        }
        canInteract = true;
    }
}
