using Spine;
using Spine.Unity;
using UnityEngine;

public class Peashooter : PlantBase
{
    [SerializeField] private PeaProjectile projectilePrefab;
    [SerializeField] private Transform attackTransform;

    private SkeletonAnimation skeletonAnim;
    private string pendingAnim = null;
    private bool wasAttacking = false;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable()
    {
        skeletonAnim.AnimationState.Event += OnSpineEvent;
        skeletonAnim.AnimationState.Complete += OnPlayAnimation;
    }

    private void OnDisable()
    {
        skeletonAnim.AnimationState.Event -= OnSpineEvent;
        skeletonAnim.AnimationState.Complete -= OnPlayAnimation;
    }

    private void Update()
    {
        bool hasZombie = ZombieManager.Instance.HasZombieInRow(OccupiedCell.Row, transform.position.x);
        if (hasZombie && !wasAttacking)
        {
            wasAttacking = true;
            QueueNextAnimation(AnimEvents.ANIM_ATTACK);
        }
        else if (!hasZombie && wasAttacking)
        {
            wasAttacking = false;
            QueueNextAnimation(AnimEvents.ANIM_IDLE);
        }
    }

    private void QueueNextAnimation(string animName)
    {
        var current = skeletonAnim.AnimationState.GetCurrent(0);
        if (current != null && current.Animation.Name == animName)
        {
            pendingAnim = null;
            return;
        }

        pendingAnim = animName;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
            Instantiate(projectilePrefab, attackTransform.position, Quaternion.identity);
    }

    private void OnPlayAnimation(TrackEntry trackEntry)
    {
        if (pendingAnim == null) return;
        skeletonAnim.AnimationState.SetAnimation(0, pendingAnim, true);
        pendingAnim = null;
    }
}
