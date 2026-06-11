using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimationController : MonoBehaviour
{
    private SkeletonAnimation skeletonAnim;
    private ZombieMovement movement;
    private CellTracker cellTracker;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
        movement = GetComponent<ZombieMovement>();
        cellTracker = GetComponentInChildren<CellTracker>();
    }

    private void OnEnable() => skeletonAnim.AnimationState.Event += OnSpineEvent;

    private void OnDisable() => skeletonAnim.AnimationState.Event -= OnSpineEvent;

    public void ChangeToEatAndWalk()
    {
        var currentAnim = skeletonAnim.AnimationState.GetCurrent(0).Animation.Name;
        if (cellTracker.TargetPlant != null && currentAnim != AnimEvents.ANIM_EAT)
            PlayEat();
        else if (cellTracker.TargetPlant == null && currentAnim != AnimEvents.ANIM_WALK)
            PlayWalk();
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
            OnAttackFrame();
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
    public void PlayEat() => SetAnimation(AnimEvents.ANIM_EAT, true);
    public void PlayDie() => SetAnimation(AnimEvents.ANIM_DIE, false);

    private void OnAttackFrame()
    {
        cellTracker.TargetPlant?.TakeDamage(10);
    }
}
