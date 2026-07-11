using DG.Tweening;
using UnityEngine;

public class Sun : MonoBehaviour, IPoolable
{
    private enum SunState
    {
        None,
        StraightFall,
        Collecting,
        CurvedFall,
    }

    [Header("Events")]
    [SerializeField] private SunCollectedEvent onSunCollected;

    [Header("Pools")]
    [SerializeField] private PoolKey sunKey;

    [Header("References")]
    [SerializeField] private int cost = 25;
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float collectSpeed = 10f;
    [SerializeField] private float collectionDuration = 3f;
    [SerializeField] private float jumpPower = 2f;
    [SerializeField] private float jumpDuration = 1.2f;
    [SerializeField] private float fadeDuration = 0.3f;

    public int Cost => cost;

    private SunState sunState = SunState.None;
    private Tween jumpTween;
    private TweenAnimator tweenAnim;
    private Vector3 groundPos;
    private Vector3 uiPos;
    private bool isReturned;

    private void Awake()
    {
        tweenAnim = GetComponent<TweenAnimator>();
    }

    public void InitStraight(float groundPosY, Vector3 uiTargetPos)
    {
        groundPos = new Vector3(transform.position.x, groundPosY, 0f);
        sunState = SunState.StraightFall;
        uiPos = uiTargetPos;
    }

    public void InitCurved(Vector3 groundPosY)
    {
        this.groundPos = groundPosY;
        sunState = SunState.CurvedFall;
        onSunCollected?.Raise(cost);
    }

    private void Update()
    {
        if (isReturned) return;
        switch (sunState)
        {
            case SunState.StraightFall:
                FallingFollowStraight();
                break;
            case SunState.Collecting:
                Collecting();
                break;
            case SunState.CurvedFall:
                FallingFollowCurved();
                break;
        }
    }

    public void OnTap()
    {
        if (sunState == SunState.Collecting) return;

        if (sunState == SunState.None)
            jumpTween?.Kill();
        sunState = SunState.Collecting;
    }

    private void FallingFollowStraight()
    {
        transform.position = Vector3.MoveTowards(transform.position, groundPos, fallSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, groundPos) <= 0.01f)
        {
            sunState = SunState.None;
            Invoke(nameof(OnTap), collectionDuration);
        }
    }

    private void Collecting()
    {
        transform.position = Vector3.Lerp(transform.position, uiPos, collectSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, uiPos) <= 0.2f)
        {
            SunManager.Instance.CollectSun(cost);
            tweenAnim.FadeOutObject(fadeDuration, () => ReturnPool());
            sunState = SunState.None;
        }
    }

    private void FallingFollowCurved()
    {
        sunState = SunState.None;
        jumpTween = transform.DOJump(groundPos, jumpPower, 1, jumpDuration)
            .SetEase(Ease.Linear)
            .OnComplete(() => Invoke(nameof(OnTap), collectionDuration));
    }

    public void OnSpawn()
    {
        isReturned = false;
        sunState = SunState.None;
        jumpTween?.Kill();
        CancelInvoke();
    }

    public void OnDespawn()
    {
        tweenAnim.ResetAlpha();
        jumpTween?.Kill();
        CancelInvoke();
    }

    private void ReturnPool()
    {
        if (isReturned) return;
        isReturned = true;

        PoolManager.Instance.Release(sunKey, this);
    }
}
