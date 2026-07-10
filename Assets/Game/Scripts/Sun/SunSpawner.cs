using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSpawner : MonoBehaviour
{
    [SerializeField] private PoolKey sunKey;
    [SerializeField] List<SunData> sunDatas;

    [SerializeField] private float spawnHeight = 3f;
    
    private float firstDuration;
    private float interval;
    private Vector2 landingOffsetXRange;
    private Vector2 landingOffsetYRange;
    private bool autoSpawn = true;

    //TODO: Game State Manager Conventions
    private int currentLevel;
    private bool isGameOver;

    private void Awake()
    {
        InitData();
    }

    private void Start()
    {
        if (!autoSpawn) return;
        InitEdge();
        StartCoroutine(SpawnSun());
    }

    private void InitData()
    {
        var data = sunDatas[currentLevel];
        if (data == null) return;

        SunManager.Instance.InitSun(data.startingSun);
        autoSpawn = data.autoSpawn;
        firstDuration = data.firstDuration;
        interval = data.interval;
    }

    private void InitEdge()
    {
        //TODO: Variable Conventions
        Cell leftBottomCell = GridManager.Instance.GetCell(0, 1);
        Cell rightTopCell = GridManager.Instance.GetCell(5 - 1, 12 - 1);

        if (leftBottomCell == null || rightTopCell == null) return;
        landingOffsetXRange = new Vector2(leftBottomCell.Position.x, rightTopCell.Position.x);
        landingOffsetYRange = new Vector2(leftBottomCell.Position.y, rightTopCell.Position.y);
    }

    IEnumerator SpawnSun()
    {
        yield return new WaitForSeconds(firstDuration);
        while (!isGameOver)
        {
            CreateSun();
            yield return new WaitForSeconds(interval);
        }
    }

    private void CreateSun()
    {
        float offsetX = Random.Range(landingOffsetXRange.x, landingOffsetXRange.y);
        float offsetY = landingOffsetYRange.y + spawnHeight;
        Vector3 pos = new Vector3(offsetX, offsetY, 0);
        float targetPosY = Random.Range(landingOffsetYRange.x, landingOffsetYRange.y);

        var sun = PoolManager.Instance.Get<Sun>(sunKey, pos, Quaternion.identity);
        sun.InitStraight(targetPosY);
    }
}
