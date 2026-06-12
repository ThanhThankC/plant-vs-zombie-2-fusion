using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Slider progressSlider;
    [SerializeField] private GameObject flagPrefab;
    [SerializeField] private float barWidth = 250f;
    [SerializeField] private float fillSpeed = 3f;

    private List<GameObject> flags = new();
    private WaveManager waveManager;

    private void Start()
    {
        waveManager = WaveManager.Instance;
        progressSlider.gameObject.SetActive(false);
        waveManager.OnBigWaveChanged += UpdateFlag;
        waveManager.OnSmallWaveChanged += UpdateProgressBar;
    }

    private void OnDestroy()
    {
        if (waveManager != null)
        {
            waveManager.OnBigWaveChanged -= UpdateFlag;
            waveManager.OnSmallWaveChanged -= UpdateProgressBar;
        }
    }

    private void UpdateFlag(int bigwave)
    {
        if (bigwave == 0)
        {
            progressSlider.gameObject.SetActive(true);
            SetUpFlag(); 
            return;
        }

        GameObject flag = flags[bigwave - 1];
        if (flag == null) return;
        flag.transform.localPosition += new Vector3(0, 20f, 0);
    }

    private void SetUpFlag()
    {
        int count = waveManager.BigWaveCount;

        for (int i = 1; i < count; i++)
        {
            float offsetX = count <= 1 ? 0f : (float)i / (count - 1) * barWidth;
            Vector3 pos = new Vector3(-offsetX, 0f, 0f);

            GameObject go = Instantiate(flagPrefab, progressSlider.transform);
            go.transform.localPosition = pos;
            go.transform.SetSiblingIndex(2);
            flags.Add(go);
        }
    }

    private void UpdateProgressBar(float targetProgress)
    {
        StopAllCoroutines();
        StartCoroutine(RunProgress(progressSlider, targetProgress));
    }

    IEnumerator RunProgress(Slider bar, float targetProgress)
    {
        while (bar.value < targetProgress)
        {
            bar.value += targetProgress * Time.deltaTime / fillSpeed;
            yield return null;
        }
        bar.value = targetProgress;
    }
}
