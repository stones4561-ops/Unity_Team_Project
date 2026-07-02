using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private ItemSO data;
    public ItemSO Data { get { return data; } }
    

    private int m_sellPrice;
    public int SellPrice
    { get { return m_sellPrice;} }
    private int m_max;
    public int Max
    {  get { return m_max;} } 
    private SpriteRenderer m_spriteRenderer;

    private void Awake()
    {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        m_sellPrice=data.sellPrice;
        m_max=data.max;
        m_spriteRenderer.sprite = data.itemImage;   
    }
}
