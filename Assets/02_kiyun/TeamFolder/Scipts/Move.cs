using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    private static Move instance;
    public static Move Instance { get { return instance; } }

    [Header("Movement Settings")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float jumpForce = 5f;

    [Header("Attack Settings")]
    public float comboDelay = 0.3f;
    private bool isDownAttack = false;

    public Animator anim;
    private Rigidbody rb;
    private bool isGrounded;
    private bool canJump = true;
    private bool lastGrounded;

    private bool isAttacking = false;
    private bool nextInputReady = false;
    private bool canCombo = false;

    [Header("Dash Settings")]
    public float dashSpeed = 5f;
    public float dashTime = 0.5f; // 너무 길면 조작이 답답하니 0.5초 정도로 권장
    public float dashCooldown = 3f;
    private bool isDashing = false;
    private bool canDash = true;
    [SerializeField]
    private Image DashSkill;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        // 물리 이동을 위해 필수 설정
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // 1. 대쉬 중엔 이동, 점프, 공격 로직을 아예 실행하지 않음
        if (isDashing || Player.Instance.IsUsingSkill) return;

        // 2. 이동 로직 (Rigidbody 사용)
        if (!isAttacking && !Player.Instance.Die)
            HandleMovement();

        // 3. 레이캐스트 및 점프/공격 입력
        int groundLayer = LayerMask.GetMask("Ground");
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.15f, groundLayer);
        if (isGrounded) { anim.SetBool("Jump", false); anim.SetBool("Down Attack", false); }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            anim.SetBool("Jump", true);
            StartCoroutine(JumpCooldown());
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            //SwordCollider.Instance.EnableAttack(1f);
            if (isGrounded && !isAttacking)
            {
                StartCoroutine(AttackRoutine());
                anim.SetTrigger("Attack");
            }
            else if (canCombo)
            {
                canCombo = false;
                nextInputReady = true;
                anim.SetTrigger("Attack");
            }
            if (!isGrounded)
                anim.SetBool("Down Attack", true);
        }

        if (Input.GetKeyDown(KeyCode.X) && !isDashing && canDash)
        {
            //SwordCollider.Instance.EnableAttack(1f);

            StartCoroutine(DashRoutine());
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Player.Instance.IsUsingSkill = true;
            StartCoroutine(SpecialAttackRoutine(4.4f));
            anim.SetTrigger("P");
        }

    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            bool isRunning = Input.GetKey(KeyCode.LeftShift);
            anim.SetFloat("Blend", isRunning ? 1f : 0.7f);

            float speed = isRunning ? runSpeed : walkSpeed;
            Vector3 moveDirection = new Vector3(horizontalInput, 0, 0);

            // MovePosition을 사용해야 물리 엔진이 이동을 인지하고 충돌을 처리함
            rb.MovePosition(rb.position + moveDirection * speed * Time.deltaTime);

            // horizontalInput이 0보다 크면 90도, 아니면 270도로 회전
            transform.rotation = Quaternion.Euler(0f, horizontalInput > 0 ? 90f : 270f, 0f);
        }
        else
        {
            anim.SetFloat("Blend", 0f);
        }
    }

    IEnumerator DashRoutine()
    {
        isDashing = true;
        canDash = false;

        // 1. 대쉬 시작: 플레이어(몸체)와 몬스터 레이어 간의 충돌을 무시
        // 주의: 공격 판정(Sword)은 이 레이어와 다르게 설정해야 타격이 가능합니다.
        int playerLayer = LayerMask.NameToLayer("Player");
        int monsterLayer = LayerMask.NameToLayer("Monster");
        Physics.IgnoreLayerCollision(playerLayer, monsterLayer, true);

        bool originalGravity = rb.useGravity;
        rb.useGravity = false;

        anim.SetTrigger("Dash");

        Vector3 dashDirection = transform.forward;
        float currentDashSpeed = dashSpeed / dashTime;

        float elapsed = 0f;
        while (elapsed < dashTime)
        {
            // 2. 물리 이동 (Ground 레이어와는 충돌이 켜져 있으므로 벽에서 멈춤)
            Vector3 nextPos = rb.position + (dashDirection * currentDashSpeed * Time.deltaTime);
            rb.MovePosition(nextPos);
            Player.Instance.SetInvincible(true);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Player.Instance.SetInvincible(false);
        // 3. 대쉬 종료: 충돌 무시 설정 복구
        //Physics.IgnoreLayerCollision(playerLayer, monsterLayer, false);

        rb.useGravity = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);

        //if (DashSkill != null)
        //{
        //    float cooldownElapsed = 0f;
        //    while (cooldownElapsed < dashCooldown)
        //    {
        //        cooldownElapsed += Time.deltaTime;
        //        // fillAmount는 0~1 사이 값이므로, 
        //        // (경과시간 / 전체시간)을 1에서 빼서 줄어드는 효과를 만듭니다.
        //        DashSkill.fillAmount = cooldownElapsed / dashCooldown;
        //        yield return null;
        //    }
        //    DashSkill.fillAmount = 1f; // 쿨타임 종료 시 꽉 채움
        //}

        canDash = true;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        int comboCount = 0;

        // 첫 타는 즉시 실행
        anim.SetTrigger("Attack");

        while (comboCount < 6)
        {
            canCombo = true;
            nextInputReady = false;

            float timer = 0f;
            // 입력 가능 시간(콤보 딜레이) 동안 대기
            while (timer < comboDelay)
            {
                timer += Time.deltaTime;
                if (nextInputReady) break; // 입력이 들어오면 루프 탈출
                yield return null;
            }

            canCombo = false;

            // 대기 시간이 끝났는데 입력이 없었다면 콤보 종료
            if (!nextInputReady)
            {
                break;
            }

            // 입력이 있었다면 다음 타수 애니메이션 실행
            comboCount++;
            anim.SetTrigger("Attack");
            Debug.Log(comboCount + "타 연계 실행");
        }

        // 루프가 완전히 종료(입력 실패 혹은 5타 완료)되면 호출
        ResetCombo();
    }

    IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(2f);
        canJump = true;
    }
    //사용 중에 무적이 안되는 버그 발견 나중에 수정
    IEnumerator SpecialAttackRoutine(float duration)
    {
        isAttacking = true; // 공격 상태 시작
        Player.Instance.SetInvincible(true);
        // 공격 설정 및 애니메이션 실행
        //SwordCollider.Instance.EnableAttack(duration);
        anim.SetTrigger("P");

        // duration초 동안 대기
        yield return new WaitForSeconds(duration);

        isAttacking = false; // 공격 상태 종료
        Player.Instance.SetInvincible(false);
        Player.Instance.IsUsingSkill = false;
        Debug.Log("특수 공격 종료");
    }

    public void ResetCombo()
    {
        Debug.Log("콤보 리셋");
        isAttacking = false;
        canCombo = false;
        nextInputReady = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 태그 대신 레이어 확인 (더 안전함)
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            isGrounded = true;
        }
    }

    public void UseHitTrigger()
    {
        anim.SetTrigger("Hit");
    }

    public void UseHit2Trigger()
    {
        anim.SetTrigger("Hit2");
    }


    // 애니메이션 이벤트가 호출할 함수
    public void OnAttackAnimationFinished()
    {
        Debug.Log("애니메이션 끝! 공격 상태 초기화");
        SwordCollider.Instance.DisableAttack();
    }

    public void OnAttackAimationStart()
    {
        Debug.Log("애니메이션 시작");
        SwordCollider.Instance.OnAttack();
    }

    public void StartHitAimation()
    {
        SwordCollider.Instance.DisableAttack();
    }

    public void Knockback(Vector3 attackerPosition, float force)
    {
        // 공격자 반대 방향 계산
        Vector3 direction = (transform.position - attackerPosition).normalized;
        direction.y = 0.5f;

        // 넉백 수행 코루틴 시작
        StartCoroutine(KnockbackRoutine(direction, force));
    }

    IEnumerator KnockbackRoutine(Vector3 direction, float force)
    {
        float duration = 0.2f; // 넉백 지속 시간
        float elapsed = 0f;

        // 밀려나는 목표 지점 계산
        Vector3 startPos = rb.position;
        Vector3 targetPos = startPos + (direction * force);

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // MovePosition을 사용해 지정된 위치로 이동
            // 물리 엔진이 충돌을 감지하면서 부드럽게 이동시킴
            rb.MovePosition(Vector3.Lerp(startPos, targetPos, t));

            yield return null;
        }
    }
}

