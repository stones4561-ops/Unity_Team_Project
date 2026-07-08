using UnityEngine;

public class PlayerLooting : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {


        if (other.CompareTag("DropItem"))
        {
            DropItem item = other.GetComponent<DropItem>();
            if (item != null && item.Collectable && !item.IsFollowing)
            {
                if (PlayerInventory.Instance.playerInven.CanAddItem(item.ItemData))
                    item.StartFollowing(transform.parent);
            }
        }
    }
}
