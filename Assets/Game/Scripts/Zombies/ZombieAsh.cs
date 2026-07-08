using Spine;
using Spine.Unity;
using UnityEngine;

public class ZombieAsh : MonoBehaviour
{
    private SkeletonAnimation skeletonAnim;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable()
    {
        skeletonAnim.AnimationState.Complete += OnSpineComplete;
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_IDLE, false);
    }

    private void OnDisable()
    {
        skeletonAnim.AnimationState.Complete -= OnSpineComplete;

    }

    private void OnSpineComplete(TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}
