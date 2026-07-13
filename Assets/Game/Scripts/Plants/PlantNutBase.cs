using Spine;
using Spine.Unity;
using UnityEngine;


public class PlantNutBase : PlantBase
{
    [System.Serializable]
    private struct DamageStage
    {
        [Range(0, 1)]
        public float hpThreshold;
        public string animName;
    }

    [Header("DamageState (Descending Order)")]
    [SerializeField] private DamageStage[] damageStages;

    [Header("Idle Animations (Alternate)")]
    [SerializeField] private string[] idleAnims;

    private int currentStageIndex;
    private int currentIdleIndex;

    private void OnEnable()
    {
        skeletonAnim.AnimationState.Complete += OnSpineComplete;
    }

    private void OnDisable()
    {
        skeletonAnim.AnimationState.Complete -= OnSpineComplete;
    }

    protected override void OnPlaced()
    {
        currentStageIndex = 0;
        currentIdleIndex = 0;
        if (idleAnims == null || idleAnims.Length <= 0) return;

        PlayAnim(idleAnims[currentIdleIndex], true);
    }

    public override void TakeDamage(ZombieBase zombie, int amount)
    {
        base.TakeDamage(zombie, amount);

        if (damageStages == null || damageStages.Length <= 0) return;
        if (currentStageIndex >= damageStages.Length) return;

        float hpRatio = (float) CurrentHP / Data.maxHP; 
        if (hpRatio >= damageStages[currentStageIndex].hpThreshold) return;

        PlayAnim(damageStages[currentStageIndex].animName, true);
        currentStageIndex++;
    }

    private void OnSpineComplete(TrackEntry trackEntry)
    {
        if (idleAnims.Length <= 1 || currentStageIndex > 0) return;
        currentIdleIndex++;
        int realIldeIndex = currentIdleIndex % idleAnims.Length;
        PlayAnim(idleAnims[realIldeIndex], false);
    }

    private void PlayAnim(string anim, bool loop)
    {
        skeletonAnim.AnimationState.SetAnimation(0, anim, loop);
    }
}
