using CodiceApp.EventTracking.Plastic;
using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicZombie : ZombieBase
{
    private const string ANIM_IDLE = "idle";
    private const string ANIM_WALK = "walk";
    private const string ANIM_EAT = "eat";
    private const string ANIM_DIE = "die";

    private const string EVENT_ATTACK = "on_attack";

    private const string SKIN_HEAD = "head";
    private const string SKIN_ARM = "arm_full";

    private SkeletonAnimation skeletonAnim;

    protected override void Awake()
    {
        base.Awake();
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable()
    {
        skeletonAnim.AnimationState.Event += OnSpineEvent;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == EVENT_ATTACK)
            OnAttackFrame();
    }

    private void OnAttackFrame()
    {
        Debug.Log("Attack");
    }
}
