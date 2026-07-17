using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform[] destinations;
    public KeyCode interactKey = KeyCode.Space;
    public float yOffset = 1.0f;

    private bool isPlayerNear = false;
    private GameObject player;
    private int currentTargetIndex = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            player = other.gameObject;
            Debug.Log("🟢 포탈 안으로 들어왔습니다!");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            player = null;
            Debug.Log("🔴 포탈 밖으로 나갔습니다!");
        }
    }

    void Update()
    {
        // 스페이스바를 누르는 순간 작동하는 CCTV
        if (Input.GetKeyDown(interactKey))
        {
            if (!isPlayerNear)
            {
                Debug.Log("❌ 스페이스바를 눌렀지만, 캐릭터가 포탈 판정(Collider) 바깥에 있습니다!");
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
            player.transform.position = newPosition;

            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
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