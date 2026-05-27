using System;
using UnityEngine;

public class DragController : Singleton<DragController>
{
    public ToolType CurrentToolType { get; private set; } = ToolType.None;
    public bool IsDragging { get; private set; } = false;
    public GameObject CurrentTool { get; private set; }

    public event Action OnDragEnd;

    private float toolFlashTimer = 0f;
    private const float ToolFlashDuration = 0.07f;

    private void Update()
    {
        if (toolFlashTimer > 0f)
        {
            toolFlashTimer -= Time.deltaTime;
            if (!isFlashing) TryEndDrag();
        }
    }

    public void SetTool(ToolType tool)
    {
        CurrentToolType = tool;
        CurrentTool = GetCurrentTool();
    }

    public void NotifyBeginDrag() => ToolManager.Instance.OnDragBegin();

    public void BeginDrag(ToolType tool)
    {
        IsDragging = true;
        SetTool(tool);
    }

    public void TryEndDrag()
    {
        if (ToolManager.Instance.MovingPlant || isFlashing) return;
        EndDrag();
    }

    public void RefreshTool() => EndDrag();

    public void ShowTool()
    {
        if (CurrentTool != null)
            CurrentTool.gameObject.SetActive(true);
    }

    public void ShowToolAt(Vector3 pos)
    {
        if (CurrentTool == null) return;
        if (!CurrentTool.gameObject.activeSelf)
            CurrentTool.gameObject.SetActive(true);
        CurrentTool.transform.position = pos;
    }

    private void HideTool()
    {
        if (CurrentTool != null)
            CurrentTool.gameObject.SetActive(false);
        CurrentTool = null;
    }

    public bool CanHover(Zone zone)
    {
        if (CurrentToolType == ToolType.Shovel || CurrentToolType == ToolType.Glove)
        {
            var fieldType = zone.FieldType;
            var other = fieldType == FieldType.Normal ? FieldType.Support : FieldType.Normal;
            var instance = zone.Cell.GetPlantInstance(fieldType) ?? zone.Cell.GetPlantInstance(other);
            return instance != null;
        }
        var plantType = PlantManager.Instance.CurrentPlantType;
        if (!plantType.HasValue) return false;
        return zone.GetFusionResult(plantType.Value).HasValue;
    }

    public void FlashThenEndDrag()
    {
        toolFlashTimer = ToolFlashDuration;
    }

    public void TransitionToPlantDrag()
    {
        HideTool();
        SetTool(ToolType.None);
        ShowTool();
    }

    private void EndDrag()
    {
        IsDragging = false;
        HideTool();
        switch (CurrentToolType)
        {
            case ToolType.None:
                PlantManager.Instance.EndDrag();
                break;
            case ToolType.Glove | ToolType.Shovel:
                ToolManager.Instance.EndDrag();
                break;
        }
        SetTool(ToolType.None);
        OnDragEnd?.Invoke();
    }

    private GameObject GetCurrentTool()
    {
        switch (CurrentToolType)
        {
            case ToolType.None:
                return PlantManager.Instance.GhostPlant;
            case ToolType.Glove:
                return ToolManager.Instance.Glove;
            case ToolType.Shovel:
                return ToolManager.Instance.Shovel;
            default: return null;
        }
    }

    private bool isFlashing => toolFlashTimer > 0f;
}
