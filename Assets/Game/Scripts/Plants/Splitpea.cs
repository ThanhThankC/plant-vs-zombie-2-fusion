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
            peaProjectile.Init(Vector3.left);
        }
    }
}
