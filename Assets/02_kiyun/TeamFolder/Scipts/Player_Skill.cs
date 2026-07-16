using UnityEngine;

public class Player_Skill : MonoBehaviour
{
    public Animator anim;
    public SwordCollider sword;

    // 애니메이션 이벤트가 호출할 함수
    public void OnAttackAnimationFinished()
    {
        Debug.Log("애니메이션 끝! 공격 상태 초기화");
        sword.DisableAttack();
    }

    public void OnAttackAimationStart()
    {
        Debug.Log("애니메이션 시작");
        sword.OnAttack();
    }
}
