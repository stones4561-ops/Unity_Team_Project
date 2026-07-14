using UnityEngine;

public class PlayerDashSound : MonoBehaviour
{
    [Header("대시(회피) 소리 파일")]
    public AudioClip dashSound;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    // 애니메이션 마커에 연결할 함수 이름은 'PlayDashSound' 입니다!
    public void PlayDashSound()
    {
        if (dashSound != null)
        {
            audioSource.PlayOneShot(dashSound);
        }
    }
}