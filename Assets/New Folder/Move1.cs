using UnityEngine;

public class Move1 : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    private Rigidbody rb;

    // 🌟 레이저의 길이 (캐릭터의 키에 따라 이 숫자를 조절해 보세요!)
    // 캐릭터의 중심점(보통 배꼽이나 발밑)에서부터 쏘기 때문에, 
    // 처음엔 1.1 정도로 해두고 유니티 화면을 보며 늘리거나 줄이면 됩니다.
    public float rayLength = 1.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // 1. 좌우 이동 및 자물쇠 (비탈길 미끄럼 방지)
        float moveInput = Input.GetAxisRaw("Horizontal");

        if (moveInput == 0)
        {
            // 가만히 있을 때는 좌우(X)로 미끄러지지 않게 잠금!
            rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
        }
        else
        {
            // 움직일 때는 좌우(X) 자물쇠를 풀어줍니다.
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
            transform.Translate(Vector3.right * moveInput * moveSpeed * Time.deltaTime);
        }

        // 2. 🌟 궁극의 바닥 체크 (Raycast 레이저)
        // transform.position(캐릭터 위치)에서 Vector3.down(아래) 방향으로 레이저를 쏩니다.
        // 레이저가 'Ground' 태그나 콜라이더에 닿으면 true가 됩니다.
        bool isGrounded = Physics.Raycast(transform.position, Vector3.down, rayLength);

        // (꿀팁) 씬 화면에서 레이저를 붉은 선으로 직접 눈으로 볼 수 있게 해줍니다! 
        Debug.DrawRay(transform.position, Vector3.down * rayLength, Color.red);

        // 3. 점프
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            // 🌟 핵심: 점프하는 순간에는 무조건 묶여있던 좌우 자물쇠를 풀어주어야 자연스럽게 위로 튕겨 나갈 수 있습니다!
            rb.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}