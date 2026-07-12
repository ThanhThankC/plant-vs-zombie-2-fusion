using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionLoadingController : MonoBehaviour
{
    [SerializeField] private float minShowTime = 1.0f;

    private IEnumerator Start()
    {
        string target = GameSettings.TransitionTargetScene;

        if (string.IsNullOrEmpty(target))
        {
            Debug.LogError("[TransitionLoading] TransitionTargetScene has not been set! Falling back to Scene_LevelSelect.");
            target = "Scene_LevelSelect";
        }

        GameSettings.TransitionTargetScene = null;

        var op = SceneManager.LoadSceneAsync(target);
        op.allowSceneActivation = false;

        float elapsed = 0f;

        while (op.progress < 0.9f || elapsed < minShowTime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        op.allowSceneActivation = true;
    }
}