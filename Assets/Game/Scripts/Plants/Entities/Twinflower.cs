using Spine;
using UnityEngine;

public class Twinflower : PlantBase
{
    [SerializeField] private PoolKey sunKey;
    [SerializeField] private Transform specialTransform1;
    [SerializeField] private Transform specialTransform2;
    [SerializeField] private int loopCount = 3;
    [SerializeField] private Vector2 landingOffsetXRange = new Vector2(-1f, 1f);
    [SerializeField] private Vector2 landingOffsetYRange = new Vector2(-1f, -0.5f);

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
        PlaySpecial();
    }

    private void PlayIdle()
    {
        QueueNextAnimation(AnimEvents.ANIM_IDLE);
        PlayAnimation();
    }

    private void PlaySpecial()
    {
        if (currentLoop <= 0)
        {
            QueueNextAnimation(AnimEvents.ANIM_SPECIAL);
            currentLoop = loopCount;
        }
        else
        {
            QueueNextAnimation(AnimEvents.ANIM_IDLE);
            currentLoop--;
        }
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
        if (IsGhost) return;
        if (e.Data.Name == AnimEvents.EVENT_SPECIAL)
        {
            var sun1 = PoolManager.Instance.Get<Sun>(sunKey, specialTransform1.position, Quaternion.identity);
            sun1.InitCurved(GetGroundPosition());

            var sun2 = PoolManager.Instance.Get<Sun>(sunKey, specialTransform2.position, Quaternion.identity);
            sun2.InitCurved(GetGroundPosition());
        }
    }

    private Vector3 GetGroundPosition()
    {
        Vector3 groundPosY = OccupiedCell.transform.position;
        float offsetX = Random.Range(landingOffsetXRange.x, landingOffsetXRange.y);
        float offsetY = Random.Range(landingOffsetYRange.x, landingOffsetYRange.y);
        return groundPosY + new Vector3(offsetX, offsetY, 0f);
    }
}