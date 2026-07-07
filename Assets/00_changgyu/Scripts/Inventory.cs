using UnityEngine;

public class Inventory : MonoBehaviour
{
    private Slot[] inventory ;


    [SerializeField]
    private Item testitem;
    [SerializeField]
    private Item testitem2;

   

    public void InitInventory()
    {
        inventory = new Slot[PlayerInventory.Instance.slotCount];
        for (int i = 0; i < inventory.Length; i++)
            inventory[i] = new Slot();
    }

    #region 아이템 추가

    /// <summary>
    /// 인벤토리에 아이템을 하나만 추가하는 메서드
    /// </summary>
    /// <param name="_item"></param>
    public bool AddItem(ItemSO _item)
    {
        //먼저 같은 아이템이 있는지 찾고 있으면 수량 증가
        int index = FindFirstSameItem(_item);
        if (index != -1)
        {
            inventory[index].ItemUp(1);
            return true;
        }
        else
        {
            //같은 아이템이 없다면 빈 슬롯이 있는지 찾아봄, 없으면 꽉 찼다는 뜻
            index = FindFirstEmptySlot();
            if (index != -1)
            {
                inventory[index].SetItem(_item);
                return true;
            }
            else
            {
                Debug.Log("인벤토리가 꽉 찼습니다!");
                return false;
            }
        }

    }


    /// <summary>
    /// 아이템을 대량으로 추가하는 메서드, 공간이 충분하지 않으면 거부됨.
    /// </summary>
    /// <param name="_item"></param>
    /// <param name="_count"></param>
    public void AddManyItem(Item _item, int _count)
    {
        // 인벤토리의 빈공간을 체크, 빈공간이 더 많으면 실행, 그렇지 않으면 거부
        if (HowManyItemSpace(_item) >= _count)
        {
            for (int i = 0; i < inventory.Length; i++)
            {
                // 아이템이 있는 슬롯을 찾았고 그 슬롯이 꽉 안찼으면
                if (inventory[i].CurItemData == _item.Data && !inventory[i].isFull())
                {
                    // 채우고 끝
                    if (inventory[i].RemainToFull() > _count)
                    {
                        inventory[i].ItemUp(_count);
                        return;
                    }
                    // 아직 더 채워야함
                    else
                    {
                        int remain = inventory[i].RemainToFull();
                        inventory[i].ItemUp(remain);
                        _count -= remain;
                    }
                }
            }
            //코드가 여기를 진행한다는것은 아직 더 채워야 한다는 뜻
            for (int i = 0; i < inventory.Length; i++)
            {
                if (!inventory[i].IsItem)
                {
                    // 채우고 끝
                    if (_item.Data.max >= _count)
                    {
                        inventory[i].SetItem(_item.Data, _count);
                        return;
                    }
                    // 아직 더 채워야함
                    else
                    {
                        inventory[i].SetItem(_item.Data, _item.Data.max);
                        _count -= _item.Data.max;
                    }

                }
            }
        }
        else
        {
            Debug.LogError("인벤토리가 초과되었습니다!");
        }


    }


