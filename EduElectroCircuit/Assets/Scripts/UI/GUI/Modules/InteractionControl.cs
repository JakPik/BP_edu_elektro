using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System;

public class InteractionControl
{
    private GameUIControl _gameUIControl;
    private VisualElement _playerGUI;

    private VisualElement _interactionContainer;
    private VisualElement _interactPanel;
    private Label _interactLabel;
    private VisualElement _grabPanel;
    private Label _grabLabel;

    private Coroutine _interactCoroutine;
    private Coroutine _grabCoroutine;

    private bool _interactVisible = false;
    private bool _grabVisible = false;

    public InteractionControl(VisualElement playerGUI, GameUIControl gameUIControl)
    {
        _gameUIControl = gameUIControl;
        _playerGUI = playerGUI;

        _interactionContainer = _playerGUI.Q<VisualElement>("InteractionContainer");

        _interactPanel = _playerGUI.Q<VisualElement>("InteractPanel");
        _grabPanel = _playerGUI.Q<VisualElement>("GrabPanel");

        _interactLabel = _interactPanel.Q<Label>("Label");
        _grabLabel = _grabPanel.Q<Label>("Label");

        _interactPanel.style.display = DisplayStyle.None;
        _grabPanel.style.display = DisplayStyle.None;

        InteractionControlData data = new InteractionControlData
        {
            show = false,
            label = ""
        };

        _interactCoroutine = _gameUIControl.StartCoroutine(AnimateInteractionPanel(data, _interactPanel, () => _interactCoroutine = null));
        _grabCoroutine = _gameUIControl.StartCoroutine(AnimateInteractionPanel(data, _grabPanel, () => _grabCoroutine = null));
    }

    public void UpdateInteraction(InteractionControlData interact, InteractionControlData grab)
    {
        _interactLabel.text = interact.label;
        if(_interactVisible != interact.show) {
            _interactVisible = interact.show;
            _gameUIControl.TerminateCoroutine(_interactCoroutine);
            _interactCoroutine = _gameUIControl.StartCoroutine(AnimateInteractionPanel(interact, _interactPanel, () => _interactCoroutine = null));
        }
        _grabLabel.text = grab.label;
        if(_grabVisible != grab.show) {
            _grabVisible = grab.show;
            _gameUIControl.TerminateCoroutine(_grabCoroutine);
            _grabCoroutine = _gameUIControl.StartCoroutine(AnimateInteractionPanel(grab, _grabPanel, () => _grabCoroutine = null));
        }
    }

    private IEnumerator AnimateInteractionPanel(InteractionControlData data, VisualElement panel, Action onComplete)
    {
        float target = data.show? 1f : 0.1f;
        if(data.show)
        {
            panel.style.display = DisplayStyle.Flex;
        }
        yield return null;
        if(data.show)
        {
            panel.RemoveFromClassList("container_hidden");
        }
        else
        {
            panel.AddToClassList("container_hidden");
        }
        while(panel.resolvedStyle.scale.value.x != target)
        {
            yield return null;
        }
        if(!data.show)
        {
            panel.style.display = DisplayStyle.None;
        }
        onComplete.Invoke();
    }
}

