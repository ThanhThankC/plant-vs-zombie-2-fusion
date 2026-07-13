using Spine;
using Spine.Unity;
using UnityEngine;

public class ZombieAnimationController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnim;
    private ZombieBase zombie;
    private ZombieMovement movement;
    private ZombieEffectController effect;
    private CellTracker cellTracker;
    private enum WantToStates { Walk, Attack, But}
    private WantToStates state = WantToStates.Walk;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
        zombie = GetComponent<ZombieBase>();
        movement = GetComponent<ZombieMovement>();
        effect = GetComponent<ZombieEffectController>();
        cellTracker = GetComponentInChildren<CellTracker>();
    }

    private void OnEnable() => skeletonAnim.AnimationState.Event += OnSpineEvent;

    private void OnDisable() => skeletonAnim.AnimationState.Event -= OnSpineEvent;

    public void RefreshState()
    {
        if (zombie.IsDead) return;
        var currentAnim = skeletonAnim.AnimationState.GetCurrent(0).Animation.Name;
        if (cellTracker.TargetPlant != null)
            state = WantToStates.Attack;
        else
            state = WantToStates.Walk;
        ApplyPendingState();
    }

    public void ApplyPendingState()
    {
        if (zombie.IsDead || effect.IsStun) return;
        var currentAnim = skeletonAnim.AnimationState.GetCurrent(0).Animation.Name;
        switch (state)
        {
            case WantToStates.Walk:
                if (currentAnim != AnimEvents.ANIM_WALK)
                    PlayWalk();
                break;
            case WantToStates.Attack:
                if (currentAnim != AnimEvents.ANIM_EAT)
                    PlayAttack();
                break;
        }
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
            zombie.OnAttackAnimFinished(cellTracker.TargetPlant);

        if (e.Data.Name == AnimEvents.EVENT_DIE)
            zombie.OnDieAnimFinished();
    }

    private void SetAnimation(string animName, bool loop)
    {
        skeletonAnim.AnimationState.SetAnimation(0, animName, loop);
        if (animName == AnimEvents.ANIM_WALK)
            movement.AllowMove();
        else
            movement.DisallowMove();
    }

    public void PlayWalk() => SetAnimation(AnimEvents.ANIM_WALK, true);
    private void PlayAttack() => SetAnimation(AnimEvents.ANIM_EAT, true);
    public void PlayDie() => SetAnimation(AnimEvents.ANIM_DIE, false);
    public void PlayIdle() => SetAnimation(AnimEvents.ANIM_IDLE, true);

    public void ResetAll()
    {
        state = WantToStates.Walk;
    }
}
