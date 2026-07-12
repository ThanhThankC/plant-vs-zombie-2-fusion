using Spine;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Spikeweed : PlantBase
{
    [Header("Events")]
    [SerializeField] private PlantAttackEvent onPlantAttack;
    [SerializeField] private PlantAttackType attackType;

    private List<ZombieBase> targets = new();
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

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (IsGhost) return;
        if (!collider.CompareTag("Zombie")) return;

        var zombie = collider.GetComponent<ZombieBase>();
        if (zombie == null || zombie.CellTracker.Row != OccupiedCell.Row) return;
        if (!targets.Contains(zombie)) targets.Add(zombie);

        if (isAttacking) return;
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_ATTACK, true);
        isAttacking = true;
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (IsGhost) return;
        if (!collider.CompareTag("Zombie")) return;

        var zombie = collider.GetComponent<ZombieBase>();
        if (zombie == null || zombie.CellTracker.Row != OccupiedCell.Row) return;
        targets.Remove(zombie);

        if (targets.Count > 0) return;
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_IDLE, true);
        isAttacking = false;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (IsGhost) return;
        if (!isAttacking) return;

        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
        {
            onPlantAttack.Raise(attackType);
            AttackAllZombie();
            return;
        }
    }

    private void AttackAllZombie()
    {
        foreach (var zombie in targets)
        {
            if (zombie == null || zombie.CellTracker.Row != OccupiedCell.Row) continue;
            zombie.TakeDamage(Data.aoeDamage, Data.damageSource);
        }
    }
}
