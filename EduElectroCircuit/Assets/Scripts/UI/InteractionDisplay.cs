/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: InteractionDisplay
 */

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class InteractionDisplay : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private GameObject uiContainer;
    [SerializeField] private bool canRotate = true;
    [SerializeField] private bool display = false;
    private Label interactionDescription;
    private Label interactionKey;
    private VisualElement _display;
    private GameObject mainCamera;
    private Coroutine lookRoutine = null;

    private void Start()
    {
        interactionDescription = uiDocument.rootVisualElement.Q<Label>("Description");
        interactionKey = uiDocument.rootVisualElement.Q<Label>("InteractionKey");
        _display = uiDocument.rootVisualElement.Q<VisualElement>("Display");
        mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    public void DisplayInteractionInfo(InteractionSO interactionData, bool show)
    {
        display = show;
        interactionDescription.text = interactionData.description;
        interactionKey.text = interactionData.interactionKey;

        /*if(fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        fadeCoroutine = StartCoroutine(FadeInOut(display, interactionData.animationSpeed));*/

        if (display)
        {
            if (canRotate) lookRoutine = StartCoroutine(UpdatePanelOrientation());
            _display.RemoveFromClassList("container_hidden");
        }
        else
        {
            if (lookRoutine != null) StopCoroutine(lookRoutine);
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

    private IEnumerator UpdatePanelOrientation()
    {
        while (true)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
            uiContainer.transform.rotation = Quaternion.LookRotation(transform.forward, Vector3.up);
            yield return null;
        }
    }
}
