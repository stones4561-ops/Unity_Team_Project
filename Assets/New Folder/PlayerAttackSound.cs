using UnityEngine;

public class PlayerAttackSound : MonoBehaviour
{
    [Header("5연타 칼 휘두르는 소리들 (순서대로 0,1,2,3,4)")]
    // [] 기호를 붙이면 소리를 1개가 아니라 여러 개 담을 수 있는 '배열'이 됩니다.
    public AudioClip[] swingSounds;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.spatialBlend = 0f;
    }

    // 괄호 안에 (int index)를 추가했습니다! 애니메이션에서 숫자를 넘겨받을 겁니다.
    public void PlaySwingSound(int index)
    {
        // 넘겨받은 숫자가 0~4 사이이고, 그 자리에 소리가 들어있을 때만 재생!
        if (swingSounds != null && index >= 0 && index < swingSounds.Length)
        {
            if (swingSounds[index] != null)
            {
                audioSource.PlayOneShot(swingSounds[index]);
            }
        }
    }
}