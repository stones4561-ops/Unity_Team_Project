using UnityEngine;

public class ReturnToPool : MonoBehaviour
{
    void OnParticleSystemStopped()
    {
        // 파티클 재생이 끝나면 비활성화
        gameObject.SetActive(false);
    }
}