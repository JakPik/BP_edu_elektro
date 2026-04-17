using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractionDisplay : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private bool canRotate = true;
    private Label interactionDescription;
    private Label interactionKey;
    private VisualElement _display;
    private GameObject mainCamera;

    private void Start()
    {
        interactionDescription = uiDocument.rootVisualElement.Q<Label>("Description");
        interactionKey = uiDocument.rootVisualElement.Q<Label>("InteractionKey");
        _display = uiDocument.rootVisualElement.Q<VisualElement>("Display");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    void Update()
    {
        if (canRotate)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }
    }

    public void DisplayInteractionInfo(InteractionSO interactionData, bool display)
    {
        interactionDescription.text = interactionData.description;
        interactionKey.text = interactionData.interactionKey;

        /*if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeInOut(display, interactionData.animationSpeed));*/

        if (display)
        {
            _display.RemoveFromClassList("container_hidden");
        }
        else
        {
            _display.AddToClassList("container_hidden");
        }
    }

    public void UpdateDisplayInfo(InteractionSO interactionData)
    {
        interactionDescription.text = interactionData.description;
        interactionKey.text = interactionData.interactionKey;
    }

    private IEnumerator FadeInOut(bool fadeIn, float animationSpeed)
    {
        float time = fadeIn ? 0 : 1;
        float progressiong = 0f;
        if (fadeIn) this._display.visible = true;
        while (progressiong < 1)
        {
            progressiong += Time.deltaTime * animationSpeed;
            time += Time.deltaTime * animationSpeed * (fadeIn ? 1 : -1);
            _display.style.opacity = time;
            yield return null;
        }
        if (!fadeIn) this._display.visible = false;
    }
}
