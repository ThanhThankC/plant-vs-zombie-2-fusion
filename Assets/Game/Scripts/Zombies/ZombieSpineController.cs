using Spine;
using Spine.Unity;
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

    [Header("Skin Config")]
    [SerializeField] private string[] alwaysOnSkins = { "default" };
    [SerializeField] private SkinEntry[] toggleableSkins;


    private SkeletonAnimation skeletonAnim;

    private readonly Dictionary<string, Skin> skinCache = new();
    private readonly Dictionary<string, bool> skinToggles = new();
    private Skin runtimeSkin;
    private bool skinDirty;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    public void Init()
    {
        if (runtimeSkin == null)
        {
            CacheAllSkins();
            runtimeSkin = new Skin("mix_runtime");
        }

        foreach (var entry in toggleableSkins)
            skinToggles[entry.skinName] = entry.activeByDefault;

        skinDirty = true;
        ApplySkinIfDirty();
    }

    private void LateUpdate()
    {
        ApplySkinIfDirty();
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
}
