using UnityEngine;
using UnityEngine.UI;

public class GameUIController : MonoBehaviour
{
    [Header("Music")]
    [SerializeField] private Slider musicSlider;
    [Header("SFX")]
    [SerializeField] private Slider sfxSlider;

    [SerializeField] private AudioSource sfxSource;

    public void PlayUISfx(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, GameSettings.MasterSfxVolume);
    }

    public void OnSfxSliderChanged(float value) => GameSettings.SetSfxVolume(value);

    public void OnMusicSliderChanged(float value) => GameSettings.SetMusicVolume(value);

    public void OnPauseButtonClicked()
    {
        sfxSlider.value = GameSettings.MasterSfxVolume;
        musicSlider.value = GameSettings.MasterMusicVolume;
        GameFlowManager.Instance.OnPauseButtonClicked();
    }
}
