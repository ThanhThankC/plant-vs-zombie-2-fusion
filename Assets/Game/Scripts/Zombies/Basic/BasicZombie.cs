using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombie : ZombieBase
{
    private SkeletonAnimation skeletonAnim;

    protected override void Awake()
    {
        base.Awake();
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable() => skeletonAnim.AnimationState.Event += OnSpineEvent;

    private void OnDisable() => skeletonAnim.AnimationState.Event -= OnSpineEvent;

    private void Update()
    {
        if (IsDead) return;

        var target = GetPlantTarget();
        var currentAnim = skeletonAnim.AnimationState.GetCurrent(0)?.Animation?.Name;

        if (target != null && currentAnim != AnimEvents.ANIM_EAT)
            PlayEat();
        else if (target == null && currentAnim != AnimEvents.ANIM_WALK)
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
            Movement.AllowMove();
        else
            Movement.DisallowMove();
    }

    public void PlayWalk() => SetAnimation(AnimEvents.ANIM_WALK, true);
    public void PlayEat() => SetAnimation(AnimEvents.ANIM_EAT, true);
    public void PlayDie() => SetAnimation(AnimEvents.ANIM_DIE, true);

    private void OnAttackFrame()
    {
        var target = GetPlantTarget();
        target?.TakeDamage(10);
    }

    private PlantBase GetPlantTarget()
    {
        var cell = CellTracker.GetCurrentCell();
        if (cell == null) return null;
        return cell.GetPlantInstance(FieldType.Support) ?? cell.GetPlantInstance(FieldType.Normal);
    }

    protected override void OnDie(DamageSource source)
    {
        PlayDie();
        base.OnDie(source);
    }
}
