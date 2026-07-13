using UnityEngine;

public class GameSettings : PersistentSingleton<GameSettings>
{
    private const string KEY_SFX = "vol_sfx";
    private const string KEY_MUSIC = "vol_music";

    private static float sfxVolume = 1f;
    private static float musicVolume = 1f;

    public static float MasterSfxVolume => sfxVolume;
    public static float MasterMusicVolume => musicVolume;

    public static int SelectedLevel = 0;
    public static string TransitionTargetScene;

    protected override void Awake()
    {
        base.Awake();
        Load();
    }

    public static void SetSfxVolume(float value)
    {
        sfxVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(KEY_SFX, sfxVolume);
        PlayerPrefs.Save();
    }

    public static void SetMusicVolume(float value)
    {
        musicVolume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(KEY_MUSIC, musicVolume);
        PlayerPrefs.Save();
        BGMManager.Instance?.ApplyVolume();
    }

    private static void Load()
    {
        sfxVolume = PlayerPrefs.GetFloat(KEY_SFX, 1f);
        musicVolume = PlayerPrefs.GetFloat(KEY_MUSIC, 1f);
    }
}