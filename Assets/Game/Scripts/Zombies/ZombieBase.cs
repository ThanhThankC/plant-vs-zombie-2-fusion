using UnityEngine;

[RequireComponent(typeof(ZombieMovement))]
[RequireComponent(typeof(ZombieEffectController))]
[RequireComponent(typeof(ZombieSpineController))]
[RequireComponent(typeof(ZombieAnimationController))]
[RequireComponent(typeof(ZombieVisualHandler))]
public abstract class ZombieBase : MonoBehaviour, IPoolable
{
    [Header("Events")]
    [SerializeField] private ZombieDiedEvent onZombieDied;
    [SerializeField] private ProjectileHitType armorHitType = ProjectileHitType.Metal;

    [Header("References")]
    [SerializeField] private ZombiePoolKey zombieKey;
    [SerializeField] private CellTracker cellTracker;

    [Header("Ash VFX")]
    [SerializeField] private ZombiePoolKey ashKey;
    [SerializeField] private Transform ashPoint;

    public ZombieData Data { get; private set; }
    public ZombieMovement Movement { get; private set; }
    public ZombieEffectController EffectController { get; private set; }
    public ZombieSpineController SpineController { get; private set; }
    public ZombieAnimationController AnimController { get; private set; }
    public ZombieVisualHandler VisualHandler { get; private set; }
    public CellTracker CellTracker => cellTracker;
    public ProjectileHitType ArmorHitType => armorHitType;

    public int CurrentHP { get; private set; }
    public int ArmorHP { get; private set; }
    public bool IsEatAnim { get; private set; }
    public bool IsDead { get; private set; }

    private bool isReturned;

    protected virtual void Awake()
    {
        Movement = GetComponent<ZombieMovement>();
        EffectController = GetComponent<ZombieEffectController>();
        AnimController = GetComponent<ZombieAnimationController>();
        SpineController = GetComponent<ZombieSpineController>();
        VisualHandler = GetComponent<ZombieVisualHandler>();
    }

    public void OnSpawn()
    {
        isReturned = false;
        IsDead = false;
        AnimController.PlayWalk();
        SpineController.Init();
    }

    public void Init(ZombieData data)
    {
        Data = data;
        CurrentHP = data.maxHP;
        ArmorHP = data.armorHP;
        IsEatAnim = data.isEatAnim;
        OnInit();
    }

    protected virtual void OnInit() { }

    public void SetupVisual(Cell cell) => VisualHandler.Init(cell);

    public virtual void TakeDamage(int amount, DamageSource source = DamageSource.Normal)
    {
        if (IsDead) return;

        VisualHandler.FlashHit();

        if (ArmorHP > 0)
        {
            int remaining = amount - ArmorHP;
            ArmorHP -= amount;
            if (ArmorHP <= 0)
            {
                ArmorHP = 0;
                OnArmorBroken();
                if (remaining > 0)
                    amount = remaining;
                else return;
            }
            else return;
        }

        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Die(source);
        }
    }

    protected virtual void OnArmorBroken() { }

    protected virtual void Die(DamageSource source)
    {
        if (IsDead) return;
        IsDead = true;
        onZombieDied?.Raise(this);
        VisualHandler.FreezeEffect.SetActive(false);

        if (source == DamageSource.Normal)
        {
            LostHead();
            VisualHandler.ResetGlowPreset();
            AnimController.PlayDie();
        }
        else if (source == DamageSource.Burn)
        {
            Ash();
            OnDieAnimFinished();
        }
    }

    protected virtual void LostHead() { }

    private void Ash()
    {
        var ash = PoolManager.Instance.GetZombie<ZombieAsh>(ashKey, ashPoint.position, Quaternion.identity);
        ash.Init(SortingOrderUtility.GetSortingOrder(LayerType.Zombie, CellTracker.Row));
    }

    public void OnAttackAnimFinished(PlantBase targetPlant)
    {
        if (targetPlant == null) return;
        targetPlant.TakeDamage(this, Data.attackDamage);
    }

    public void OnDieAnimFinished() => ReturnPool();

    public void OnDespawn()
    {
        Movement.ResetAll();
        EffectController.ResetAll();
        AnimController.ResetAll();
        VisualHandler.ResetAll();
    }

    private void ReturnPool()
    {
        if (isReturned) return;
        isReturned = true;

        PoolManager.Instance.ReleaseZombie(zombieKey, this);
    }
}
