using UnityEngine;

public class ToolManager : Singleton<ToolManager>
{
    [SerializeField] private GameObject glovePrefab;
    [SerializeField] private GameObject shovelPrefab;

    public GameObject Glove { get; private set; }
    public GameObject Shovel { get; private set; }
    public bool MovingPlant => gloveState == GloveState.PlantSelected;

    private GloveState gloveState;

    private void Start()
    {
        Glove = Instantiate(glovePrefab);
        Glove.SetActive(false);
        Shovel = Instantiate(shovelPrefab);
        Shovel.SetActive(false);
    }

    public void OnDragBegin()
    {
        if (MovingPlant) ChangeGlovePhase();
        if (MovingPlant) Debug.Log("Hello");
    }

    public void EndDrag()
    {
        if (MovingPlant) gloveState = GloveState.Idle;
    }

    public void SelectPlantWithGlove(PlantBase plant, Cell cell)
    {
        gloveState = GloveState.PlantSelected;
        PlantManager.Instance.OnCellClicked(plant, cell);
        DragController.Instance.TransitionToPlantDrag();
    }

    public void RemovePlantWithShovel(PlantBase plant, Cell cell)
    {
        PlantManager.Instance.DestroyPlantAt(cell, plant.PlantType.GetFieldType());
    }

    private void ChangeGlovePhase()
    {
        gloveState = GloveState.Idle;
    }
}