/*
 * 입력 시마다 예약 단계가 증가가 아니라 입력하고 해당 단계를 유지 -> 딜레이 시간 안에 입력하지 않으면 초기화
 * 
 * 각 단계가 끝나고 딜레이 시간안에 입력을 하지 않을 경우 입력 단계가 초기화된다
 * 각 단계 즉 애니메이션 모션이 끝날 때 쯤? 딜레이 시간 안에 입력을 하지 않을 경우 입력 단계가 초기화된다
 * 
 * 어택스텝 0 -> 1 딜레이 0.5초 안에 입력하지 않을 경우 0으로 해당 스텝에 z키를 입력했을 경우 어택 스탭은 1로 고정하고 입력했으니까 다음 스탭으로 넘어가야함
 * 어택스텝 1 -> 2 딜레이 0.5초 안에 입력하지 않을 경우 0으로 해당 스텝에 z키를 입력했을 경우 어택 스탭은 2로 고정하고 입력했으니까 다음 스탭으로 넘어가야함
 * 어택스텝 2 -> 3 딜레이 0.5초 안에 입력하지 않을 경우 0으로 해당 스텝에 z키를 입력했을 경우 어택 스탭은 3로 고정하고 입력했으니까 다음 스탭으로 넘어가야함
 * 어택스텝 3 -> 4 딜레이 0.5초 안에 입력하지 않을 경우 0으로 해당 스텝에 z키를 입력했을 경우 어택 스탭은 4로 고정하고 입력했으니까 다음 스탭으로 넘어가야함
 * 어택스텝 4 -> 5 딜레이 0.5초 안에 입력하지 않을 경우 0으로 해당 스텝에 z키를 입력했을 경우 어택 스탭은 5로 고정하고 입력했으니까 다음 스탭으로 넘어가야함
 * 어택스텝 5 애니메이션 종료 후 0으로
 * 
 * 콤보 시스템의 수정이 필요함
 */