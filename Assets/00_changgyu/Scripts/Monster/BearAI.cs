using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class BearAI : ChaseBase
{
    [Header("애니메이션 시간")]
    [SerializeField] private float attackFootTime;
    [SerializeField] private float attackBiteTime;
    [SerializeField] private float attackStompTime;

    [Header("공격범위")]
    [SerializeField] private float attackBiteRange;
    [SerializeField] private float attackStompRange;

    [Header("이동제한")]
    public float minX;
    public float maxX;

    private void Start()
    {
        chaseRange = 12f;
    }


    // 🎯 1. 곰 공격 시작 조건 통제 (횡스크롤 좌우 판별)
    protected override bool CheckCustomAttackCondition()
    {
        // 곰이 오른쪽을 보고 있는지? (transform.forward.x가 양수면 오른쪽)
        bool isFacingRight = transform.forward.x > 0;

        // 플레이어가 곰보다 오른쪽에 있는지?
        bool isPlayerOnRight = player.position.x > transform.position.x;

        // 곰이 바라보는 방향과 플레이어의 위치(좌/우)가 일치해야만 "정면"으로 인정!
        return isFacingRight == isPlayerOnRight;
    }

    private void LateUpdate()
    {
        // 현재 좌표 가져오기
        Vector3 currentPos = transform.position;

        // X 좌표를 minX와 maxX 사이의 값으로 강제 고정(Clamp)
        float clampedX = Mathf.Clamp(currentPos.x, minX, maxX);

        // 만약 곰이 제한 구역을 벗어나려 했다면?
        if (currentPos.x != clampedX)
        {
            // 고정된 X 좌표로 다시 위치를 덮어씌웁니다.
            transform.position = new Vector3(clampedX, currentPos.y, currentPos.z);

            // 투명 벽에 막혔으니 더 이상 밀고 나가지 않도록 속도도 0으로 만들어줍니다.
            if (rb != null)
            {
                rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
                ChooseNextAction();
            }
        }
    }
    protected override IEnumerator AttackRoutine()
    {
        isAttacking = true;

        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        anim.SetFloat("Speed", 0f);

        int randomAttack = Random.Range(0, 3);
        float currentWaitTime = 0f;

        switch (randomAttack)
        {
            case 0:
                anim.SetTrigger("attackFoot");
                currentWaitTime = attackFootTime;
                break;
            case 1:
                anim.SetTrigger("attackBite");
                currentWaitTime = attackBiteTime;
                break;
            case 2:
                anim.SetTrigger("attackStomp");
                currentWaitTime = attackStompTime;
                break;
        }
        yield return new WaitForSeconds(currentWaitTime);

        isAttacking = false;
    }

    // 🎯 2. 데미지 판정 함수들 (3개 모두 좌우 판별 적용)
    public void FootDamage()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool isFacingRight = transform.forward.x > 0;
        bool isPlayerOnRight = player.position.x > transform.position.x;

        if (distance <= attackRange && (isFacingRight == isPlayerOnRight))
        {
            player.GetComponent<IDamageable>()?.TakeDamage(mBase.MonsterAtk);
        }
    }

    public void BiteDamage()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool isFacingRight = transform.forward.x > 0;
        bool isPlayerOnRight = player.position.x > transform.position.x;

        if (distance <= attackBiteRange && (isFacingRight == isPlayerOnRight))
        {
            player.GetComponent<IDamageable>()?.TakeDamage(mBase.MonsterAtk);
        }
    }

    public void StompDamage()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        bool isFacingRight = transform.forward.x > 0;
        bool isPlayerOnRight = player.position.x > transform.position.x;

        if (distance <= attackStompRange && (isFacingRight == isPlayerOnRight))
        {
            player.GetComponent<IDamageable>()?.TakeDamage(mBase.MonsterAtk);
        }
    }

    protected override void CancelAttackAnim()
    {
        if (anim != null)
        {
            anim.ResetTrigger("attackFoot");
            anim.ResetTrigger("attackBite");
            anim.ResetTrigger("attackStomp");
        }
    }


    // 🎯 3. 기즈모 (곰의 공격 패턴 3가지 범위를 색상별로 표현)
    private void OnDrawGizmosSelected()
    {
        if (mBase == null) return;

        bool isFacingRight = transform.forward.x > 0;

        // 1. 할퀴기 범위 (기본 사거리 - 빨간색)
        Gizmos.color = Color.red;
        Draw2DHalfCircle(transform.position, isFacingRight, attackRange);

        // 2. 물기 범위 (주황색)
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Draw2DHalfCircle(transform.position, isFacingRight, attackBiteRange);

        // 3. 짓밟기 범위 (보라색)
        Gizmos.color = Color.magenta;
        Draw2DHalfCircle(transform.position, isFacingRight, attackStompRange);
    }



    /// <summary>
    /// 횡스크롤에 맞춰 앞쪽 반원(180도) 영역을 그리는 기즈모
    /// </summary>
    private void Draw2DHalfCircle(Vector3 position, bool isFacingRight, float radius)
    {
        int segments = 20;
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


