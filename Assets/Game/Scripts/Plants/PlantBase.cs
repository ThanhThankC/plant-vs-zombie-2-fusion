using System.Collections;
using UnityEngine;

public abstract class PlantBase : MonoBehaviour
{
    public PlantData Data { get; private set; }
    public PlantType PlantType => Data.plantType;
    public int CurrentHP { get; private set; }
    public bool IsActivated { get; private set; }
    public Cell OccupiedCell { get; private set; }
    public FieldType OccupiedFieldType { get; private set; }
    public bool IsGhost { get; private set; }

    public void Init(PlantData data)
    {
        Data = data;
        CurrentHP = data.maxHP;
    }

    public void SetupAsGhost(Cell cell, FieldType fieldType)
    {
        IsGhost = true;
        enabled = false;
        var render = GetComponent<SpriteRenderer>();
        if (render != null) render.sortingOrder = 10;
        SetAlpha(0.5f);
        if (cell == null) return;
        transform.position = cell.transform.position;
        transform.position += fieldType == FieldType.Normal 
            ? new Vector3(-0.2f, 0.5f, 0f) 
            : new Vector3(-0.2f, 0f, 0f);
    }

    public void SetupAsReal(Cell cell, FieldType fieldType)
    {
        IsGhost = false;
        enabled = true;
        SetAlpha(1f);
        OccupiedCell = cell;
        OccupiedFieldType = fieldType;
        int sorttingOder = fieldType == FieldType.Normal ? 7 : 8;
        var render = GetComponent<SpriteRenderer>();
        if (render != null) render.sortingOrder = sorttingOder;
        Vector3 pos;
        float offsetY = fieldType == FieldType.Normal ? 0.2f : -0.2f;
        pos = transform.position;
        pos.y += offsetY;
        transform.position = pos;
        transform.SetParent(cell.transform);
        transform.name = Data.name;
        OnPlaced();
        IsActivated = true;
    }

    //TODO: Do something when just set down.
    protected virtual void OnPlaced() { }

    public virtual void TakeDamage(int amount)
    {
        CurrentHP -= amount;
        transform.position += new Vector3(0f, 0.05f, 0f);
        if (CurrentHP <= 0) Die();
    }

    protected virtual void Die()
    {
        OccupiedCell?.ClearPlant(OccupiedFieldType);
        OccupiedCell = null;
        IsActivated = false;
        Destroy(gameObject);
    }

    protected void SetAlpha(float alpha) { }
}
