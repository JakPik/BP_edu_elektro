using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class MainMenuUIControl : MonoBehaviour
{
    [SerializeField] UIDocument uiDocument;
    [SerializeField] string mainGameSceneName = "DemoScene";
    private void Start()
    {
        Button playButton = uiDocument.rootVisualElement.Q<Button>("Play_Button");
        playButton.clicked += OnPlayButtonClicked;
    }

    private void OnPlayButtonClicked()
    {
        StartCoroutine(LoadMainGameScene());
    }

    private IEnumerator LoadMainGameScene()
    {
        float progress = 0f;
        VisualElement panel = uiDocument.rootVisualElement.Q<VisualElement>("FadeScreen");
        panel.visible = true;
        while (progress < 1f)
        {
            progress += Time.deltaTime;
            panel.style.backgroundColor = new Color(0f, 0f, 0f, progress);
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(mainGameSceneName);
    }
}
