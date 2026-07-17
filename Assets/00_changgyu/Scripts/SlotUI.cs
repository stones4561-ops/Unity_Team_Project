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
        if (eventData.pointerDrag == null) return;

        SlotUI draggedSlotUI = eventData.pointerDrag.GetComponent<SlotUI>();

        if (draggedSlotUI != null && draggedSlotUI != this && draggedSlotUI.mySlot.IsItem)
        {
            // 💡 좌클릭: 아이템 이동 및 병합
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                // 1. 내려놓은 곳에 이미 같은 아이템이 있을 경우 (병합 로직)
                if (this.mySlot.IsItem && draggedSlotUI.MySlot.CurItemData == this.mySlot.CurItemData)
                {
                    // 💡 목적지 슬롯(this.MySlot)의 max 값을 기준으로 확인합니다.
                    if (draggedSlotUI.MySlot.CurItemMany + this.MySlot.CurItemMany <= this.MySlot.CurItemData.max)
                    {
                        // 목적지(this)에 드래그한 아이템 개수만큼 더해주고, 드래그한 슬롯은 비웁니다.
                        this.MySlot.ItemUp(draggedSlotUI.MySlot.CurItemMany);
                        draggedSlotUI.MySlot.ItemClear();
                    }
                    else
                    {
                        // 💡 목적지(this)의 남은 공간(RemainToFull)만큼만 가져옵니다.
                        int count = this.MySlot.RemainToFull();
                        this.MySlot.ItemUp(count);
                        draggedSlotUI.MySlot.ItemDown(count);
                    }

                    // 💡 중요: 병합이 끝났으므로 UI를 갱신하고 함수를 종료(return)하여 Swap이 실행되지 않게 막습니다!
                    inventoryUI.Redraw();
                    return;
                }

                // 2. 빈 슬롯이거나 서로 다른 아이템일 경우 (교환 로직)
                // 위에서 return 되지 않은 경우에만 여기까지 내려와서 스왑을 실행합니다.
                SwapSlotData(draggedSlotUI.mySlot, this.mySlot);
                inventoryUI.Redraw();
            }
            // 💡 우클릭: 아이템 나누기 (기존 코드 유지)
            else if (eventData.button == PointerEventData.InputButton.Right && draggedSlotUI.mySlot.CurItemMany >= 2)
            {
                if (!this.mySlot.IsItem)
                {
                    inventoryUI.OpenSplitPopup(draggedSlotUI.MySlot, this.mySlot);
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
