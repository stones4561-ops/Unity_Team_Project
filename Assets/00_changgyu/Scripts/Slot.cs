using UnityEngine;

public class Slot 
{
    private ItemSO curItemData;
    public ItemSO CurItemData
    { get { return curItemData; } }
    private int curItemMany;
    public int CurItemMany
    {  get { return curItemMany; } }
    private bool isItem=false;
    public bool IsItem
        { get { return isItem; } }

    //빈 슬롯에 아이템을 추가하는 메서드
    public void SetItem(ItemSO _data)
    {
        curItemData = _data;
        curItemMany = 1;
        isItem = true;
    }

    //상점 상호작용으로 쓸 대량 추가 메서드
    public void SetItem(ItemSO _data,int _count)
    {
        curItemData = _data;
        curItemMany = _count;
        isItem = true;
    }

    //이미 있는 슬롯에 수량을 증가시키는 메서드
    public void ItemUp(int _count)
    {
        if (!isItem)
            Debug.LogError("뭐 잘못됐다잉-여 아이템 읎다");
        curItemMany+=_count;
    }

    //이미 있는 슬롯에 수량을 감소시키는 메서드, 필요 이상 빼기 주문시 에러남 다 빼고 싶으면 아래 메서드 이용
    public void ItemDown(int _count)
    {
        if (!isItem)
            Debug.LogError("뭐 잘못됐다잉-여 아이템 읎다");
        else if (curItemMany < _count)
            Debug.LogError("뭐 잘못됐다잉-그만큼 읎다");
        else
            curItemMany-= _count;
    }

    //싹 쓸어버려 맷
    public void ItemClear()
    {
        curItemData = null;
        curItemMany=0;
        isItem=false;
    }

    //여기 다 찼음?
    public bool isFull()
    {
        if (curItemData == null) return false;

        return curItemMany >= curItemData.max;
    }

    //여기 비었지?
    public bool isEmpty()
    { return curItemMany <=0; }

    /// <summary>
    /// 얼마나 더 들어갈 수 있는지 알려주는 메서드
    /// </summary>
    /// <returns></returns>
    public int RemainToFull()
    {
        return curItemData.max - curItemMany;
    }

}
