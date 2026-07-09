using UnityEngine;

public abstract class ProjectileBase : MonoBehaviour
{
    [System.Serializable]
    private struct SpriteEntry
    {
        public SpriteRenderer spriteRenderer;
        public int sortingBonus;
    }

    [Header("References")]
    [SerializeField] protected ProjectileData data;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private SpriteEntry[] sprites;

    [Header("Splat")]
    [SerializeField] private SplatProjectile splatPrefab;
    [SerializeField] private Vector3 spawnOffset;

    public bool HasEffect { get; private set; }

    private bool isSpineRender;
    private GameObject shadow;
    protected IEffect onHitEffect;
    protected Cell spawnCell;

    private const float shadowOffsetY = -0.4f;

    protected virtual void Awake()
    {
        isSpineRender = meshRenderer != null;
        HasEffect = data.hasEffect;

        shadow = transform.Find("Shadow").gameObject;
        shadow.GetComponent<SpriteRenderer>().sortingOrder = SortingOrderUtility.GetSortingOrder(LayerType.Shadow);
    }

    public virtual void Init(Vector3 direction, Cell cell, IEffect effect = null)
    {
        onHitEffect = effect;
        spawnCell = cell;
        InitRenderer(cell.Row);
    }

    private void InitRenderer(int row)
    {
        int sortingOrder = SortingOrderUtility.GetSortingOrder(LayerType.Projectile, row);
        if (isSpineRender)
        {
            meshRenderer.sortingOrder = sortingOrder;
            return;
        }

        if (sprites.Length <= 0) return;
        foreach (var s in sprites)
        {
            s.spriteRenderer.sortingOrder = sortingOrder + s.sortingBonus;
        }
    }

    protected void UpdateShadow()
    {
        if (spawnCell == null) return;
        float groundPosY = spawnCell.transform.position.y + shadowOffsetY;
        shadow.transform.position = new Vector3(transform.position.x, groundPosY, 0f);
        shadow.transform.rotation = Quaternion.identity;
    }

    protected void OnImpact()
    {
        if (splatPrefab == null) return;
        var splat = Instantiate(splatPrefab, transform.position + spawnOffset, Quaternion.identity);
        splat.Init(SortingOrderUtility.GetSortingOrder(LayerType.Splat, spawnCell.Row));
    }
}
