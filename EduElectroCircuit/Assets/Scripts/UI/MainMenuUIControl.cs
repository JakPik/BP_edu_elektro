using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class MainMenuUIControl : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    [SerializeField] string mainGameSceneName = "DemoScene";

    private VisualElement _fadePanel;
    private Button _playButton;
    private void Start()
    {
        _playButton = uiDocument.rootVisualElement.Q<Button>("Play_Button");
        _fadePanel = uiDocument.rootVisualElement.Q<VisualElement>("FadeScreen");
        _playButton.clicked += OnPlayButtonClicked;
        StartCoroutine(FadeOut());
    }

    private void OnPlayButtonClicked()
    {
        StartCoroutine(LoadMainGameScene());
    }

    private IEnumerator LoadMainGameScene()
    {
        _fadePanel.AddToClassList("fade-in");
        while(_fadePanel.resolvedStyle.opacity != 1f) {
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainGameSceneName);
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1);
        _fadePanel.RemoveFromClassList("fade-in");
    }
}


