using System.Collections;
using UnityEngine;

public class WolfAI : ChaseBase
{
    // 🎯 1. 늑대 공격 시작 조건 (완전 심플한 횡스크롤 좌우 판별)
    protected override bool CheckCustomAttackCondition()
    {
        

        // 늑대가 오른쪽을 보고 있는지? (transform.forward.x가 양수면 오른쪽)
        bool isFacingRight = transform.forward.x > 0;

        // 플레이어가 늑대보다 오른쪽에 있는지?
        bool isPlayerOnRight = player.position.x > transform.position.x;

        // 💡 핵심: 늑대가 바라보는 방향과 플레이어의 위치(좌/우)가 일치해야만 "정면"으로 인정!
        return isFacingRight == isPlayerOnRight;
    }

    // --- [공격 로직] ---
    protected override IEnumerator AttackRoutine()
    {
        isAttacking = true;

        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        anim.SetFloat("Speed", 0f);
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(2.5f);

        isAttacking = false;
    }

    // 🎯 2. 데미지 판정 함수 (여기도 좌우 판별 적용)
    public void DealDamage()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        bool isFacingRight = transform.forward.x > 0;
        bool isPlayerOnRight = player.position.x > transform.position.x;

        // 사거리 이내 && 플레이어가 내 정면(같은 방향)에 있을 때만 피격
        if (distance <= attackRange && (isFacingRight == isPlayerOnRight))
        {
            player.GetComponent<IDamageable>()?.TakeDamage(mBase.MonsterAtk);
        }
    }

    // 🎯 3. 기즈모 (정확히 늑대의 앞쪽 절반(반원) 영역만 그리도록 수정)
    private void OnDrawGizmosSelected()
    {
        if (mBase == null) return;

        Gizmos.color = Color.yellow;
        Draw2DHalfCircle(transform.position, transform.forward.x > 0, attackDetectedRange);

        Gizmos.color = Color.red;
        Draw2DHalfCircle(transform.position, transform.forward.x > 0, attackRange);
    }

    protected override void CancelAttackAnim()
    {
        if (anim != null)
        {
            anim.ResetTrigger("Attack");
        }
    }


    /// <summary>
    /// 횡스크롤에 맞춰 앞쪽 반원(180도) 영역을 그리는 기즈모
    /// </summary>
    private void Draw2DHalfCircle(Vector3 position, bool isFacingRight, float radius)
    {
        int segments = 20;
        // 0도는 머리 위(Up), 오른쪽을 보면 180도까지 뻗고, 왼쪽을 보면 -180도까지 뻗습니다.
        float startAngle = 0f;
        float endAngle = isFacingRight ? 180f : -180f;

        Vector3 previousPoint = position + new Vector3(0, radius, 0);

        for (int i = 1; i <= segments; i++)
        {
            float progress = (float)i / segments;
            float currentAngle = Mathf.Lerp(startAngle, endAngle, progress);

            float rad = currentAngle * Mathf.Deg2Rad;
            // 삼각함수를 이용해 X, Y 좌표 계산
            Vector3 currentDir = new Vector3(Mathf.Sin(rad), Mathf.Cos(rad), 0);
            Vector3 currentPoint = position + currentDir * radius;

            Gizmos.DrawLine(previousPoint, currentPoint);
            previousPoint = currentPoint;
        }

        // 반원을 닫는 직선 (위아래 연결)
        Gizmos.DrawLine(position + new Vector3(0, radius, 0), position + new Vector3(0, -radius, 0));
    }
}