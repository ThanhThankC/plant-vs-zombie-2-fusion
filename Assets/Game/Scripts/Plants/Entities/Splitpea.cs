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
            var pea = PoolManager.Instance.Get<PeaProjectile>(projectileKey, backAtkPoint.position, Quaternion.identity);
            var effect = pea.HasEffect ? Data.CreateOnHitEffect() : null;
            pea.Init(Vector3.left, OccupiedCell, effect);
        }
    }
}
