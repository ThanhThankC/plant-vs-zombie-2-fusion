using UnityEngine;

public class CellTracker : MonoBehaviour
{
    public int Row { get; private set; }
    public int Col { get; private set; }
    public Cell CurrentCell { get; private set; }
    public Cell PreviousCell { get; private set; }

    public PlantBase TargetPlant { get; private set; }

    private ZombieBase zombie;
    private ZombieAnimationController animationController;

    private void Awake()
    {
        zombie = GetComponentInParent<ZombieBase>();
        animationController = GetComponentInParent<ZombieAnimationController>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Cell"))
        {
            var zone = other.GetComponent<Zone>();
            if (zone == null) return;

            if (CurrentCell != null) PreviousCell = CurrentCell;
            CurrentCell = zone.Cell;

            Row = zone.Cell.Row;
            Col = zone.Cell.Col;

            zone.Cell.OnPlantChanged += HandleWithCellHasPlant;
            HandleWithCellHasPlant();

            zombie.SetupVisual(zone.Cell);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Cell"))
        {
            var zone = other.GetComponent<Zone>();
            if (zone == null) return;

            if (PreviousCell == zone.Cell) PreviousCell = null;
            if (CurrentCell == zone.Cell) CurrentCell = null;

            zone.Cell.OnPlantChanged -= HandleWithCellHasPlant;
        }
    }

    private void HandleWithCellHasPlant()
    {
        TargetPlant = PreviousCell?.GetPlantInstanceForZombie(zombie.IsEatAnim)
        ?? CurrentCell?.GetPlantInstanceForZombie(zombie.IsEatAnim);

        animationController.RefreshState();
    }
}
