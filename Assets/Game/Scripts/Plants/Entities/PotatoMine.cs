using Spine;
using UnityEngine;

[RequireComponent(typeof(TargetingHelper))]
[RequireComponent(typeof(BoxCollider2D))]
public class PotatoMine : PlantBase
{
    [SerializeField] private int waitingTotal = 500;

    private int waitingIndex;
    private bool isRecovered;
    private bool isAttacking;

    private void OnEnable()
    {
        skeletonAnim.AnimationState.Event += OnSpineEvent;
        skeletonAnim.AnimationState.Complete += OnSpineComplete;
    }

    private void OnDisable()
    {
        skeletonAnim.AnimationState.Event -= OnSpineEvent;
        skeletonAnim.AnimationState.Complete -= OnSpineComplete;
    }

    protected override void OnPlaced()
    {
        waitingIndex = 0;
        isAttacking = false;
        isRecovered = false;
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_PLANT_IDLE, true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (IsGhost || !isRecovered || isAttacking) return;
        if (!other.CompareTag("Zombie")) return;

        var zombie = other.GetComponent<ZombieBase>();
        if (zombie == null || zombie.CellTracker.Row != OccupiedCell.Row) return;

        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_ATTACK, false);
        isAttacking = true;
    }

    private void OnSpineComplete(TrackEntry trackEntry)
    {
        if (trackEntry.Animation.Name == AnimEvents.ANIM_RECOVER)
        {
            skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_IDLE, true);
            isRecovered = true;
        }

        if (trackEntry.Animation.Name == AnimEvents.ANIM_PLANT_IDLE)
        {
            waitingIndex++;
            if (waitingIndex < waitingTotal || isRecovered) return;
            
            skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_RECOVER, false);
        }
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (!isAttacking) return;

        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
        {
            PlantActivator.Instance.Activate(PlantType, OccupiedCell);
            return;
        }

        if (e.Data.Name == AnimEvents.EVENT_DIE)
        {
            Die();
            return;
        }
    }
}
