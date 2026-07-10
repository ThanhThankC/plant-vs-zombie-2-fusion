using Spine;
using Spine.Unity;
using UnityEngine;

public class PlantEffect : MonoBehaviour, IPoolable
{
    [SerializeField] private PoolKey effectKey;

    private SkeletonAnimation skeletonAnim;
    private MeshRenderer meshRenderer;
    private bool isReturned;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
        meshRenderer = skeletonAnim.GetComponent<MeshRenderer>();
    }

    private void OnEnable() => skeletonAnim.AnimationState.Complete += OnSpineComplete;

    private void OnDisable() => skeletonAnim.AnimationState.Complete -= OnSpineComplete;

    public void OnSpawn() => isReturned = false;

    public void Init(int sortingOrder)
    {
        meshRenderer.sortingOrder = sortingOrder;
        skeletonAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_IDLE, false);
    }

    private void OnSpineComplete(TrackEntry trackEntry) => ReturnPool();

    public void OnDespawn() { }

    private void ReturnPool()
    {
        if (isReturned) return;
        isReturned = true;

        PoolManager.Instance.Release(effectKey, this);
    }
}
