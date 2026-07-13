using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public enum GameState { Idle, CardSelection, BattleIntro, Playing, Paused, Win, Lose }

public class GameFlowManager : Singleton<GameFlowManager>
{
    public GameState CurrentState { get; private set; } = GameState.Idle;
    public event System.Action<GameState> OnStateChanged;

    [Header("BGM – Transition")]
    [SerializeField] private float bgmFadeDuration = 1.0f;


    [Header("Camera")]
    [SerializeField] private float camShiftRightX = 2f;
    [SerializeField] private float camShiftLeftX = -2f;
    [SerializeField] private float camDuration = 0.6f;
    [SerializeField] private Ease camEase = Ease.InOutSine;


    [Header("UI Panel (Card Selection)")]
    [SerializeField] private GameObject selectionPanel;
    [SerializeField] private GameObject startButton;

    [Header("Battle Intro")]
    [SerializeField] private TextMeshProUGUI letsTxt;
    [SerializeField] private TextMeshProUGUI startText;
    [SerializeField] private float introImageDuration = 0.6f;
    [SerializeField] private float introPauseBetween = 0.1f;

    [Header("HUD")]
    [SerializeField] private GameObject hudRoot;

    [Header("Pause")]
    [SerializeField] private GameObject pausePanel;

    [Header("Exit Level")]
    [SerializeField] private GameObject exitPanel;

    [Header("Quit Game")]
    [SerializeField] private GameObject quitPanel;

    [Header("Win")]
    [SerializeField] private GameObject cupObject;

    [Header("Lose")]
    [SerializeField] private GameObject loseOverlay;
    [SerializeField] private GameObject losePanel;
    [SerializeField] private float loseOverlayFadeDuration = 1.2f;

    [Header("Events (ScriptableObject)")]
    [SerializeField] private ZombieReachedEvent onZombieReachedEnd;
    [SerializeField] private LevelClearedEvent onLevelCleared;


    private Transform cameraTransform;
    private Vector3 camOriginalPos;

    protected override void Awake()
    {
        base.Awake();
        cameraTransform = Camera.main.transform;
    }

    private void OnEnable()
    {
        if (onZombieReachedEnd != null) onZombieReachedEnd.OnRaised += TriggerLose;
        if (onLevelCleared != null) onLevelCleared.OnRaised += TriggerWin;
    }

    private void OnDisable()
    {
        if (onZombieReachedEnd != null) onZombieReachedEnd.OnRaised -= TriggerLose;
        if (onLevelCleared != null) onLevelCleared.OnRaised -= TriggerWin;
    }

    private void Start()
    {
        camOriginalPos = cameraTransform.position;
        SetActive(hudRoot, false);
        SetActive(pausePanel, false);
        SetActive(quitPanel, false);
        SetActive(losePanel, false);
        SetActive(loseOverlay, false);
        SetActive(cupObject, false);
        HideIntroImages();
        EnterCardSelection();
    }

    public void OnStartButtonClicked()
    {
        if (CurrentState != GameState.CardSelection) return;
        StartCoroutine(BattleIntroSequence());
    }

    public void OnPauseButtonClicked()
    {
        if (CurrentState == GameState.Playing) EnterPause();
        else if (CurrentState == GameState.Paused) ResumeFromPause();
    }

