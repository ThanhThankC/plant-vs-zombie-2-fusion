using System.Collections;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;

public class WinCupController : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private SkeletonGraphic spineAnim;
    [SerializeField] private float exitDelay = 1.5f;

    [Header("Spawn")]
    [SerializeField] private float spawnOffset = 300f;
    [SerializeField] private float flyInDuration = 0.5f;

    [Header("Click")]
    [SerializeField] private float shrinkDuration = 0.15f;
    [SerializeField] private float growDuration = 0.35f;
    [SerializeField] private float growScale = 1.4f;

    private bool clicked;
    private Vector3 targetPos;

    private void OnEnable()
    {
        clicked = false;
        targetPos = transform.localPosition;

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right };
        Vector3 randomDir = directions[Random.Range(0, directions.Length)];
        transform.localPosition = targetPos + randomDir * spawnOffset;
        transform.localScale = Vector3.zero;

        transform.DOLocalMove(targetPos, flyInDuration).SetUpdate(true);
        transform.DOScale(Vector3.one, flyInDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clicked) return;
        clicked = true;

        transform.DOKill();

        transform.DOScale(0f, shrinkDuration).SetUpdate(true)
            .OnComplete(() =>
            {
                transform.DOScale(growScale, growDuration).SetEase(Ease.OutBack).SetUpdate(true)
                    .OnComplete(() => StartCoroutine(PlayWinAndExit()));
            });

        GameFlowManager.Instance?.OnCupClicked();
    }

    private IEnumerator PlayWinAndExit()
    {
        spineAnim.AnimationState.SetAnimation(0, AnimEvents.ANIM_ANIMATION, true);
        yield return new WaitForSecondsRealtime(exitDelay);
        GameFlowManager.Instance?.OnWinChanged();
    }
}