using UnityEngine;

public class Sun : MonoBehaviour
{
    private enum SunState
    {
        None,
        StraightFall,
        Collecting,
        CurvedFall,
    }

    [SerializeField] private int cost = 25;
    [SerializeField] private float fallSpeed = 3f;
    [SerializeField] private float collectSpeed = 10f;
    [SerializeField] private float collectionDuration = 3f;

    public int Cost => cost;

    private SunState sunState = SunState.None;
    private Vector3 landPos;
    private Vector3 uiPos;

    public void Init(float landPosY)
    {
        landPos = new Vector3(transform.position.x, landPosY, 0f);
        sunState = SunState.StraightFall;
        uiPos = SunManager.Instance.SunCounterPos;
    }

    private void Update()
    {
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
        if (sunState != SunState.Collecting)
            sunState = SunState.Collecting;
    }

    private void FallingFollowStraight()
    {
        transform.position = Vector3.MoveTowards(transform.position, landPos, fallSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, landPos) <= 0.01f)
        {
            sunState = SunState.None;
            Invoke(nameof(OnTap), collectionDuration);
        }
    }

    private void Collecting()
    {
        transform.position = Vector3.Lerp(transform.position, uiPos, collectSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, uiPos) <= 0.01f)
        {
            SunManager.Instance.CollectSun(cost);
            Destroy(gameObject);
            sunState = SunState.None;
        }
    }

    private void FallingFollowCurved()
    {
        // Move follow principal of sunflower
    }
}
