using Spine;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(TargetingHelper))]
public class CherryBomb : PlantBase
{
    private SkeletonAnimation skeletonAnim;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable()
    {
        skeletonAnim.AnimationState.Event += OnSpineEvent;
    }

    private void OnDisable()
    {
        skeletonAnim.AnimationState.Event -= OnSpineEvent;
    }

    protected override void OnPlaced()
    {
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_ATTACK, false);
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name == AnimEvents.EVENT_ATTACK)
        {
            PlantActivator.Instance.Activate(PlantType, OccupiedCell, 10);
            return;
        }

        if (e.Data.Name == AnimEvents.EVENT_DIE)
        {
            Die();
            return;
        }
    }
}
