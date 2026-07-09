using Spine;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class IceStorm : PlantBase
{
    private bool isAttacking;

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
            PlantActivator.Instance.Activate(PlantType, OccupiedCell, 10);
            return;
        }

        if (e.Data.Name == AnimEvents.EVENT_DIE)
        {
            Die();
            return;
        }
    }
}
