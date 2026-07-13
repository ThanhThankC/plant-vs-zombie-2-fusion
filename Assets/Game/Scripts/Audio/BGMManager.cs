using System.Collections;
using UnityEngine;

public class BGMManager : Singleton<BGMManager>
{
    [Header("BGM – Clips")]
    [SerializeField] private AudioClip bgmSelection;
    [SerializeField] private AudioClip bgmBattleEarly; 
    [SerializeField] private AudioClip bgmBattleLate;   
    [SerializeField] private AudioClip bgmBattleFinal;  
    [SerializeField] private AudioClip bgmWin;
    [SerializeField] private AudioClip bgmLoss;


    [SerializeField] private AudioSource bgmSourceA;
    [SerializeField] private AudioSource bgmSourceB;

    [Header("BGM – Settings")]
    [SerializeField] private float fadeDuration = 1.0f;
    [SerializeField] private float masterVolume = 1.0f;

    [Header("BGM – Wave Thresholds")]
    [SerializeField] private int midWaveThreshold = 2;
    [SerializeField] private int finalWaveOffset = 1;


    private AudioSource bgmActive;
    private Coroutine fadeCoroutine;
    private AudioClip lastBattleClip;


    protected override void Awake()
    {
        base.Awake();
        SetupSources();
    }

    private void SetupSources()
    {
        if (bgmSourceA == null || bgmSourceB == null)
            Debug.LogWarning("[BMGManager] Not found AudioSource");

        bgmSourceA.volume = masterVolume;
        bgmSourceB.volume = 0f;
        bgmActive = bgmSourceA;
    }


    public void PlaySelection() => PlayImmediate(bgmSelection);
    public void PlayBattleEarly()
    {
        lastBattleClip = bgmBattleEarly;
        PlayImmediate(bgmBattleEarly);
    }
    public void PlayWin() => PlayImmediate(bgmWin, loop: false);
    public void PlayLoss() => PlayImmediate(bgmLoss, loop: false);

    public void Pause() => bgmActive?.Pause();
    public void Resume() => bgmActive?.UnPause();
    public void StopAll() => StopAllBGM();

    public void FadeOut(float duration) => FadeTo(0f, duration);

    public void OnWaveChanged(float progress, int bigWaveIndex, int totalWaves)
    {
        bool isFinalWave = totalWaves > 0 && bigWaveIndex >= totalWaves - 1 - finalWaveOffset;
        bool isMidWave = bigWaveIndex >= midWaveThreshold;

        AudioClip target = bgmBattleEarly;
        if (isFinalWave && bgmBattleFinal != null) target = bgmBattleFinal;
        else if (isMidWave && bgmBattleLate != null) target = bgmBattleLate;

        if (lastBattleClip == target) return;

        lastBattleClip = target;
        CrossFade(target);
    }

    private void PlayImmediate(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        if (fadeCoroutine != null) { StopCoroutine(fadeCoroutine); fadeCoroutine = null; }

        bgmSourceA.Stop(); bgmSourceA.volume = 0f;
        bgmSourceB.Stop(); bgmSourceB.volume = 0f;

        bgmActive = bgmSourceA;
        bgmActive.clip = clip;
        bgmActive.loop = loop;
        bgmActive.volume = masterVolume;
        bgmActive.Play();
    }

    private void CrossFade(AudioClip clip, bool loop = true)
    {
        if (clip == null) return;

        AudioSource incoming = (bgmActive == bgmSourceA) ? bgmSourceB : bgmSourceA;
        AudioSource outgoing = bgmActive;

        incoming.clip = clip;
        incoming.loop = loop;
        incoming.volume = 0f;
        incoming.Play();

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(CrossFadeRoutine(outgoing, incoming));

        bgmActive = incoming;
    }

    private void FadeTo(float targetVolume, float duration)
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeRoutine(bgmActive, targetVolume, duration));
    }

    private void StopAllBGM()
    {
        if (fadeCoroutine != null) { StopCoroutine(fadeCoroutine); fadeCoroutine = null; }
        bgmSourceA.Stop();
        bgmSourceB.Stop();
    }


    private IEnumerator CrossFadeRoutine(AudioSource outgoing, AudioSource incoming)
    {
        float elapsed = 0f;
        float startOut = outgoing.volume;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            outgoing.volume = Mathf.Lerp(startOut, 0f, t);
            incoming.volume = Mathf.Lerp(0f, masterVolume, t);
            yield return null;
        }

        outgoing.Stop();
        outgoing.volume = 0f;
        incoming.volume = masterVolume;
        fadeCoroutine = null;
    }

    private IEnumerator FadeRoutine(AudioSource source, float targetVolume, float duration)
    {
        if (source == null) yield break;

        float startVolume = source.volume;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            source.volume = Mathf.Lerp(startVolume, targetVolume, Mathf.Clamp01(elapsed / duration));
            yield return null;
        }

        source.volume = targetVolume;
        if (targetVolume <= 0f) source.Stop();
        fadeCoroutine = null;
    }
}