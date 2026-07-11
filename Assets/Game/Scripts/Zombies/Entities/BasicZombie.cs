using UnityEngine;

public class BasicZombie : ZombieBase
{
    [Header("Events")]
    [SerializeField] protected ZombiePartDroppedEvent onPartDropped;

    [Header("Part References")]
    [SerializeField] private ZombiePoolKey armKey;
    [SerializeField] private ZombiePoolKey headKey;
    [SerializeField] private Transform armSpawnPoint;
    [SerializeField] protected Transform headSpawnPoint;

    private bool armLost;

    private const string SKIN_ARM_FULL = "arm_full";
    private const string SKIN_ARM_TORN = "arm_torn";
    private const string SKIN_HEAD_PART = "head";

    protected override void OnInit()
    {
        armLost = false;
    }

    public override void TakeDamage(int amount, DamageSource source = DamageSource.Normal)
    {
        base.TakeDamage(amount, source);
        if (CurrentHP <= Data.maxHP / 2 && !armLost)
        {
            armLost = true;
            LostArm();
        }
    }

    private void LostArm()
    {
        SpineController.SetSkinActive(SKIN_ARM_FULL, false);
        SpineController.SetSkinActive(SKIN_ARM_TORN, true);

        onPartDropped?.Raise();

        var arm = PoolManager.Instance.GetZombie<BodyPart>(armKey, armSpawnPoint.position, Quaternion.identity);
        arm.Init(GetGroundY(), SortingOrderUtility.GetSortingOrder(LayerType.ZombieEffect, CellTracker.Row)); ;
    }

    protected override void LostHead()
    {
        SpineController.SetSkinActive(SKIN_HEAD_PART, false);

        onPartDropped?.Raise();

        var head = PoolManager.Instance.GetZombie<BodyPart>(headKey, headSpawnPoint.position, Quaternion.identity);
        head.Init(GetGroundY(), SortingOrderUtility.GetSortingOrder(LayerType.ZombieEffect, CellTracker.Row)); ;
    }

    protected float GetGroundY()
    {
        if (CellTracker.CurrentCell == null) return transform.position.y;
       return CellTracker.CurrentCell.SupportZone.transform.position.y;
    }
}