using Spine.Unity;
using UnityEngine;

public class PlantVisualHandler : MonoBehaviour
{
    private SkeletonAnimation skeletonAnim;
    private MeshRenderer meshRenderer;
    private GameObject shadow;

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
}
