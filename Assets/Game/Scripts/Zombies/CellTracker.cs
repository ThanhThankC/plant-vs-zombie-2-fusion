using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTracker : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }

    private float nextColBoundaryX = float.MaxValue;
    private bool isLeft = true;

    public void Init(int row, int col)
    {
        Row = row;
        Col = col;
        UpdateBoundary();
    }

    private void Update()
    {
        if (isLeft && transform.position.x < nextColBoundaryX)
        {
            Col--;
            UpdateBoundary();
        }
        else if (!isLeft && transform.position.x > nextColBoundaryX)
        {
            Col++;
            UpdateBoundary();
        }
    }

    private void UpdateBoundary()
    {
        var cell = GridManager.Instance.GetCell(Row, Col);
        if (cell == null) return;
        nextColBoundaryX = cell.transform.position.x - GridManager.Instance.CellSize.x * 0.5f;
    }

    public Cell GetCurrentCell() => GridManager.Instance.GetCell(Row, Col);
}
