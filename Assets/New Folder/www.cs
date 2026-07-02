using UnityEngine;

public class www : MonoBehaviour
{
    public Transform target;       // 따라갈 캐릭터의 Transform
    public float smoothSpeed = 0.125f; // 카메라가 따라가는 부드러운 속도 (0에 가까울수록 느림)
    public Vector3 offset;

    [Header("카메라 이동 제한 벽 설정")]
    public float minX;             // 화면 왼쪽 끝 제한 (최소 X값)
    public float maxX;             // 화면 오른쪽 끝 제한 (최대 X값)

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        // 1. 카메라가 이동해야 할 목표 위치 계산
        Vector3 desiredPosition = target.position + offset;

        // 2. Mathf.Clamp를 사용해 X축 이동 범위를 minX와 maxX 사이로 제한
        float clampedX = Mathf.Clamp(desiredPosition.x, minX, maxX);

        // 3. 제한된 X값을 적용한 새로운 목표 위치 설정 (Y축과 Z축은 그대로 유지)
        Vector3 clampedPosition = new Vector3(clampedX, desiredPosition.y, desiredPosition.z);

        // 4. Vector3.Lerp를 이용해 부드럽게 카메라 이동
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, clampedPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}
