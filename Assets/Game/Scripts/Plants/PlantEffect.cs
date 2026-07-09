using Spine;
using Spine.Unity;
using UnityEngine;

public class PlantEffect : MonoBehaviour
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
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_IDLE, false);
    }

    private void OnDisable()
    {
        skeletonAnim.AnimationState.Complete -= OnSpineComplete;
    }

    public void Init(int sortingOrder)
    {
        meshRenderer.sortingOrder = sortingOrder;
    }

    private void OnSpineComplete(TrackEntry trackEntry)
    {
        Destroy(this);
    }
}
