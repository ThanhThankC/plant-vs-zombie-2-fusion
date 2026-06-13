using Spine;
using Spine.Unity;
using UnityEngine;

public class Peashooter : PlantBase
{
    [SerializeField] private PeaProjectile projectilePrefab;
    [SerializeField] private Transform attackTransform;

    private SkeletonAnimation skeletonAnim;
    private bool wasAttacking = false;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable() => skeletonAnim.AnimationState.Event += OnSpineEvent;

    private void OnDisable() => skeletonAnim.AnimationState.Event -= OnSpineEvent;

    private void Update()
    {
        bool hasZombie = ZombieManager.Instance.HasZombieInRow(OccupiedCell.Row, transform.position.x);
        if (hasZombie && !wasAttacking)
        {
            wasAttacking = true;
            PlayAttack();
        }
        else if (!hasZombie && wasAttacking)
        {
            wasAttacking = false;
            PlayIdle();
        }
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
            Instantiate(projectilePrefab, attackTransform.position, Quaternion.identity);
    }

    private void PlayAttack()
    {
        var current = skeletonAnim.AnimationState.GetCurrent(0);
        if (current != null && current.Animation.Name == AnimEvents.ANIM_ATTACK) return;

        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_ATTACK, false); 
        skeletonAnim.AnimationState.AddAnimation(0, AnimEvents.ANIM_ATTACK, true, 0);
    }

    private void PlayIdle()
    {
        var current = skeletonAnim.AnimationState.GetCurrent(0);
        if (current != null && current.Animation.Name == AnimEvents.ANIM_IDLE) return;

        skeletonAnim.AnimationState.AddAnimation(0, AnimEvents.ANIM_IDLE, true, 0);
    }
}
