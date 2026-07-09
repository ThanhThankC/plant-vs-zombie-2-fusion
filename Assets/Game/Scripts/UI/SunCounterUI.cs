using System.Collections;
using TMPro;
using UnityEngine;

public class SunCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI sunText;
    [SerializeField] private float stepInterval = 0.06f;
    [SerializeField] private int stepCount = 5;
    [SerializeField] private int unit = 5;

    private SunManager sunManager;
    private Coroutine animCoroutine;
    private int displayValue;

    private void Start()
    {
        sunManager = SunManager.Instance;
        sunManager.OnSunChanged += SetSun;
        displayValue = sunManager.TotalSun;
        UpdateText();
    }

    private void OnDestroy()
    {
        if (sunManager != null)
            sunManager.OnSunChanged -= SetSun;
    }

    private void SetSun(int newValue)
    {
        if (displayValue == newValue) return;

        if (animCoroutine != null)
            StopCoroutine(animCoroutine);

        animCoroutine =  StartCoroutine(AnimTo(newValue));
    }

    IEnumerator AnimTo(int targetValue)
    {
        int diff = targetValue - displayValue;

        var steps = SplitIntoParts(diff, stepCount);

        for (int i = 0; i < steps.Length; i++)
        {
            displayValue += steps[i];
            UpdateText();
            yield return new WaitForSeconds(stepInterval);
        }

        displayValue = targetValue;
        UpdateText();
        animCoroutine = null;
    }

    private int[] SplitIntoParts(int diff, int parts)
    {
        int units = diff / unit;
        int baseUnit = units / parts;

        var results = new int[parts];
        for (int i = 0; i < parts; i++)
        {
            int u = baseUnit;
            if (i == 1) u --;
            if (i == 3) u ++;
            results[i] = u * unit;
        }
        return results;
    }

    private void UpdateText()
    {
        sunText.text = displayValue.ToString();
    }
}
