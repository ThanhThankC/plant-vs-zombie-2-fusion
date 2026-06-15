using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Cell : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private SpriteRenderer highlightRenderer;
    [SerializeField] private Zone normalZone;
    [SerializeField] private Zone supportZone;

    public int Row { get; private set; }
    public int Col { get; private set; }
    public CellType CellType { get; private set; }

    public PlantType? NormalPlant { get; private set; }
    public PlantType? SupportPlant { get; private set; }

    public PlantBase NormalPlantInstance { get; private set; }
    public PlantBase SupportPlantInstance { get; private set; }

    public Zone NormalZone => normalZone;
    public Zone SupportZone => supportZone;

    public Vector2 Position { get; private set; }

    public event Action OnPlantChanged;

    public void Init(int row, int col, CellType cellType, Vector2 pos)
    {
        Row = row;
        Col = col;
        CellType = cellType;
        Position = pos;
    }

    public bool CanPlant() => CellType == CellType.Plantable;

    public void SetPlant(FieldType fieldType, PlantType plantType, PlantBase instance)
    {
        if (fieldType == FieldType.Normal)
        {
            NormalPlant = plantType;
            NormalPlantInstance = instance;
        }
        else
        {
            SupportPlant = plantType;
            SupportPlantInstance = instance;
        }
        OnPlantChanged?.Invoke();
    }

    public void ClearPlant(FieldType fieldType)
    {
        if (fieldType == FieldType.Normal)
        {
            NormalPlant = null;
            NormalPlantInstance = null;
        }
        else
        {
            SupportPlant = null;
            SupportPlantInstance = null;
        }
        OnPlantChanged?.Invoke();
    }

    public PlantBase GetPlantInstance(FieldType prioritiedFiled) 
            => prioritiedFiled == FieldType.Normal                                   
            ? NormalPlantInstance ?? SupportPlantInstance 
            : SupportPlantInstance ?? NormalPlantInstance;

    public void ToggleHighlight(bool show)
    {
        if (highlightRenderer != null)
            highlightRenderer.gameObject.SetActive(show);
    }
}
