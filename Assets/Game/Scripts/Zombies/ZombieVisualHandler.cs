using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GlowType { None, Freeze, Poison, Mix }

public class ZombieVisualHandler : MonoBehaviour
{
    [System.Serializable]
    private struct GlowPreset
    {
        public Color skeletonTint;
        public Color glowColor;
        [Range(0, 10)] public float glowIntensity;
        [Range(0, 10)] public float flashIntensity;
        [Range(1, 5)] public float glowContrast;
    }

    [Header("Glow Presets")]
    [SerializeField]
    private GlowPreset freezeGlow = new GlowPreset
    {
        skeletonTint = new Color(0.09f, 0.44f, 0.8f),
        glowColor = new Color(0.19f, 0.62f, 0.87f),
        glowIntensity = 5f,
        flashIntensity = 9f,
        glowContrast = 1f
    };
    [SerializeField]
    private GlowPreset poisonGlow = new GlowPreset
    {
        skeletonTint = Color.magenta,
        glowColor = new Color(1f, 0.7f, 0.9f),
        glowIntensity = 0.4f,
        flashIntensity = 6f,
        glowContrast = 1f
    };
    [SerializeField]
    private GlowPreset mixGlow = new GlowPreset
    {
        skeletonTint = new Color(0.26f, 0.19f, 0.92f),
        glowColor = new Color(0.34f, 0.34f, 1f),
        glowIntensity = 2f,
        flashIntensity = 6f,
        glowContrast = 1f
    };

    [SerializeField] private SpriteRenderer freezeEffect;
    [SerializeField] private SpriteRenderer shadow;

    public GameObject FreezeEffect => freezeEffect.gameObject;
    public GameObject Shadow => shadow.gameObject;

    private SkeletonAnimation skeletonAnim;
    private MeshRenderer meshRenderer;
    private Material glowMaterial;

    private GlowType currentGlow = GlowType.None;
    private Coroutine flashCoroutine;
    private const float flashDuration = 0.09f;
    private const float baseFlashIntensity = 3f;

    private static readonly int PropGlowColor = Shader.PropertyToID("_GlowColor");
    private static readonly int PropGlowIntensity = Shader.PropertyToID("_GlowIntensity");
    private static readonly int PropGlowContrast = Shader.PropertyToID("_GlowContrast");

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
        meshRenderer = skeletonAnim.GetComponent<MeshRenderer>();
        shadow.sortingOrder = SortingOrderUtility.GetSortingOrder(LayerType.Shadow);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            ApplyGlowPreset(GlowType.Freeze);
        if (Input.GetKeyDown(KeyCode.Alpha2))
            ApplyGlowPreset(GlowType.Poison);
        if (Input.GetKeyDown(KeyCode.Alpha4))
            ResetGlowPreset();
    }

    private void OnDisable()
    {
        DisposeMaterial();
    }

    public void Init(Cell cell)
    {
        meshRenderer.sortingOrder = SortingOrderUtility.GetSortingOrder(LayerType.Zombie, cell.Row);
        freezeEffect.sortingOrder = SortingOrderUtility.GetSortingOrder(LayerType.ZombieEffect, cell.Row);
    }

    private void EnsureGlowMaterial()
    {
        if (glowMaterial != null) return;

        var renderer = skeletonAnim.GetComponent<MeshRenderer>();
        if (renderer == null) return;

        var originalMaterial = renderer.sharedMaterial;
        if (originalMaterial == null) return;

        glowMaterial = new Material(originalMaterial);
        skeletonAnim.CustomMaterialOverride[originalMaterial] = glowMaterial;
    }

    public void ApplyGlowPreset(GlowType glowType)
    {
        EnsureGlowMaterial();
        if (glowMaterial == null) return;

        GlowPreset glow = GetGlowPreset(glowType);
        skeletonAnim.Skeleton.SetColor(glow.skeletonTint);
        glowMaterial.SetColor(PropGlowColor, glow.glowColor);
        glowMaterial.SetFloat(PropGlowIntensity, glow.glowIntensity);
        glowMaterial.SetFloat(PropGlowContrast, glow.glowContrast);
    }

    public void ResetGlowPreset()
    {
        EnsureGlowMaterial();
        if (glowMaterial == null) return;

        currentGlow = GlowType.None;
        skeletonAnim.Skeleton.SetColor(Color.white);
        glowMaterial.SetColor(PropGlowColor, Color.white);
        glowMaterial.SetFloat(PropGlowIntensity, 0f);
        glowMaterial.SetFloat(PropGlowContrast, 1f);
    }

    private GlowPreset GetGlowPreset(GlowType type)
    {
        if (currentGlow == GlowType.Mix || (currentGlow != GlowType.None && currentGlow != type))
        {
            currentGlow = GlowType.Mix;
            return mixGlow;
        }

        currentGlow = type;
        return type switch
        {
            GlowType.Freeze => freezeGlow,
            GlowType.Poison => poisonGlow,
            _ => mixGlow
        };
    }

    public void FlashHit()
    {
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        EnsureGlowMaterial();
        if (glowMaterial == null) yield break;

        glowMaterial.SetFloat(PropGlowIntensity, GetInsensity(true));

        yield return new WaitForSeconds(flashDuration);

        glowMaterial.SetFloat(PropGlowIntensity, GetInsensity(false));
        flashCoroutine = null;
    }

    private float GetInsensity(bool isFlash)
    {
        return currentGlow switch
        {
            GlowType.Freeze => isFlash ? freezeGlow.flashIntensity : freezeGlow.glowIntensity,
            GlowType.Poison => isFlash ? poisonGlow.flashIntensity : poisonGlow.glowIntensity,
            GlowType.Mix => isFlash ? mixGlow.flashIntensity : mixGlow.glowIntensity,
            _ => isFlash ? baseFlashIntensity : 0,
        };
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
