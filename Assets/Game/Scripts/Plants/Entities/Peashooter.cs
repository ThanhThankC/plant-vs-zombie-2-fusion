using Spine;
using UnityEngine;

public class Peashooter : PlantBase
{
    [Header("Events")]
    [SerializeField] protected PlantAttackEvent onPlantAttack;
    [SerializeField] protected PlantAttackType attackType;

    [Header("Attack")]
    [SerializeField] protected PoolKey projectileKey;
    [SerializeField] private Transform attackPoint;

    [Header("Animations")]
    [SerializeField] private int loopCount = 1;

    private int currentLoop;
    private string pendingAnim = null;

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

    protected virtual void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
        {
            onPlantAttack.Raise(attackType);
            var pea = PoolManager.Instance.Get<PeaProjectile>(projectileKey, attackPoint.position, Quaternion.identity);
            var effect = pea.HasEffect ? Data.CreateOnHitEffect() : null;
            pea.Init(Vector3.right, OccupiedCell, effect);
        }
    }
}
