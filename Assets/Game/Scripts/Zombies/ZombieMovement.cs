using Spine.Unity;
using Spine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SkeletonAnimation))]
public class ZombieMovement : MonoBehaviour
{
    [SerializeField] private float baseSpeedMultiplier = 1f;

    private SkeletonAnimation skeletonAmin;
    private bool canMove = true;
    private float speedMultiplier = 1f;

    private void Awake()
    {
        skeletonAmin = GetComponent<SkeletonAnimation>();
    }

    private void OnEnable()
    {
        skeletonAmin.AnimationState.Event += OnSpineEvent;
    }

    private void OnDisable()
    {
        skeletonAmin.AnimationState.Event -= OnSpineEvent;
    }

    private void OnSpineEvent(TrackEntry trackEntry, Spine.Event e)
    {
        if (e.Data.Name != "move_step") return;
        if (!canMove) return;
        transform.Translate(Vector3.left * e.Float * baseSpeedMultiplier * speedMultiplier);
    }

    public void OnStun()
    {
        canMove = false;
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

    private void SetAnimationSpeed(float speed) => skeletonAmin.AnimationState.GetCurrent(0).TimeScale = speed;
}
