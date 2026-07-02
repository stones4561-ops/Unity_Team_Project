using UnityEngine;

public class Move1 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody rb;
    private bool isGrounded = true;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. 좌우 이동
        float moveInput = Input.GetAxisRaw("Horizontal");
        transform.Translate(Vector3.right * moveInput * moveSpeed * Time.deltaTime);

        // 2. 점프 (바닥에 닿아있을 때만 가능)
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // 점프할 때 기존의 위아래 떨어지던 힘을 초기화해서 항상 일정한 높이로 뛰게 만듭니다.
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false; // 공중에 떴으므로 점프 불가 상태로 변경
        }
    }

    // 3. 🌟 스마트 착지 체크 (벽타기 방지 핵심 코드)
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            // 캐릭터가 부딪힌 지점의 '표면 방향(Normal)'을 가져옵니다.
            // y값이 0.5보다 크다는 것은, 벽(옆면)이 아니라 '위쪽을 향하는 평평한 바닥'이라는 뜻입니다!
            if (collision.contacts[0].normal.y > 0.5f)
            {
                isGrounded = true;
                Debug.Log("✅ 평평한 바닥 착지! 점프 가능");
            }
            else
            {
                Debug.Log("🚫 옆쪽 벽에 닿음! 점프 안됨");
            }
        }
    }
}