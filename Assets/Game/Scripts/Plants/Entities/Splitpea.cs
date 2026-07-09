using Spine;
using UnityEngine;

public class Splitpea : Peashooter
{
    [SerializeField] private Transform backAtkPoint;

    protected override void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        base.OnSpineEvent(trackEntry, e);

        if (e.Data.Name == AnimEvents.EVENT_BACK_ATTACK)
        {
            PeaProjectile peaProjectile = Instantiate(projectilePrefab, backAtkPoint.position, Quaternion.identity);
            var effect = peaProjectile.HasEffect ? Data.CreateOnHitEffect() : null;
            peaProjectile.Init(Vector3.left, OccupiedCell, effect);
        }
    }
}
