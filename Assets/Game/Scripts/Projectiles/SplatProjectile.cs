using Spine;
using Spine.Unity;
using UnityEngine;

public class SplatProjectile : MonoBehaviour
{
    private SkeletonAnimation skeletonAnim;
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
        meshRenderer = skeletonAnim.GetComponent<MeshRenderer>();
    }

    private void OnEnable()
    {
        skeletonAnim.AnimationState.Complete += OnSpineComplete;
    }

    private void OnDisable()
    {
        skeletonAnim.AnimationState.Complete -= OnSpineComplete;
    }

    public void Init(int sortingOrder)
    {
        meshRenderer.sortingOrder = sortingOrder;
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_ANIMATION, false);
    }

    private void OnSpineComplete(TrackEntry trackEntry)
    {
        Destroy(gameObject);
    }
}
