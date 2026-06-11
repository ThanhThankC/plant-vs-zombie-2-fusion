using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Peashooter : PlantBase
{
    [SerializeField] private PeaProjectile projectilePrefab;
    [SerializeField] private Transform attackTransform;

    private SkeletonAnimation skeletonAnim;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable() => skeletonAnim.AnimationState.Event += OnSpineEvent;

    private void OnDisable() => skeletonAnim.AnimationState.Event -= OnSpineEvent;

    private void Update()
    {
        if (skeletonAnim == null) return;
        bool hasZombie = ZombieManager.Instance.HasZombieInRow(OccupiedCell.Row, transform.position.x);
        var currentAnim = skeletonAnim.AnimationState.GetCurrent(0).Animation.Name;
        if (hasZombie && currentAnim != AnimEvents.ANIM_ATTACK)
            PlayAttack();
        else if (!hasZombie && currentAnim != AnimEvents.ANIM_IDLE)
            PlayIdle();
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
            Instantiate(projectilePrefab, attackTransform.position, Quaternion.identity);
    }

    private void PlayAttack()
    {
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_ATTACK, true);
    }

    private void PlayIdle()
    {
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_IDLE, true);
    }
}
