using System.Collections;
using UnityEngine;

public class ButtonObject : MonoBehaviour, IInteractable
{
    [SerializeField] private GenericEventChannel<ButtonPressedEvent> interactionEventChannel;
    [SerializeField] private string interactionInfo;
    [SerializeField] private bool canInteract;
    [SerializeField] private bool isPressed;
    [SerializeField] private Shader buttonShader;
    private Material material;
    [SerializeField] private InteractionDisplay interactionDisplay;
    [SerializeField] private InteractionSO pressOffInteraction;
    [SerializeField] private InteractionSO pressOnInteraction;

    void Awake()
    {
        material = new Material(buttonShader);

        GetComponent<Renderer>().material = material;
    }

    public void Reset()
    {
        material.color = Color.red;
        isPressed = false;
    }

    public bool CanInteract() => canInteract;

    public void DisplayInfo(bool display) => interactionDisplay.DisplayInteractionInfo(isPressed ? pressOffInteraction : pressOnInteraction, display);

    public void OnInteract()
    {
        if(isPressed)
        {
            interactionEventChannel?.RaiseEvent(new ButtonPressedEvent(false), this.name);
            StartCoroutine(CollorChnage(false));
            isPressed = false;
            interactionDisplay.UpdateDisplayInfo(pressOnInteraction);
        }
        else
        {
            interactionEventChannel?.RaiseEvent(new ButtonPressedEvent(true), this.name);
            StartCoroutine(CollorChnage(true));
            isPressed = true;
            interactionDisplay.UpdateDisplayInfo(pressOffInteraction);
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
