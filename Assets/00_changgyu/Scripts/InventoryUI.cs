using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField]
    private SlotUI slotPF;
    [SerializeField]
    private GameObject inventoryPannel;
    [SerializeField]
    private Inventory inven;
    [SerializeField]
    private Image dragIcon;
    [SerializeField]
    private SplitPopupUI splitPopup;

    private SlotUI[] slotUIArray;


    private void Start()
    {
        if (dragIcon != null)
            dragIcon.gameObject.SetActive(false);

        if(splitPopup!=null)
            splitPopup.gameObject.SetActive(false);


        
}

    public void InitInventoryUI()
    {
        slotUIArray = new SlotUI[PlayerInventory.Instance.slotCount];
        

        for (int i = 0; i < slotUIArray.Length; i++)
        {
            SlotUI slotUI = Instantiate(slotPF, inventoryPannel.transform);
            slotUI.Init(this);
            slotUIArray[i] = slotUI;
        }
        Redraw();
    }

    public void Redraw()
    {
        for(int i= 0; i < slotUIArray.Length; i++)
        {
            slotUIArray[i].ShowSlot(inven.GetSlot(i));
        }
    }

    public void OpenSplitPopup(Slot souce, Slot target)
    {
        if(splitPopup != null)
            splitPopup.OpenPopup(souce, target);
    }

    public void StartDragIcon(Sprite itemSprite)
    {
        if (dragIcon == null) return;

        dragIcon.sprite = itemSprite;

        Color color = dragIcon.color;
        color.a = 0.5f;
        dragIcon.color = color;

        dragIcon.gameObject.SetActive(true);
    }

    public void MoveDragIcon(Vector2 _mousePos)
    {
        if (dragIcon == null) return;
        dragIcon.transform.position = _mousePos;
    }

    public void EndDragIcon()
    {
        if (dragIcon == null) return;
        dragIcon.gameObject.SetActive(false);
    }

}
