using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [System.Serializable]
    private struct Sound
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume;
        [Range(0.5f, 2f)] public float pitch;
        [Range(0f, 0.3f)] public float pitchVariance;
    }

    [System.Serializable]
    private struct HitSound
    {
        public ProjectileHitType type;
        public Sound sound;
    }

    [System.Serializable]
    private struct EffectSound
    {
        public PlantEffectType type;
        public Sound sound;
    }

    [System.Serializable]
    private struct PlantAttackSound
    {
        public PlantAttackType type;
        public Sound sound;
    }

    [Header("Event channels")]
    [SerializeField] private ZombieDiedEvent onZombieDied;
    [SerializeField] private ZombiePartDroppedEvent onPartZombieDropped;
    [SerializeField] private ProjectileHitEvent onProjectileHit;
    [SerializeField] private PlantEffectActivateEvent onEffectActivate;
    [SerializeField] private PlantAttackEvent onPlantAttack;

    [Header("SFX")]
    [SerializeField] private Sound zombieDieSfx;
    [SerializeField] private Sound partZombieDropSfx;
    [SerializeField] private HitSound[] hitSounds;
    [SerializeField] private EffectSound[] effectSounds;
    [SerializeField] private PlantAttackSound[] plantAttackSounds;
    [SerializeField] private Sound sunClickSfx;

    [Header("AudioSource Pool")]
    [SerializeField] private int poolSize = 10;

    private AudioSource[] sourcePool;
    private Dictionary<ProjectileHitType, Sound> hitSoundMap = new();
    private Dictionary<PlantEffectType, Sound> effectSoundMap = new();
    private Dictionary<PlantAttackType, Sound> plantAttackSoundMap = new();

    protected override void Awake()
    {
        base.Awake();
        BuildPool();
        BuildSoundMap();
    }

    private void BuildPool()
    {
        sourcePool = new AudioSource[poolSize];
        for (int i = 0; i < poolSize; i++)
        {
            sourcePool[i] = gameObject.AddComponent<AudioSource>();
            sourcePool[i].playOnAwake = false;
        }
    }

    private void BuildSoundMap()
    {
        foreach (var h in hitSounds) hitSoundMap[h.type] = h.sound;
        foreach (var e in effectSounds) effectSoundMap[e.type] = e.sound;
        foreach (var a in plantAttackSounds) plantAttackSoundMap[a.type] = a.sound;
    }

    private void OnEnable()
    {
        if (onZombieDied != null) onZombieDied.OnRaised += HandleZombieDied;
        if (onPartZombieDropped != null) onPartZombieDropped.OnRaised += HandlePartZombieDropped;
        if (onProjectileHit != null) onProjectileHit.OnRaised += HandleProjectileHit;
        if (onEffectActivate != null) onEffectActivate.OnRaised += HandleEffectActivate;
        if (onPlantAttack != null) onPlantAttack.OnRaised += HandlePlantAttack;
    }

    private void OnDisable()
    {
        if (onZombieDied != null) onZombieDied.OnRaised -= HandleZombieDied;
        if (onPartZombieDropped != null) onPartZombieDropped.OnRaised -= HandlePartZombieDropped;
        if (onProjectileHit != null) onProjectileHit.OnRaised -= HandleProjectileHit;
        if (onPlantAttack != null) onPlantAttack.OnRaised -= HandlePlantAttack;
    }

    private void HandleZombieDied(ZombieBase z) => PlaySound(zombieDieSfx);
    private void HandlePartZombieDropped() => PlaySound(partZombieDropSfx);

    private void HandleProjectileHit(ProjectileHitType type)
    {
        if (!hitSoundMap.TryGetValue(type, out var sound))
        {
            Debug.LogWarning($"[AudioManager]: Not found sound type: {type}");
            return;
        }

        PlaySound(sound);
    }

    private void HandleEffectActivate(PlantEffectType type)
    {
        if (!effectSoundMap.TryGetValue(type, out var sound))
        {
            Debug.LogWarning($"[AudioManager]: Not found sound type: {type}");
            return;
        }

        PlaySound(sound);
    }

    private void HandlePlantAttack(PlantAttackType type)
    {
        if (!plantAttackSoundMap.TryGetValue(type, out var sound))
        {
            Debug.LogWarning($"[AudioManager]: Not found sound type: {type}");
            return;
        }

        PlaySound(sound);
    }

    public void PlaySunlick() => PlaySound(sunClickSfx);

    private AudioSource GetFreeAudioSource()
    {
        foreach (var src in sourcePool)
            if (!src.isPlaying) return src;

        Debug.LogWarning($"[AudioManager] pool exhausted — get oldest source.");
        return sourcePool[0];
    }

    private void PlaySound(Sound sound)
    {
        if (sound.clip == null) return;

        var src = GetFreeAudioSource();
        src.clip = sound.clip;
        src.volume = sound.volume * GameSettings.MasterSfxVolume;
        src.pitch = sound.pitch + Random.Range(-sound.pitchVariance, sound.pitchVariance);
        src.Play();
    }
}