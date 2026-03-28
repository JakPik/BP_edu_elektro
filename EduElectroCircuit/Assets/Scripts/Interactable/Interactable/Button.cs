using System.Collections;
using UnityEngine;

public class Button : MonoBehaviour, IInteractable
{
    [SerializeField] private GenericVoidEventChannel interactionEventChannel;
    [SerializeField] private string interactionInfo;
    [SerializeField] private bool canInteract;
    [SerializeField] private Material material;
    public bool CanInteract() => canInteract;

    public string GetInteractionInfo() => interactionInfo;

    public void OnInteract()
    {
        interactionEventChannel?.RaiseEvent(this.name);
        StartCoroutine(CollorChnage());
    }

    private IEnumerator CollorChnage()
    {
        material.color = Color.green;
        yield return new WaitForSeconds(0.5f);
        material.color = Color.red;
    }
}
