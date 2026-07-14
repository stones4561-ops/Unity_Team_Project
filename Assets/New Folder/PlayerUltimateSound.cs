using UnityEngine;

public class PlayerUltimateSound : MonoBehaviour
{
    [Header("필살기 5연격 사운드")]
    public AudioClip[] ultSounds;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    // 마커에 연결할 함수 이름은 'PlayUltSound' 입니다!
    public void PlayUltSound(int index)
    {
        // 빈칸이 아니면서 숫자가 알맞게 들어왔을 때만 소리 재생
        if (ultSounds != null && index >= 0 && index < ultSounds.Length)
        {
            if (ultSounds[index] != null)
            {
                audioSource.PlayOneShot(ultSounds[index]);
            }
        }
    }
}