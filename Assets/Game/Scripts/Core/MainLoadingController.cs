using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainLoadingController : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image fillBar;

    [Header("Audio")]
    [SerializeField] private AudioSource bgMusic;

    [Header("Config")]
    [SerializeField] private string targetScene = "Scene_LevelSelect";
    [SerializeField] private float minLoadTime = 2f;

    private IEnumerator Start()
    {
        if (bgMusic != null) bgMusic.Play();
        if (fillBar != null) fillBar.fillAmount = 0f;

        var op = SceneManager.LoadSceneAsync(targetScene);
        op.allowSceneActivation = false;

        float elapsed = 0f;

        while (elapsed < minLoadTime)
        {
            elapsed += Time.deltaTime;
            if (fillBar != null)
                fillBar.fillAmount = elapsed / minLoadTime;
            yield return null;
        }

        while (op.progress < 0.9f)
            yield return null;

        if (fillBar != null) fillBar.fillAmount = 1f;
        yield return new WaitForSeconds(0.3f);
        op.allowSceneActivation = true;
    }
}