/*
 * Edukativní hra zaměřená na elektrické obvody
 * Author: Jakub Pikal
 * Year: 2026
 * Module: MainMenuUIControl
 */

using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

using System.Collections.Generic;

public class MainMenuUIControl : MonoBehaviour
{
    [SerializeField] private UIDocument uiDocument;
    [SerializeField] private string mainGameSceneName = "DemoScene";

    

    [SerializeField] private LevelSelect _levelSelect;
    [SerializeField] private MainMenu _mainMenu;

    private VisualElement _fadePanel;    
    private void Awake() {
        _fadePanel = uiDocument.rootVisualElement.Q<VisualElement>("FadeScreen");
        _fadePanel = uiDocument.rootVisualElement.Q<VisualElement>("FadeScreen");

        _levelSelect.InitLevelSelect(uiDocument.rootVisualElement.Q<VisualElement>("LevelSelectContainer"), this);
        _mainMenu.InitMainMenu(uiDocument.rootVisualElement.Q<VisualElement>("MainMenuContainer"), this);

        StartCoroutine(FadeOut());
        
        _levelSelect.PopulateLevelSelect();

        PersistanceManager.InitPersistance();
    }

    public void Transition(MainMenuTransitions transition)
    {
        StartCoroutine(AnimateTransition(transition));
    }

    private IEnumerator AnimateTransition(MainMenuTransitions transition)
    {
        switch(transition)
        {
            case MainMenuTransitions.TO_GAME:
                yield return StartCoroutine(LoadMainGameScene(PersistanceManager.GetLevelIDToLoad()));
                break;
            case MainMenuTransitions.MAIN_MENU_TO_CONTINUE_GAME:
                break;
            case MainMenuTransitions.MAIN_MENU_TO_LEVELS:
                yield return StartCoroutine(_mainMenu.AnimateTransition());
                yield return new WaitForSeconds(0.1f);
                yield return StartCoroutine(_levelSelect.AnimateTransition());
                break;
            case MainMenuTransitions.LEVELS_TO_MAIN_MENU:
                yield return StartCoroutine(_levelSelect.AnimateTransition());
                yield return new WaitForSeconds(0.1f);
                yield return StartCoroutine(_mainMenu.AnimateTransition());
                break;
            default:
                break;
        }
    }

    private IEnumerator LoadMainGameScene(string scene)
    {
        _fadePanel.AddToClassList("fade-in");
        while (_fadePanel.resolvedStyle.opacity != 1f)
        {
            yield return null;
        }
        UnityEngine.SceneManagement.SceneManager.LoadScene(scene);
    }

    private IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(1);
        _fadePanel.RemoveFromClassList("fade-in");
    }
}