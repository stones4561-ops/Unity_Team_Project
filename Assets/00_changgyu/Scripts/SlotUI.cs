using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SlotUI : MonoBehaviour,IBeginDragHandler, IEndDragHandler,IDragHandler,IDropHandler
{
   
    [SerializeField]
    private Image image;
    [SerializeField]
    private TextMeshProUGUI itemCount;

    private Slot mySlot;
    public Slot MySlot
    { get { return mySlot; } }
    private InventoryUI inventoryUI;
   


    public void Init(InventoryUI _inventoryUI)
    {
        inventoryUI = _inventoryUI;
    }

  

    /// <summary>
    /// 슬롯의 아이템과 수를 갱신하는 메서드.
    /// </summary>
    /// <param name="_slot"></param>
    public void ShowSlot(Slot _slot)
    {
        mySlot = _slot;
        if(_slot.CurItemData == null)
        {
            image.gameObject.SetActive(false);
            itemCount.text = "";
        }
        else
        {
            image.gameObject.SetActive(true);
            image.sprite = _slot.CurItemData.itemImage;
            itemCount.text = _slot.CurItemMany.ToString();
        }
         
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (mySlot == null || !mySlot.IsItem) return;

        inventoryUI.StartDragIcon(mySlot.CurItemData.itemImage);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (mySlot == null || !mySlot.IsItem) return;

        inventoryUI.MoveDragIcon(eventData.position);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
        inventoryUI.EndDragIcon();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if(eventData.pointerDrag==null) return; 

        SlotUI draggedSlotUI = eventData.pointerDrag.GetComponent<SlotUI>();

        if(draggedSlotUI != null&&draggedSlotUI!=this&&draggedSlotUI.mySlot.IsItem)
        {
            if(eventData.button == PointerEventData.InputButton.Left)
            {
                SwapSlotData(draggedSlotUI.mySlot, this.mySlot);
                inventoryUI.Redraw();
            }
            else if(eventData.button == PointerEventData.InputButton.Right && draggedSlotUI.mySlot.CurItemMany >= 2)
            {
                if(!this.mySlot.IsItem)
                {
                    inventoryUI.OpenSplitPopup(draggedSlotUI.MySlot, this.mySlot);
                }
            }
            else
            {
                if (!this.mySlot.IsItem)
                {
                    SwapSlotData(draggedSlotUI.mySlot, this.mySlot);
                    inventoryUI.Redraw();
                }
            }



        }
    }

    /// <summary>
    /// 두 슬롯의 데이터를 교환하는 보조 메서드
    /// </summary>
    /// <param name="_slotA"></param> 첫번째 슬롯
    /// <param name="_slotB"></param> 두번째 슬롯
    private void SwapSlotData(Slot _slotA, Slot _slotB)
    {
        ItemSO dataA = _slotA.CurItemData;
        int countA = _slotA.CurItemMany;
        bool isItemA = _slotA.IsItem;

        ItemSO dataB = _slotB.CurItemData;
        int countB = _slotB.CurItemMany;
        bool isItemB = _slotB.IsItem;

        _slotA.ItemClear();
        _slotB.ItemClear();

        if (isItemB) _slotA.SetItem(dataB, countB);
        if (isItemA) _slotB.SetItem(dataA, countA);
    }
}
