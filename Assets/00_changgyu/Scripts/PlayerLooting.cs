using UnityEngine;

public class PlayerLooting : MonoBehaviour
{
    //이 코드 사용법
    //1.플레이어의 자식으로 박스 콜라이더로 루팅 범위를 지정, 이즈 트리거 체크
    //2.그 루팅 범위에 이 코드를 붙임
    //3.드롭아이템의 수정을 진행하지 않았다면 플레이어에게 템프플레이어 부착


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
