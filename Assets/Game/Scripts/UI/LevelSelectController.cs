using UnityEngine;
using UnityEngine.UI;

public class LevelSelectController : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private GameObject settingPanel;
    [Header("Music")]
    [SerializeField] private Slider musicSlider;
    [Header("SFX")]
    [SerializeField] private Slider sfxSlider;
    [Header("Audio")]
    [SerializeField] private AudioClip bgClip;
    [SerializeField] private AudioSource bgMusic;
    [SerializeField] private AudioSource sfxSource; 

    private void Start()
    {
        PlayBgMusic();
    }

    public void PlayBgMusic()
    {
        if (bgMusic == null || bgClip == null) return;
        bgMusic.clip = bgClip;
        bgMusic.loop = true;
        bgMusic.volume = GameSettings.MasterMusicVolume;
        bgMusic.Play();
    }

    public void StopBgMusic()
    {
        if (bgMusic != null) bgMusic.Stop();
    }

    public void PlaySfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, GameSettings.MasterSfxVolume);
    }

    public void OnShowSettingPanel(bool isShow) => SetPanelActive(settingPanel, isShow);
    public void OnLevelClicked(int levelIndex) => SceneLoader.Instance.GoToLevel(levelIndex);
    public void OnSfxSliderChanged(float value) => GameSettings.SetSfxVolume(value);

    public void OnMusicSliderChanged(float value)
    {
        GameSettings.SetMusicVolume(value);
        if (bgMusic != null) bgMusic.volume = value;
    }

    public void OnBackToMenuClicked() => SceneLoader.Instance.GoTo("Scene_MainMenu");

    private void SetPanelActive(GameObject go, bool active)
    {
        sfxSlider.value = GameSettings.MasterSfxVolume;
        musicSlider.value = GameSettings.MasterMusicVolume;
        if (go != null) go.SetActive(active);
    }
}