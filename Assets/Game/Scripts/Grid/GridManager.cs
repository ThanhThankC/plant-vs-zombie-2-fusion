using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : Singleton<GridManager>
{
    [SerializeField] private Cell cellPrefab;
    [SerializeField] private Vector2 cellSize = Vector2.one;
    [SerializeField] private float houseDistance = 3f;
    [SerializeField] private float leftScreenDistance = 2f;
    [SerializeField] private float loseThresholdX = -12f;

    public float LoseThresholdX => loseThresholdX;
    public const int ZombieSpawnCol = 11;
    public Vector2 CellSize => cellSize;
    public int Col => maxCol;
    public int Row => maxRow;

    private Cell[,] grid = new Cell[maxRow, maxCol];

    //TODO: Variable Conventions
    private const int maxCol = 12;
    private const int maxRow = 5;

    protected override void Awake()
    {
        base.Awake();
        BuildGrid();
    }

    private void BuildGrid()
    {
        for (int row = 0; row < maxRow; row++)
        {
            for (int col = 0; col < maxCol; col++)
            {
                Vector3 pos = new Vector3(
                    transform.position.x + col * cellSize.x,
                    transform.position.y + row * cellSize.y,
                    0f
                );
                Cell cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);

                cell.Init(row, col, GetCellType(col), pos);
                cell.transform.localScale = cellSize;
                cell.name = $"Cell[{row},{col}]";
                grid[row, col] = cell;
            }
        }
    }

    private CellType GetCellType(int col)
    {
        return col switch
        {
            0 => CellType.MowerZone,
            11 => CellType.ZombieSpawn,
            10 => CellType.BorderZone,
            _ => CellType.Plantable,
        };
    }

    public Cell GetCell(int row, int col)
    {
        if (row < 0 || row >= maxRow || col < 0 || col >= maxCol) return null;
        return grid[row, col];
    }

    public IEnumerable<Cell> GetRowFrom(int row, int fromCol)
    {
        for (int col = fromCol; col < maxCol; col++)
            yield return grid[row, col];
    }

    public List<Cell> GetCellsInRadius(int centerRow, int centerCol, int radius)
    {
        var result = new List<Cell>();
        for (int r = centerRow - radius; r <= centerRow + radius; r++)
        {
            for (int c = centerCol - radius; c <= centerRow + radius; c++)
            {
                var cell = GetCell(r, c);
                if (cell != null) result.Add(cell);
            }
        }
        return result;
    }

    public float GetHousePositionX() => GetCell(0, 0).transform.position.x - houseDistance;
    public float GetScreenPositionX() => GetCell(Row - 1, Col - 1).transform.position.x + leftScreenDistance;
}
