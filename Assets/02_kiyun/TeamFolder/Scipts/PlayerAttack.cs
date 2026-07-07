using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    // 애니메이션 이벤트가 호출할 함수
    public void OnAttackAnimationFinished()
    {
        Debug.Log("애니메이션 끝! 공격 상태 초기화");

        // 콤보 로직 초기화
        Move.Instance.ResetCombo();

    }
}
