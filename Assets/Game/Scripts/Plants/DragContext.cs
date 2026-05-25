public enum DragSource { Deck, Cell}

public class DragContext
{
    public PlantBase Plant { get; set; }
    public DragSource DragSource { get; set; }
    public Cell SourceCell { get; set; }
    public FieldType SourceFieldType { get; set; }
    public PlantType PlantType { get; set; }
}
