using UnityEngine;

public class BasicZombie : ZombieBase
{
    [SerializeField] private BodyPart armPartPrefab;
    [SerializeField] private BodyPart headPartPrefab;
    [SerializeField] private Transform armSpawnPoint;
    [SerializeField] protected Transform headSpawnPoint;
    [SerializeField] private ZombieAsh zombieAsh;
    [SerializeField] private Transform ashPoint;

    private bool armLost;

    private const string SKIN_ARM_FULL = "arm_full";
    private const string SKIN_ARM_TORN = "arm_torn";
    private const string SKIN_HEAD_PART = "head";

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

        var armObj = Instantiate(armPartPrefab, armSpawnPoint.position, Quaternion.identity);
        armObj.Init(GetGroundY(), 10);
    }

    protected override void LostHead()
    {
        SpineController.SetSkinActive(SKIN_HEAD_PART, false);

        var headObj = Instantiate(headPartPrefab, headSpawnPoint.position, Quaternion.identity);
        headObj.Init(GetGroundY(), 11);
    }

    protected override void Ash()
    {
        Instantiate(zombieAsh, ashPoint.position, Quaternion.identity);
    }

    protected float GetGroundY()
    {
        if (CellTracker.CurrentCell == null) return transform.position.y;
       return CellTracker.CurrentCell.SupportZone.transform.position.y;
    }
}