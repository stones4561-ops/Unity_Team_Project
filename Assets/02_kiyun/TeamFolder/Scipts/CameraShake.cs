using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour
{
    // 흔들림을 원할 때 외부에서 호출할 함수
    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // 랜덤한 위치를 설정 (magnitude가 클수록 더 크게 흔들림)
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-3f, 3f) * magnitude;

            transform.localPosition = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 흔들림 종료 후 원래 위치로 복귀
        transform.localPosition = originalPos;
    }
}