    public void OnRestartClicked()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.GoToLevel(GameSettings.SelectedLevel);
    }

    public void OnExitToMenuClicked()
    {
        Time.timeScale = 1f;
        SceneLoader.Instance.GoTo("Scene_LevelSelect");
    }
    public void ShowExitToMenuConfirmation(bool isShow) => SetActive(exitPanel, isShow);
    public void ShowQuitGameConfirmation(bool isShow) => SetActive(quitPanel, isShow);
    public void OnQuitGameClicked() => Application.Quit();
    public void OnCupClicked() => BGMManager.Instance?.PlayWin();
    public void OnWinChanged()
    {
        if (CurrentState != GameState.Win) return;
        SceneLoader.Instance.GoTo("Scene_LevelSelect");
    }

    private void EnterCardSelection()
    {
        Vector3 target = camOriginalPos + new Vector3(camShiftRightX, 0f, 0f);
        cameraTransform.DOMove(target, camDuration).SetEase(camEase)
            .OnComplete(() => SetState(GameState.CardSelection));

        SetActive(selectionPanel, true);
        SetActive(startButton, true);
        BGMManager.Instance?.PlaySelection();
    }

    private IEnumerator BattleIntroSequence()
    {
        SetState(GameState.BattleIntro);
        SetActive(selectionPanel, false);
        SetActive(startButton, false);

        Vector3 target = camOriginalPos + new Vector3(camShiftLeftX, 0f, 0f);
        cameraTransform.DOMove(target, camDuration).SetEase(camEase);

        BGMManager.Instance?.FadeOut(bgmFadeDuration);
        yield return new WaitForSeconds(Mathf.Max(camDuration, bgmFadeDuration));

        BGMManager.Instance?.PlayBattleEarly();
        yield return new WaitForSeconds(0.3f);

        yield return ShowIntroImage(letsTxt);
        yield return new WaitForSeconds(introPauseBetween);
        yield return ShowIntroImage(startText);

        EnterPlaying();
    }

    private void EnterPlaying()
    {
        SetState(GameState.Playing);
        SetActive(hudRoot, true);

        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveChanged += OnWaveChanged;

        WaveManager.Instance?.StartLevel();
        SunSpawner.Instance.StartSpawning();
    }

    private void EnterPause()
    {
        SetState(GameState.Paused);
        Time.timeScale = 0f;
        BGMManager.Instance?.Pause();
        SetActive(pausePanel, true);
    }

    private void ResumeFromPause()
    {
        SetState(GameState.Playing);
        Time.timeScale = 1f;
        BGMManager.Instance?.Resume();
        SetActive(pausePanel, false);
    }

    private void TriggerWin()
    {
        if (CurrentState != GameState.Playing) return;
        StartCoroutine(WinSequence());
    }

    private void TriggerLose()
    {
        if (CurrentState != GameState.Playing) return;
        StartCoroutine(LoseSequence());
    }

    private IEnumerator WinSequence()
    {
        SetState(GameState.Win);
        UnsubscribeWaveEvents();

        BGMManager.Instance?.StopAll();

        SetActive(cupObject, true);
        yield return null;
    }

    private IEnumerator LoseSequence()
    {
        SetState(GameState.Lose);
        UnsubscribeWaveEvents();

        Time.timeScale = 0f;
        SetActive(loseOverlay, true);

        BGMManager.Instance?.StopAll();
        BGMManager.Instance?.PlayLoss();

        yield return new WaitForSecondsRealtime(loseOverlayFadeDuration + 0.3f);
        SetActive(losePanel, true);
    }

    private void OnWaveChanged(float progress, int bigWaveIndex)
    {
        if (CurrentState != GameState.Playing) return;
        int totalWaves = WaveManager.Instance != null ? WaveManager.Instance.BigWaveCount : -1;
        BGMManager.Instance?.OnWaveChanged(progress, bigWaveIndex, totalWaves);
    }

    private void UnsubscribeWaveEvents()
    {
        if (WaveManager.Instance != null)
            WaveManager.Instance.OnWaveChanged -= OnWaveChanged;
    }

    private IEnumerator ShowIntroImage(TextMeshProUGUI txt)
    {
        if (txt == null) yield break;
        txt.gameObject.SetActive(true);
        yield return new WaitForSeconds(introImageDuration * 0.3f);
        txt.gameObject.SetActive(false);
    }

    private void HideIntroImages()
    {
        SetActive(letsTxt?.gameObject, false);
        SetActive(startText?.gameObject, false);
    }

    private void SetState(GameState s)
    {
        CurrentState = s;
        OnStateChanged?.Invoke(s);
        Debug.Log($"[GameFlowManager] State: {s}");
    }

    private static void SetActive(GameObject go, bool active)
    {
        if (go != null) go.SetActive(active);
    }
}