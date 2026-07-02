using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private static PlayerInventory instance;
    public static PlayerInventory Instance
    {  get { return instance; } }


    public int slotCount;
    
    public Inventory playerInven;
    public InventoryUI playerInvenUI;

    

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        playerInven.InitInventory();
        playerInvenUI.InitInventoryUI();
    }

}
