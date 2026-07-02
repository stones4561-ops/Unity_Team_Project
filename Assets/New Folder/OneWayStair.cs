using UnityEngine;

public class OneWayStair : MonoBehaviour
{
    private Collider stairCollider;

    void Start()
    {
        stairCollider = GetComponent<Collider>();
    }

    void Update()
    {
        float moveInput = Input.GetAxisRaw("Horizontal");

        // 1. 왼쪽에서 오른쪽으로 갈 때 (→) : 계단을 밟고 올라가야 함
        if (moveInput > 0)
        {
            stairCollider.enabled = true;  // 콜라이더 켜기 (단단해짐)
        }
        // 2. 오른쪽에서 왼쪽으로 갈 때 (←) : 계단을 쑥 통과해야 함
        else if (moveInput < 0)
        {
            stairCollider.enabled = false; // 콜라이더 끄기 (유령 상태)
        }
    }
}