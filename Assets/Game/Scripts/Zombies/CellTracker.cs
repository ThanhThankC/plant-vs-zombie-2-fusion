using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTracker : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }

    public PlantBase TargetPlant { get; private set; }

    private ZombieAnimationController animationController;
    private Cell currentCell;
    private Cell previousCell;

    private void Awake()
    {
        animationController = GetComponentInParent<ZombieAnimationController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cell"))
        {
            var zone = other.GetComponent<Zone>();
            if (zone == null) return;

            if (currentCell != null) previousCell = currentCell;
            currentCell = zone.Cell;

            Row = zone.Cell.Row;
            Col = zone.Cell.Col;

            zone.Cell.OnPlantChanged += HandleWithCellHasPlant;
            HandleWithCellHasPlant();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cell"))
        {
            var zone = other.GetComponent<Zone>();
            if (zone == null) return;

            if (previousCell == zone.Cell) previousCell = null;
            if (currentCell == zone.Cell) currentCell = null;

            zone.Cell.OnPlantChanged -= HandleWithCellHasPlant;
        }
    }

    private void HandleWithCellHasPlant()
    {
        TargetPlant = previousCell?.GetPlantInstance(FieldType.Support)
                    ?? currentCell?.GetPlantInstance(FieldType.Support);
        animationController.RefreshAnimation();
    }

    public Cell GetCurrentCell() => GridManager.Instance.GetCell(Row, Col);
}
