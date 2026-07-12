using UnityEngine;

public class LevelSelectController : MonoBehaviour
{
    public void OnLevelClicked(int levelIndex)
    {
        SceneLoader.Instance.GoToLevel(levelIndex);
    }

    public void OnBackToMenuClicked()
    {
        SceneLoader.Instance.GoTo("Scene_MainMenu");
    }
}