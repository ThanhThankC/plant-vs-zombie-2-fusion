using UnityEngine;
using DG.Tweening;
using System;

public class TweenAnimator : MonoBehaviour
{
    [System.Serializable]
    private class RotationEntry
    {
        public GameObject obj;
        public Vector3 axis = Vector3.back;
        public float degreesPerSecond = 120f;
        [System.NonSerialized] public float angle;
    }

    [System.Serializable]
    private class ScaleEntry
    {
        public GameObject obj;
        public float minSize = 0.8f;
        public float maxSize = 1.2f;
        public float scaleDuration = 1f;
    }

    [Header("Rotation")]
    [SerializeField] private RotationEntry[] rotEntries;

    [Header("Scale")]
    [SerializeField] private ScaleEntry[] scEntries;

    [Header("Fade Out")]
    [SerializeField] private SpriteRenderer[] fadeRenderer;

    void Awake()
    {
        DOTween.SetTweensCapacity(tweenersCapacity: 500, sequencesCapacity: 200);
    }

    private void OnEnable()
    {
        if (rotEntries != null && rotEntries.Length > 0)
            foreach (var e in rotEntries)
                RotateObject(e);

        if (scEntries != null && scEntries.Length > 0)
            foreach (var e in scEntries)
                ScaleObject(e);
    }

    public void FadeOutObject(float fadeDuration, Action onComplete = null)
    {
        if (fadeRenderer == null || fadeRenderer.Length == 0) return;

        Sequence seq = DOTween.Sequence();

        foreach (var render in fadeRenderer)
        {
            if (render == null) continue;

            Color c = render.color;
            Tween t = DOTween.To(
                () => c.a,
                x => {
                    if (render == null) return;
                    render.color = new Color(c.r, c.g, c.b, x);
                },
                0f,
                fadeDuration
            ).SetTarget(render);

            seq.Join(t); 
        }

        seq.OnComplete(() => onComplete?.Invoke());
    }

    public void ResetAlpha()
    {
        if (fadeRenderer == null || fadeRenderer.Length == 0) return;
        foreach (var render in fadeRenderer)
        {
            if (render == null) continue;
            Color c = render.color;
            render.color = new Color(c.r, c.g, c.b, 1f);
        }
    }

    private void RotateObject(RotationEntry e)
    {
        float duration = 360f / Mathf.Abs(e.degreesPerSecond);
        float sign = Mathf.Sign(e.degreesPerSecond);

        DOTween
            .To(() => e.angle, v => { e.angle = v; e.obj.transform.localRotation = Quaternion.AngleAxis(v, e.axis.normalized); },
                e.angle + 360f * sign, duration)
            .SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Incremental)
            .SetTarget(e.obj.transform);
    }
    
    private void ScaleObject(ScaleEntry e)
    {
        e.obj.transform
            .DOScale(e.maxSize, e.scaleDuration)
            .From(e.minSize)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetTarget(e.obj.transform);
    }

    void OnDisable()
    {
        if (rotEntries != null && rotEntries.Length > 0)
            foreach (var e in rotEntries)
                DOTween.Kill(e.obj.transform);

        if (scEntries != null && scEntries.Length > 0)
            foreach (var e in scEntries)
                DOTween.Kill(e.obj.transform);
    }
}