using UnityEngine;
using UnityEngine.UI;

public class DropItem : MonoBehaviour
{
   

   
    private ItemSO itemData;
    public ItemSO ItemData
    { get { return itemData; } }


    private SpriteRenderer spriteRenderer;

    private bool isFollowing = false;
    public bool IsFollowing
    { get { return isFollowing; } }
    private Transform targetPlayer;

    private float currentSpeed = 10f;

    private bool collectable = false;
    public bool Collectable
    { get { return collectable; } }


    private Rigidbody rb;
    private Collider col;


    public void SetUp(ItemSO itemSO)
    {
        itemData = itemSO;
        spriteRenderer.sprite = itemSO.itemImage;
        
    }

    public void StartFollowing(Transform _player)
    {
        isFollowing = true;
        targetPlayer = _player;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isFollowing || targetPlayer == null) return;

        Vector3 targetPos = targetPlayer.transform.position;
        targetPos.y += 0.5f;

        transform.position = Vector3.MoveTowards
            (transform.position, targetPos, currentSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPlayer.position) < 0.7f)
        {
            bool isAdded = targetPlayer.GetComponent<Move>().inventory.GetItem(itemData);

            if (isAdded)
            {
                ResetItem();
                DropItemManager.Instance.ReturnToPool(this);
            }
            else
            {
                ResetItem();
                PopUp();
            }

        }
    }

    private void OnEnable()
    {
        collectable = false;
        Invoke("CanCollect", 1f);
    }

    private void CanCollect()
    {
        collectable = true;
    }

    private void ResetItem()
    {
        isFollowing = false;


        if (rb != null) rb.isKinematic = false;


        if (col != null) col.enabled = true;
    }

    public void PopUp()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        float jumpForce = 5f;
        float spread = Random.Range(-1.5f, 1.5f);
        Vector3 force = new Vector3(spread, jumpForce, 0);

        rb.AddForce(force, ForceMode.Impulse);

    }
}
