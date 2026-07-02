using DG.Tweening;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BodyPart : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private float fadeDuration = 1.4f;
    [SerializeField] private float jumpPower1 = 0.7f;
    [SerializeField] private float jumpPower2 = 0.3f;
    [SerializeField] private float jumpDuration1 = 0.73f;
    [SerializeField] private float jumpDuration2 = 0.46f;
    [SerializeField] private float rotationAmount1 = 160f;
    [SerializeField] private float rotationAmount2 = 190f;
    [SerializeField] private Vector2 bounceDistance = new Vector2(1f, 2.5f);   // x: min, y: max
    [SerializeField] private Vector2 rollDistance = new Vector2(0.2f, 0.5f); // x: min, y: max
    [SerializeField] private Vector2 directionRange = new Vector2(-0.8f, 1f); // x: left, y: right

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Init(float groundPosY, int sortingOrder)
    {
        Init(groundPosY, sortingOrder, 0);
    }

    public void Init(float groundPosY, int sortingOrder, int state)
    {
        if (sprites != null && state < sprites.Length)
            spriteRenderer.sprite = sprites[state];

        spriteRenderer.sortingOrder = sortingOrder;

        ApplyBounce(groundPosY);
    }

    private void ApplyBounce(float groundPosY)
    {
        float direction = Random.Range(0, 2) == 0 ? directionRange.x : directionRange.y;

        Vector3 endPos1 = new Vector3(
            transform.position.x + Random.Range(bounceDistance.x, bounceDistance.y) * direction,
            groundPosY, 0f
        );

        Vector3 endPos2 = new Vector3(
            endPos1.x + Random.Range(rollDistance.x, rollDistance.y) * direction,
            groundPosY, 0f
        );


        Sequence seq = DOTween.Sequence();

        seq.Append(transform.DOJump(endPos1, jumpPower1, 1, jumpDuration1).SetEase(Ease.Linear));
        seq.Join(transform.DORotate(new Vector3(0f, 0f, rotationAmount1 * direction), jumpDuration1).SetEase(Ease.OutQuad));
        seq.Append(transform.DOJump(endPos2, jumpPower2, 1, jumpDuration2).SetEase(Ease.Linear));
        seq.Join(transform.DORotate(new Vector3(0f, 0f, rotationAmount2 * direction), jumpDuration2).SetEase(Ease.OutQuad));

        seq.OnComplete(() => FadeOut());
    }

    private void FadeOut()
    {
        Color c = spriteRenderer.color;
        DOTween.To(
            () => c.a,
            x => spriteRenderer.color = new Color(c.r, c.g, c.b, x),
            0f,
            fadeDuration
        ).OnComplete(() => Destroy(gameObject));
    }
}
