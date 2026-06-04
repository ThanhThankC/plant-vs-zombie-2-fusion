using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class ZombieMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeedMultiplier = 3f;

    private SkeletonAnimation skeletonAnim;
    private bool canMove = true;
    private float speedMultiplier = 1f;
    private Vector3 targetPosition;
    private bool isMoving = false;

    private void Awake()
    {
        skeletonAnim = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable() => skeletonAnim.AnimationState.Event += OnSpineEvent;

    private void OnDisable() => skeletonAnim.AnimationState.Event -= OnSpineEvent;

    private void Update()
    {
        if (!isMoving) return;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.001f)
            isMoving = false;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name != AnimEvents.EVENT_MOVE) return;
        if (!canMove) return;

        float stepDistance = e.Float * baseSpeedMultiplier * speedMultiplier;
        targetPosition = transform.position + Vector3.left * stepDistance;
        isMoving = true;
    }

    public void AllowMove()
    {
        canMove = true;
    }

    public void DisallowMove()
    {
        canMove = false;
        isMoving = false;
        targetPosition = transform.position;
    }

    public void OnStun()
    {
        DisallowMove();
        SetAnimationSpeed(0f);
    }

    public void OnSlow(float multiplier)
    {
        canMove = true;
        speedMultiplier = multiplier;
        SetAnimationSpeed(multiplier);
    }

    public void ResetSpeed()
    {
        canMove = true;
        speedMultiplier = 1f;
        SetAnimationSpeed(1f);
    }

    private void SetAnimationSpeed(float speed) => skeletonAnim.AnimationState.GetCurrent(0).TimeScale = speed;
}