    /// <summary>
    /// 추가에 쓰는 보조 메서드, 인벤토리를 뒤져서 해당 아이템이 가장 먼저 나오는 슬롯을 찾음, 없으면 -1 리턴
    /// </summary>
    /// <param name="_data"></param>
    /// <returns></returns>
    private int FindFirstSameItem(ItemSO _data)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].CurItemData == _data && !inventory[i].isFull())
                return i;
        }
        return -1;
    }


    /// <summary>
    /// 추가에 쓰는 보조 메서드, 가장 먼저 나오는 빈 슬롯을 찾음, 없으면 -1 리턴
    /// </summary>
    /// <returns></returns>
    private int FindFirstEmptySlot()
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].IsItem)
                return i;
        }
        return -1;
    }



    /// <summary>
    /// AddManyItem을 발동하기전 해당 아이템이 최대한 들어갈 수 있는 갯수를 리턴하는 메서드
    /// </summary>
    /// <param name="_item"></param>
    /// <returns></returns>
    private int HowManyItemSpace(Item _item)
    {
        int totalSpace = 0;

        for (int i = 0; i < inventory.Length; i++)
        {

            if (inventory[i].CurItemData == _item.Data && !inventory[i].isFull())
            {
                totalSpace += inventory[i].RemainToFull();
            }
        }

        for (int i = 0; i < inventory.Length; i++)
        {
            if (!inventory[i].IsItem)
            {
                totalSpace += _item.Data.max;

            }
        }

        return totalSpace;
    }



    #endregion

    #region 아이템 제거

    /// <summary>
    /// 해당 아이템이 있는지 체크, 있으면 가장 적은 슬롯에서부터 제거. 없으면 없다고 로그 띄움
    /// </summary>
    /// <param name="_item"></param>
    public void RemoveItem(Item _item)
    {
        int index = FindMinSameItem(_item.Data);
        if (index == -1)
            Debug.LogError("아이템이 없습니다!");
        else
        {
            inventory[index].ItemDown(1);
            if (inventory[index].isEmpty())
                inventory[index].ItemClear();
        }

    }

    /// <summary>
    /// 아이템을 대량으로 제거하는 메서드. 보유 한도보다 제거하려는 수가 많으면 거부됨.
    /// </summary>
    /// <param name="_item"></param>
    /// <param name="_count"></param>
    public void RemoveManyItem(Item _item, int _count)
    {
        if (_count <= HowManyItem(_item))
        {
            while (_count > 0)
            {
                int index = FindMinSameItem(_item.Data);
                int indexsCount = inventory[index].CurItemMany;
                inventory[index].ItemDown(Mathf.Min(_count, indexsCount));
                if (inventory[index].isEmpty())
                    inventory[index].ItemClear();
                _count -= indexsCount;
                Debug.Log(_count);

            }
        }
        else
        {
            Debug.Log("아이템의 갯수가 충분하지 않습니다!");
        }
    }


    /// <summary>
    /// 아이템 제거에 사용되는 보조 메서드, 인벤토리를 뒤져서 해당 아이템을 가장 적게 갖고있는 슬롯을 리턴, 혹여나 없을경우 -1을 리턴
    /// </summary>
    /// <param name="_data"></param>
    /// <returns></returns>
    private int FindMinSameItem(ItemSO _data)
    {
        int index = -1;
        int min = _data.max;
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].CurItemData == _data)
            {
                if (inventory[i].CurItemMany <= min)
                {
                    min = inventory[i].CurItemMany;
                    index = i;
                }
            }
        }

        return index;
    }

    /// <summary>
    /// 아이템 대량 제거에 사용되는 보조 메서드, 인벤토리를 뒤져서 해당 아이템이 총 몇개있는지 계산함.
    /// </summary>
    /// <param name="_item"></param>
    /// <returns></returns>
    private int HowManyItem(Item _item)
    {
        int itemCount = 0;

        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].CurItemData == _item.Data)
                itemCount += inventory[i].CurItemMany;
        }

        return itemCount;
    }


    ///// <summary>
    ///// 해당 인덱스의 아이템을 갯수만큼 줄임. 수량이 0이 되면 해당 인덱스의 SO데이터를 없앰.
    ///// </summary>
    ///// <param name="_index"></param>
    ///// <param name="_count"></param>
    //public void RemoveItemIndex(int _index,int _count)
    //{
    //    inventory[_index].ItemDown(_count);
    //    if (inventory[_index].isEmpty())
    //        inventory[_index].ItemClear();
    //}
    //임시로 비활성화, 만약 제거기능을 보고 다시 활성화 결정

    #endregion



    #region 외부 메서드

    /// <summary>
    /// 해당 인덱스의 슬롯 정보를 리턴
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public Slot GetSlot(int index)
    { return inventory[index]; }


    #endregion



    public bool CanAddItem(ItemSO _itemData)
    {
        int index = FindFirstSameItem(_itemData);
        if (index != -1) return true;

        index = FindFirstEmptySlot();
        if (index != -1) return true;

        return false;
    }













    //public void testAdd()
    //{
    //    AddItem(testitem);
    //}

    //public void testAdd2()
    //{
    //    AddItem(testitem2);
    //}

    public void testRemove1()
    {
        RemoveItem(testitem);
    }

    public void testAddMany()
    {
        RemoveManyItem(testitem, 10);
    }

    public void testClear()
    {
        if (inventory[1].IsItem)
            inventory[1].ItemClear();
    }

    
}
