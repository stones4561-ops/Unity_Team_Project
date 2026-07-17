using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform[] destinations;
    public KeyCode interactKey = KeyCode.Space;
    public float yOffset = 1.0f;

    private bool isPlayerNear = false;
    private GameObject player;
    private int currentTargetIndex = 0;

    // 💡 1. 플레이어의 콜라이더 개수를 세어줄 변수 추가
    private int playerColliderCount = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerColliderCount++; // 콜라이더가 들어올 때마다 +1
            isPlayerNear = true;

            // attachedRigidbody를 쓰면 자식 콜라이더가 닿아도 최상위 플레이어 오브젝트를 정확히 가져옵니다.
            player = other.attachedRigidbody != null ? other.attachedRigidbody.gameObject : other.gameObject;

            Debug.Log($"🟢 포탈 안으로 들어왔습니다! (현재 닿은 콜라이더 수: {playerColliderCount})");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerColliderCount--; // 콜라이더가 나갈 때마다 -1

            // 💡 2. 닿아있는 콜라이더가 '0'이 되었을 때만 진짜로 나간 것으로 판정!
            if (playerColliderCount <= 0)
            {
                playerColliderCount = 0; // 안전장치
                isPlayerNear = false;
                player = null;
                Debug.Log("🔴 포탈 밖으로 완전히 나갔습니다!");
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(interactKey))
        {
            if (!isPlayerNear || player == null)
            {
                Debug.Log("❌ 스페이스바를 눌렀지만, 캐릭터가 포탈 판정 바깥에 있습니다!");
                return;
            }

            if (destinations == null || destinations.Length == 0)
            {
                Debug.Log("❌ 목적지(Destinations)가 비어있습니다!");
                return;
            }

            Transform target = destinations[currentTargetIndex];
            if (target == null)
            {
                Debug.Log("❌ 연결된 목적지 오브젝트가 없습니다!");
                return;
            }

            // 진짜 텔레포트 실행
            Vector3 newPosition = target.position;
            newPosition.z = player.transform.position.z;
            newPosition.y += yOffset;

            // 💡 3. Rigidbody가 있다면 transform 대신 rb.position을 직접 옮겨서 물리 엔진 오작동 방지
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.position = newPosition;
                rb.linearVelocity = Vector3.zero;
            }
            else
            {
                player.transform.position = newPosition;
            }

            Debug.Log($"🚀 텔레포트 성공! 새 위치: {newPosition}");

            currentTargetIndex++;
            if (currentTargetIndex >= destinations.Length)
            {
                currentTargetIndex = 0;
            }
        }
    }
}