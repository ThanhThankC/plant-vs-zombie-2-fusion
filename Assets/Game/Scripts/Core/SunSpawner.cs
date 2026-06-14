using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunSpawner : MonoBehaviour
{
    [SerializeField] private Sun sunPrefab;
    [SerializeField] List<SunData> sunDatas;

    [SerializeField] private float spawnHeight = 3f;
    
    private float firstDuration;
    private float interval;
    private float topEdge;
    private float rightEdge;
    private float leftEdge;
    private float bottomEdge;
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
        topEdge = rightTopCell.transform.position.y;
        rightEdge = rightTopCell.transform.position.x;
        leftEdge = leftBottomCell.transform.position.x;
        bottomEdge = leftBottomCell.transform.position.y;
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
        float offsetX = Random.Range(leftEdge, rightEdge);
        float offsetY = topEdge + spawnHeight;
        Vector3 pos = new Vector3(offsetX, offsetY, 0);
        float targetPosY = Random.Range(bottomEdge, topEdge);

        Sun sun = Instantiate(sunPrefab, pos, Quaternion.identity, transform);
        sun.Init(targetPosY);
    }
}
