using UnityEngine;

public class DummyReceiver : MonoBehaviour
{
    // 애니메이션 마커가 소리를 내라고 찾아와도, 
    // 이펙트에서는 소리가 겹치지 않게 아무 일도 안 하고 조용히 돌려보냅니다.

    public void PlayFootstep() { }               // 발소리 방어
    public void PlayFootstep(int index) { }      // 배열 발소리 방어
    public void PlaySwingSound(int index) { }    // 기본 공격 방어
    public void PlayDashSound() { }              // 대시 방어
    public void PlayUltSound(int index) { }      // ✨ 필살기 방어 (이번 에러의 범인!)
    public void PlayUltSound() { }
    public void PlayerUltimateSound() { }
    public void PlayerUltimateSound(int index) { }// (혹시 몰라 괄호 빈 것도 방어!)
    public void PlayerAttackSound() { }
    public void PlayerAttackSound(int index) { }
}