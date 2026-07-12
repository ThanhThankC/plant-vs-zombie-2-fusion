using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(PlantVisualHandler))]
public abstract class PlantBase : MonoBehaviour, IPoolable
{
    [Header("Pool")]
    [SerializeField] private PoolKey plantKey;

    public PlantData Data { get; private set; }
    public PoolKey PlantKey => plantKey;
    public PlantType PlantType => Data.plantType;
    public int CurrentHP { get; private set; }
    public bool IsActivated { get; private set; }
    public Cell OccupiedCell { get; private set; }
    public FieldType OccupiedFieldType { get; private set; }
    public bool IsInvincible { get; private set; }
    public bool CanBeEaten { get; protected set; }
    public bool IsGhost { get; private set; }
    public PlantVisualHandler VisualHandler { get; private set; }

    protected SkeletonAnimation skeletonAnim;
    private bool isReturned;

    protected virtual void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
        VisualHandler = GetComponent<PlantVisualHandler>();
    }

    public void Init(PlantData data)
    {
        Data = data;
        CurrentHP = data.maxHP;
        IsInvincible = data.isInvincible;
        CanBeEaten = !data.notBeEaten;
    }

    public void OnSpawn() => isReturned = false;

    public void SetupAsGhost(Cell cell, FieldType fieldType)
    {
        IsGhost = true;
        enabled = false;

        VisualHandler.SetGhostVisual();

        if (cell == null) return;
        transform.position = cell.transform.position;
        transform.position += fieldType == FieldType.Normal 
            ? new Vector3(-0.2f, 0.5f, 0f) 
            : new Vector3(-0.2f, 0f, 0f);
    }

    public void SetupAsReal(Cell cell, FieldType fieldType)
    {
        IsGhost = false;
        enabled = true;

        if (cell == null) return;
        OccupiedCell = cell;
        OccupiedFieldType = fieldType;
        VisualHandler.SetRealVisual(Data.plantType, cell.Row);

        Vector3 pos;
        float offsetY = fieldType == FieldType.Normal ? 0f : -0.2f;
        pos = transform.position;
        pos.y += offsetY;
        transform.position = pos;
        transform.name = Data.name;
        OnPlaced();
        IsActivated = true;
    }

    protected virtual void OnPlaced() { }

    public virtual void TakeDamage(ZombieBase zombie, int amount)
    {
        CurrentHP -= amount;
        transform.position += new Vector3(0f, 0.01f, 0f);
        if (CurrentHP <= 0) Die(zombie);
    }

    public virtual void Activate() { }

    protected virtual void Die(ZombieBase zombie = null)
    {
        OccupiedCell?.ClearPlant(OccupiedFieldType);
        OccupiedCell = null;
        IsActivated = false;
        ReturnPool();
    }

    private void ReturnPool()
    {
        if (isReturned) return;
        isReturned = true;

        PoolManager.Instance.Release(plantKey, this);
    }

    public void OnDespawn() => enabled = false;
}
