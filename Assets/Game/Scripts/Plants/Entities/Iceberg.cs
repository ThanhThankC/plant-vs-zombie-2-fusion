using Spine;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(TargetingHelper))]
[RequireComponent(typeof(BoxCollider2D))]
public class Iceberg : PlantBase
{
    private TargetingHelper targetingHelper;
    private bool isAttacking;

    protected override void Awake()
    {
        base.Awake();
        targetingHelper = GetComponent<TargetingHelper>();
    }

    private void OnEnable()
    {
        skeletonAnim.AnimationState.Event += OnSpineEvent;
    }

    private void OnDisable()
    {
        skeletonAnim.AnimationState.Event -= OnSpineEvent;
    }

    protected override void OnPlaced()
    {
        isAttacking = false;
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_IDLE, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsGhost || isAttacking) return;
        if (!other.CompareTag("Zombie")) return;

        var zombie = other.GetComponent<ZombieBase>();
        if (zombie == null || zombie.CellTracker.Row != OccupiedCell.Row) return;

        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_ATTACK, false);
        isAttacking = true;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (!isAttacking) return;

        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
        {
            FreezeFirstZombie();
            return;
        }

        if (e.Data.Name == AnimEvents.EVENT_DIE)
        {
            Die();
            return;
        }
    }

    private void FreezeFirstZombie()
    {
        foreach (var hit in targetingHelper.GetTargetsInBox(transform.position))
        {
            if (!hit.CompareTag("Zombie")) continue;

            var zombie = hit.GetComponent<ZombieBase>();
            if (zombie == null || zombie.CellTracker.Row != OccupiedCell.Row) continue;

            zombie.EffectController.ApplyEffect(Data.CreateOnHitEffect());
            return;
        }
    }
}
