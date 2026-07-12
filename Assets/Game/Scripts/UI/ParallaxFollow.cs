using UnityEngine;

public class ParallaxFollow : MonoBehaviour
{
    [SerializeField] private RectTransform scrollTarget;
    [SerializeField] private float parallaxFactor = 0.5f;
    [SerializeField] private float smoothTime = 0.1f;

    private Vector3 initialLocalPos;
    private Vector3 initialTargetPos;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        initialLocalPos = transform.localPosition;
        initialTargetPos = scrollTarget.localPosition;
    }

    void Update()
    {
        Vector3 delta = scrollTarget.localPosition - initialTargetPos;
        Vector3 targetPos = initialLocalPos + delta * parallaxFactor;

        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, targetPos, ref velocity, smoothTime);
    }
}
