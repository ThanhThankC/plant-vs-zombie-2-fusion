using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : PersistentSingleton<SceneLoader>
{
    [SerializeField] private string loadingTransitionScene = "Scene_LoadingTransition";
    [SerializeField] private string gameScene = "Scene_Game";

    public void GoToLevel(int levelIndex)
    {
        GameSettings.SelectedLevel = levelIndex;
        GoTo(gameScene);
    }

    public void GoTo(string targetScene)
    {
        GameSettings.TransitionTargetScene = targetScene;
        SceneManager.LoadScene(loadingTransitionScene);
    }
}