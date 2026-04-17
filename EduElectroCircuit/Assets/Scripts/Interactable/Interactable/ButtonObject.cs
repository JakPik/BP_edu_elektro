using System.Collections;
using UnityEngine;

public class ButtonObject : MonoBehaviour, IInteractable
{
    #region Variables
    [Header("Control Variables")]
    [SerializeField] private InteractionDisplay interactionDisplay;
    [SerializeField] private Shader buttonShader;
    [SerializeField] private bool canInteract;
    [SerializeField] private bool isPressed;

    [Header("Interaction Types")]
    [SerializeField] private InteractionSO pressOn;

    [SerializeField] private InteractionSO pressOff;

    [Header("Raised Events")]
    [SerializeField] private GenericEventChannel<ButtonPressedEvent> interactionEventChannel;

    private Material material;
    #endregion

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

    public void DisplayInfo(bool display) => interactionDisplay.DisplayInteractionInfo(isPressed ? pressOff : pressOn, display);

    public void OnInteract()
    {
        isPressed = !isPressed;
        interactionEventChannel?.RaiseEvent(new ButtonPressedEvent(isPressed), this.name);
        StartCoroutine(CollorChnage());
        interactionDisplay.UpdateDisplayInfo(isPressed ? pressOff : pressOn);
    }

    private IEnumerator CollorChnage()
    {
        canInteract = false;

        material.color = Color.yellow;
        yield return new WaitForSeconds(0.5f);
        material.color = isPressed ? Color.green : Color.red;

        canInteract = true;
    }
}
