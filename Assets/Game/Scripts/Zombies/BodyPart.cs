using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BodyPart : MonoBehaviour, IPoolable
{
    [Header("Pool Settings")]
    [SerializeField] private ZombiePoolKey partKey;

    [Header("Visuals")]
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private float fadeDuration = 1.4f;

    [Header("Jump Animation")]
    [SerializeField] private float jumpPower1 = 0.7f;
    [SerializeField] private float jumpDuration1 = 0.73f;
    [SerializeField] private float rotationAmount1 = 160f;
    [SerializeField] private float jumpPower2 = 0.3f;
    [SerializeField] private float jumpDuration2 = 0.46f;
    [SerializeField] private float rotationAmount2 = 190f;

    [Header("Movement")]
    [SerializeField] private Vector2 bounceDistance = new Vector2(1f, 2.5f);
    [SerializeField] private Vector2 rollDistance = new Vector2(0.2f, 0.5f);
    [SerializeField] private Vector2 directionRange = new Vector2(-0.8f, 1f);

    private SpriteRenderer spriteRenderer;
    private Sequence animSequence;
    private Color originalColor;
    private bool isReturned;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    public void Init(float groundPosY, int sortingOrder, int state = 0)
    {
        if (sprites != null && state < sprites.Length)
            spriteRenderer.sprite = sprites[state];

        spriteRenderer.sortingOrder = sortingOrder;
        ApplyBounce(groundPosY);
    }

    private void ApplyBounce(float groundPosY)
    {
        float direction = Random.value < 0.5f ? directionRange.x : directionRange.y;

        var endPos1 = new Vector3(
            transform.position.x + Random.Range(bounceDistance.x, bounceDistance.y) * direction,
            groundPosY, 0f
        );
        var endPos2 = new Vector3(
            endPos1.x + Random.Range(rollDistance.x, rollDistance.y) * direction,
            groundPosY, 0f
        );

        animSequence?.Kill();
        animSequence = DOTween.Sequence();

        animSequence.Append(
            transform.DOJump(endPos1, jumpPower1, 1, jumpDuration1)
                .SetEase(Ease.Linear)
        );
        animSequence.Join(
            transform.DORotate(new Vector3(0f, 0f, rotationAmount1 * direction), jumpDuration1)
                .SetEase(Ease.OutQuad)
        );
        animSequence.Append(
            transform.DOJump(endPos2, jumpPower2, 1, jumpDuration2)
                .SetEase(Ease.Linear)
        );
        animSequence.Join(
            transform.DORotate(new Vector3(0f, 0f, rotationAmount2 * direction), jumpDuration2)
                .SetEase(Ease.OutQuad)
        );
        animSequence.OnComplete(FadeOut);
    }

    private void FadeOut()
    {
        var color = originalColor;
        DOTween.To(
            () => color.a,
            x => spriteRenderer.color = new Color(color.r, color.g, color.b, x),
            0f,
            fadeDuration
        ).OnComplete(() => ReturnPool());
    }

    public void OnSpawn()
    {
        spriteRenderer.color = originalColor;
        animSequence?.Kill();
        isReturned = false;
    }

    public void OnDespawn()
    {
        animSequence?.Kill();
    }

    private void ReturnPool()
    {
        if (isReturned) return;
        isReturned = true;

        PoolManager.Instance.ReleaseZombie(partKey, this);
    }
}