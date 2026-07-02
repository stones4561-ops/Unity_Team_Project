using UnityEngine;

public class Portal : MonoBehaviour
{
    public Transform destination;
    public KeyCode interactKey = KeyCode.UpArrow;
    public float yOffset = 1.0f;

    private bool isPlayerNear = false;
    private GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            player = other.gameObject;
            Debug.Log("포탈 근처 도착! 설정된 키를 누르세요.");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            player = null;
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(interactKey))
        {
            // 🌟 1. 목적지의 X, Y 좌표만 가져오고, Z 좌표는 캐릭터의 원래 Z 좌표를 그대로 씁니다!
            Vector3 newPosition = destination.position;
            newPosition.z = player.transform.position.z; // Z축 고정 (추락 방지 핵심)
            newPosition.y += yOffset;                    // Y축 위로 살짝 띄우기

            // 2. 계산된 안전한 위치로 캐릭터 이동
            player.transform.position = newPosition;

            // 3. 가속도(관성) 초기화
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
            }

            Debug.Log("안전한 순간이동 완료!");
        }
    }
}