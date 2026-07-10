using Spine;
using Spine.Unity;
using UnityEngine;

public class SplatProjectile : MonoBehaviour, IPoolable
{
    [SerializeField] private PoolKey splatKey;

    private SkeletonAnimation skeletonAnim;
    private MeshRenderer meshRenderer;
    private bool isReturned;

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

    public void OnSpawn() => isReturned = false;

    public void Init(int sortingOrder)
    {
        meshRenderer.sortingOrder = sortingOrder;
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_ANIMATION, false);
    }

    public void OnDespawn() { }

    private void OnSpineComplete(TrackEntry trackEntry)
    {
        if (isReturned) return;
        isReturned = true;

        PoolManager.Instance.Release(splatKey, this);
    }
}
