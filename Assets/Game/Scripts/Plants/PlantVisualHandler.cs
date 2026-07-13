using Spine.Unity;
using System.Collections;
using UnityEngine;

public class PlantVisualHandler : MonoBehaviour
{
    private SkeletonAnimation skeletonAnim;
    private MeshRenderer meshRenderer;
    private Material glowMaterial;
    private GameObject shadow;

    private Coroutine flashCoroutine;
    private const float flashDuration = 0.09f;
    private const float flashIntensity = 1f;

    private static readonly int PropGlowIntensity = Shader.PropertyToID("_GlowIntensity");

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
        meshRenderer = skeletonAnim.GetComponent<MeshRenderer>();
        shadow = transform.Find("Shadow").gameObject;
        if (shadow == null)
        {
            Debug.LogWarning($"[PlantBase] Not found Shadow Object!");
        }
        shadow.GetComponent<SpriteRenderer>().sortingOrder = SortingOrderUtility.GetSortingOrder(LayerType.Shadow);
    }

    private void EnsureGlowMaterial()
    {
        if (glowMaterial != null) return;
        if (meshRenderer == null) return;

        var originalMaterial = meshRenderer.sharedMaterial;
        if (originalMaterial == null) return;

        glowMaterial = new Material(originalMaterial);
        skeletonAnim.CustomMaterialOverride[originalMaterial] = glowMaterial;
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

        glowMaterial.SetFloat(PropGlowIntensity, flashIntensity);
        yield return new WaitForSeconds(flashDuration);
        ResetGlow();
        flashCoroutine = null;
    }

    private void ResetGlow()
    {
        EnsureGlowMaterial();
        if (glowMaterial == null) return;

        glowMaterial.SetFloat(PropGlowIntensity, 0f);
    }

    public void SetGhostVisual()
    {
        shadow.SetActive(false);
        meshRenderer.sortingOrder = SortingOrderUtility.GetSortingOrder(LayerType.GhostPlant);
        SetAlpha(0.7f);
    }

    public void SetRealVisual(PlantType plantType, int row)
    {
        shadow.SetActive(true);
        var layerSorting = plantType.GetFieldType() == FieldType.Normal
                         ? LayerType.NormalPlant : LayerType.SupportPlant;
        meshRenderer.sortingOrder = SortingOrderUtility.GetSortingOrder(layerSorting, row);
        SetAlpha(1f);
    }

    public void SetDisplayVisual()
    {
        shadow.SetActive(false);
        meshRenderer.sortingOrder = SortingOrderUtility.GetSortingOrder(LayerType.DisplayPlant);
    }

    public void SetAlpha(float alpha)
    {
        var color = skeletonAnim.Skeleton.GetColor();
        color.a = alpha;
        skeletonAnim.Skeleton.SetColor(color);
    }

    public void ResetAll() => ResetGlow();
}
