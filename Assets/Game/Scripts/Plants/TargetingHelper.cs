using UnityEngine;

public class TargetingHelper : MonoBehaviour
{
    private enum GizmoShape { Square, Circle, Rectangle }

    [Header("Gizmos")]
    [SerializeField] private GizmoShape gizmoShape = GizmoShape.Square;
    [SerializeField] private bool isShowGizmos;

    [Header("Square / Circle")]
    [SerializeField] private float range;

    [Header("Rectangle")]
    [SerializeField] private float width;
    [SerializeField] private float height;

    [Header("References")]
    [SerializeField] private Transform attackPoint;

    public Collider2D[] GetTargetsInBox(Vector3 attackPoint)
        => Physics2D.OverlapBoxAll(attackPoint, new Vector2(range, range), 0f);

    public Collider2D[] GetTargetsInCircle(Vector3 attackPoint)
        => Physics2D.OverlapCircleAll(attackPoint, range);

    public Collider2D[] GetTargetsInRect(Vector3 attackPoint)
        => Physics2D.OverlapBoxAll(attackPoint, new Vector3(width, height), 0f);

    private void OnDrawGizmos()
    {
        if (!isShowGizmos) return;

        switch (gizmoShape)
        {
            case GizmoShape.Square:
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(attackPoint.position, new Vector3(range, range, 0f));
                break;
            case GizmoShape.Circle:
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(attackPoint.position, range);
                break;
            case GizmoShape.Rectangle:
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(attackPoint.position, new Vector3(width, height, 0f));
                break;
        }
    }
}
