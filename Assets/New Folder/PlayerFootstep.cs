using UnityEngine;

public class PlayerFootstep : MonoBehaviour
{
    [Header("발소리 오디오 클립들")]
    public AudioClip[] footstepClips;

    public void PlayFootstep()
    {
        // 1. 코드가 정상적으로 호출되는지 확인
        Debug.Log("👟 [1단계] 애니메이션 이벤트가 코드를 호출했습니다!");

        // 2. 씬에 소리를 듣는 '귀(Listener)'가 있는지 코드 차원에서 강제 검사
        AudioListener listener = Object.FindFirstObjectByType<AudioListener>();
        if (listener == null)
        {
            Debug.LogError("🚨 [오류] 맵에 소리를 듣는 'Audio Listener'가 없습니다! 카메라를 확인하세요.");
            return;
        }

        // 3. 누군가 유니티 전체를 음소거 해뒀다면 강제로 최대 볼륨으로 켭니다.
        AudioListener.volume = 1f;
        AudioListener.pause = false;

        // 4. 캐릭터 설정 다 무시하고 2D 스피커를 즉석에서 만들어 소리를 때려 넣습니다.
        if (footstepClips != null && footstepClips.Length > 0)
        {
            AudioClip clipToPlay = footstepClips[Random.Range(0, footstepClips.Length)];

            // 임시 스피커 생성
            GameObject tempSpeaker = new GameObject("ForceFootstepSpeaker");
            AudioSource audio = tempSpeaker.AddComponent<AudioSource>();

            // 어떤 환경이든 무조건 들리게 세팅
            audio.spatialBlend = 0f; // 3D 거리 완전 무시 (100% 2D)
            audio.volume = 1f;       // 볼륨 최대
            audio.playOnAwake = false;
            audio.clip = clipToPlay;

            // 재생!
            audio.Play();

            Debug.Log($"🔊 [2단계] '{clipToPlay.name}' 사운드를 강제 재생했습니다!");

            // 소리 길이가 끝나면 임시 스피커 삭제
            Destroy(tempSpeaker, clipToPlay.length + 0.1f);
        }
        else
        {
            Debug.LogError("🚨 [오류] 스크립트 컴포넌트에 발소리 파일이 비어있습니다!");
        }
    }
}