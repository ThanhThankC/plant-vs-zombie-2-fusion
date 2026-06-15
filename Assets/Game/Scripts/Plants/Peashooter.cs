using Spine;
using Spine.Unity;
using UnityEngine;

public class Peashooter : PlantBase
{
    [SerializeField] private PeaProjectile projectilePrefab;
    [SerializeField] private Transform attackTransform;
    [SerializeField] private int loopCount = 1;

    private SkeletonAnimation skeletonAnim;
    private int currentLoop;
    private string pendingAnim = null;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

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
        currentLoop = loopCount;
        PlayIdle();
    }

    private void OnSpineComplete(TrackEntry trackEntry)
    {
        bool hasZombie = ZombieManager.Instance.HasZombieInRow(OccupiedCell.Row, transform.position.x);
        if (hasZombie)
            PlayAttack();
        else
            PlayIdle();
    }

    private void PlayIdle()
    {
        QueueNextAnimation(AnimEvents.ANIM_IDLE);
        PlayAnimation();
    }

    private void PlayAttack()
    {
        if (currentLoop == loopCount)
            QueueNextAnimation(AnimEvents.ANIM_ATTACK);
        else
            QueueNextAnimation(AnimEvents.ANIM_IDLE);

        if (currentLoop > 0)
            currentLoop--;
        else
            currentLoop = loopCount;

        PlayAnimation();
    }

    private void QueueNextAnimation(string animName)
    {
        var current = skeletonAnim.AnimationState.GetCurrent(0);
        if (current == null) return;

        if (current.Animation.Name == animName)
        {
            pendingAnim = null;
            return;
        }

        pendingAnim = animName;
    }

    private void PlayAnimation()
    {
        if (pendingAnim == null) return;
        skeletonAnim.AnimationState.SetAnimation(0, pendingAnim, true);
        pendingAnim = null;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
            Instantiate(projectilePrefab, attackTransform.position, Quaternion.identity);
    }
}
