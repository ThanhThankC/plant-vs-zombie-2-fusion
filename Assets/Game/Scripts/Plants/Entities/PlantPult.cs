using Spine;
using System;
using UnityEngine;

public class PlantPult : PlantBase
{
    [System.Serializable]
    private struct ProjectileEntry
    {
        public PultProjectile projectilePrefab;
        [Range(0, 100)] public float rate;
        public string animationAttack;
    }

    [Header("Projectiles")]
    [SerializeField] private ProjectileEntry[] projectileEntries;
    [SerializeField] private Transform attackPoint;

    [Header("Animations")]
    [SerializeField] private int loopCount = 1;

    private PultProjectile currentProjectile;
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
        if (projectileEntries.Length > 0) 
            currentProjectile = projectileEntries[0].projectilePrefab;
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
        {
            string anim = projectileEntries.Length <= 1
                ? AnimEvents.ANIM_ATTACK 
                : PickAnimation(projectileEntries);

            QueueNextAnimation(anim);
        }
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
            SpawnProjectile();
    }

    private void SpawnProjectile()
    {
        if (projectileEntries.Length <= 0 || currentProjectile == null) return;

        var pultProjectile = Instantiate(currentProjectile, attackPoint.position, Quaternion.identity);
        var effect = pultProjectile.HasEffect ? Data.CreateOnHitEffect() : null;
        pultProjectile.Init(GetTargetPosition(), OccupiedCell, effect);
    }

    private string PickAnimation(ProjectileEntry[] projectileEntries)
    {
        float totalRate = 0;
        foreach (var p in projectileEntries)
            totalRate += p.rate;

        float roll = UnityEngine.Random.Range(0f, totalRate);
        float cumulative = 0;
        foreach (var p in projectileEntries)
        {
            cumulative += p.rate;
            if (roll < cumulative)
            {
                currentProjectile = p.projectilePrefab;
                return p.animationAttack;
            }
        }
        return AnimEvents.ANIM_ATTACK;
    }

    private Vector3 GetTargetPosition()
    {
        var target = ZombieManager.Instance.GetNearestZombieInRow(OccupiedCell.Row, transform.position.x);
        if (target != null)
            return target.transform.position;

        return new Vector3(GridManager.Instance.GetScreenPositionX(), OccupiedCell.transform.position.y, 0f);
    }
}
