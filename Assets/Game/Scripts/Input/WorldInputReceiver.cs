using UnityEngine;

public class WorldInputReceiver : MonoBehaviour
{
    [SerializeField] private LayerMask sunLayer;

    private readonly Collider2D[] sunHits = new Collider2D[5];

    private Camera mainCamera;
    private bool isGameStart = true;

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        //TODO: Game State Manager Conventions
        if (!isGameStart) return;

        int count = Physics2D.OverlapPointNonAlloc(GetMouseWorldPos(), sunHits, sunLayer);
        int loopCount = Mathf.Min(sunHits.Length, count);
        for (int i = 0; i < loopCount; i++)
            sunHits[i].GetComponent<Sun>()?.OnTap();

        if (loopCount > 0)
            AudioManager.Instance.PlaySunlick();
    }

    private Vector3 GetMouseWorldPos()
    {
        return mainCamera.ScreenToWorldPoint(Input.mousePosition);
    }
}
