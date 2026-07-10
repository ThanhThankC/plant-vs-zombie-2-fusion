using Spine;
using Spine.Unity;

public class Jalapeno : PlantBase
{
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
            PlantActivator.Instance.Activate(PlantType, OccupiedCell);
            return;
        }

        if (e.Data.Name == AnimEvents.EVENT_DIE)
        {
            Die();
            return;
        }
    }
}