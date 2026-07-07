using UnityEngine;
using System.Collections.Generic;


public class DropItemManager : MonoBehaviour
{
    private static DropItemManager instance;
    public static DropItemManager Instance
    { get { return instance; } }

    [SerializeField]
    private GameObject baseDropPrefab;

    private Queue<DropItem> itemPool = new Queue<DropItem>();


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SpawnItem(ItemSO _data, Vector3 _position)
    {
        DropItem item;

        if (itemPool.Count == 0)
        {
            item = Instantiate(baseDropPrefab).GetComponent<DropItem>();
            Debug.Log("»ý¼º");
        }
        else
            item = itemPool.Dequeue();

        item.transform.position = _position;
        item.SetUp(_data);
        item.gameObject.SetActive(true);
        item.PopUp();

    }

    public void ReturnToPool(DropItem _item)
    {
        _item.gameObject.SetActive(false);
        itemPool.Enqueue(_item);
    }
}
