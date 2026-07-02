using UnityEngine;
using UnityEngine.EventSystems;

public class TrashcanUi : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        SlotUI draggedSlotUI=eventData.pointerDrag.GetComponent<SlotUI>();

        if(draggedSlotUI!=null&&draggedSlotUI.MySlot.IsItem)
        {
            draggedSlotUI.MySlot.ItemClear();
            PlayerInventory.Instance.playerInvenUI.Redraw();
        }
    }


}
