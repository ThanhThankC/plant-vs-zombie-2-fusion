using UnityEngine.EventSystems;

public interface IDraggableButton : IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler { }

public interface ISelectableButton
{
    void Select();
    void Deselect();
}