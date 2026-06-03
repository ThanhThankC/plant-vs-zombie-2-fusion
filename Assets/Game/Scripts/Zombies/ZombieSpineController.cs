using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class ZombieSpineController : MonoBehaviour
{
    [System.Serializable]
    private struct SkinEntry
    {
        public string skinName;
        public bool activeByDefault;
    }

    [System.Serializable]
    public struct GlowPreset
    {
        public Color skeletonTint;
        public Color glowColor;
        [Range(0, 10)] public float glowIntensity;
        [Range(1, 5)] public float glowContrast;
    }

    [Header("Skin Config")]
    [SerializeField] private string[] alwaysOnSkins = { "default" };
    [SerializeField] private SkinEntry[] toggleableSkins;

    [Header("Glow Presets")]
    [SerializeField]
    private GlowPreset freezePreset = new GlowPreset
    {
        skeletonTint = Color.blue,
        glowColor = new Color(0.3f, 0.65f, 0.85f),
        glowIntensity = 5f,
        glowContrast = 1f
    };
    [SerializeField]
    private GlowPreset poisonPreset = new GlowPreset
    {
        skeletonTint = Color.magenta,
        glowColor = new Color(1f, 0.7f, 0.9f),
        glowIntensity = 0.4f,
        glowContrast = 1f
    };

    private SkeletonAnimation skeletonAnim;
    private Material glowMaterial;

    private readonly Dictionary<string, Skin> skinCache = new();
    private readonly Dictionary<string, bool> skinToggles = new();
    private Skin runtimeSkin;
    private bool skinDirty;

    private static readonly int PropGlowColor = Shader.PropertyToID("_GlowColor");
    private static readonly int PropGlowIntensity = Shader.PropertyToID("_GlowIntensity");
    private static readonly int PropGlowContrast = Shader.PropertyToID("_GlowContrast");

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();

        //Material originalMaterial = skeletonAnim.GetComponent<MeshRenderer>().sharedMaterial;
        //glowMaterial = new Material(originalMaterial);
        //skeletonAnim.CustomMaterialOverride[originalMaterial] = glowMaterial;
    }

    private void Start()
    {
        CacheAllSkins();
        runtimeSkin = new Skin("mix_runtime");

        foreach (var entry in toggleableSkins)
            skinToggles[entry.skinName] = entry.activeByDefault;

        skinDirty = true;
        ApplySkinIfDirty();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ApplyGlowPreset(freezePreset);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ApplyGlowPreset(poisonPreset);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ResetGlowPreset();
    }

    public void ApplyGlowPreset(GlowPreset preset)
    {
        skeletonAnim.Skeleton.SetColor(preset.skeletonTint);
        glowMaterial.SetColor(PropGlowColor, preset.glowColor);
        glowMaterial.SetFloat(PropGlowIntensity, preset.glowIntensity);
        glowMaterial.SetFloat(PropGlowContrast, preset.glowContrast);
    }

    public void ResetGlowPreset()
    {
        skeletonAnim.Skeleton.SetColor(Color.white);
        glowMaterial.SetColor(PropGlowColor, Color.white);
        glowMaterial.SetFloat(PropGlowIntensity, 0f);
        glowMaterial.SetFloat(PropGlowContrast, 1f);
    }

    private void LateUpdate()
    {
        ApplySkinIfDirty();
    }

    private void OnDestroy()
    {
        DisposeMaterial();
    }

    public void SetSkinActive(string skinName, bool active)
    {
        if (skinToggles.TryGetValue(skinName, out var cus) && cus == active) return;
        skinToggles[skinName] = active;

        if (!skinCache.TryGetValue(skinName, out var skin))
            TryCacheSkin(skeletonAnim.Skeleton.Data, skinName);

        skinDirty = true;
    }

    public void SetSkinInGroup(string[] group, string activeSkin)
    {
        foreach (var s in group)
            SetSkinActive(s, s == activeSkin);
    }

    public void SetTint(Color color)
    {
        skeletonAnim.Skeleton.SetColor(color);
    }

    public void ResetTint()
    {
        skeletonAnim.Skeleton.SetColor(Color.white);
    }

    private void CacheAllSkins()
    {
        var data = skeletonAnim.Skeleton.Data;
        foreach (var s in alwaysOnSkins) TryCacheSkin(data, s);
        foreach (var entry in toggleableSkins) TryCacheSkin(data, entry.skinName);
    }

    private void TryCacheSkin(SkeletonData data, string name)
    {
        var skin = data.FindSkin(name);
        if (skin != null) skinCache[name] = skin;
        else Debug.LogWarning($"[ZombieSpineController] Not found skin: {name}!");
    }

    private void ApplySkinIfDirty()
    {
        if (!skinDirty) return;
        skinDirty = false;

        runtimeSkin.Clear();
        foreach (var s in alwaysOnSkins)
            if (skinCache.TryGetValue(s, out var skin)) runtimeSkin.AddSkin(skin);

        foreach (var (name, isOn) in skinToggles)
            if (isOn && skinCache.TryGetValue(name, out var skin)) runtimeSkin.AddSkin(skin);

        skeletonAnim.Skeleton.SetSkin(runtimeSkin);
        skeletonAnim.Skeleton.SetSlotsToSetupPose();
    }

    private void DisposeMaterial()
    {
        if (glowMaterial == null) return;

        var sharedMat = skeletonAnim.GetComponent<MeshRenderer>().sharedMaterial;
        skeletonAnim.CustomMaterialOverride.Remove(sharedMat);

        Destroy(glowMaterial);
        glowMaterial = null;
    }

}
