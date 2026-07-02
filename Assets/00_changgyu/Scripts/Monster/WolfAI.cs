using System.Collections;
using UnityEngine;

public class WolfAI : ChaseBase
{
    // --- [공격 로직] ---
    protected override IEnumerator AttackRoutine()
    {
        isAttacking = true;

        // 공격할 때는 미끄러지지 않도록 즉시 정지
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        anim.SetFloat("Speed", 0f);

        anim.SetTrigger("Attack");

        // 공격 애니메이션이 완전히 끝날 때까지 대기 (애니메이션 길이에 맞춰 수정하세요)
        yield return new WaitForSeconds(2.5f);

        isAttacking = false;
    }

    // 애니메이션 이벤트에서 실행할 데미지 함수
    public void DealDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            player.GetComponent<IDamageable>()?.TakeDamage(mBase.MonsterAtk);
        }
    }


}