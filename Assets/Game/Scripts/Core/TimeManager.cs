using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimeManager : MonoBehaviour
{
    [SerializeField] private Button speedBtn;
    [SerializeField] private Button normalBtn;

    private void Start()
    {
        speedBtn.onClick.AddListener(SpeedDown);
        normalBtn.onClick.AddListener(NormalSpeed);
    }

    private void SpeedDown() => Time.timeScale = 0.2f;
    private void NormalSpeed() => Time.timeScale = 1f;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            SpeedDown();
        if (Input.GetKeyDown(KeyCode.W))
            NormalSpeed();
    }
}
