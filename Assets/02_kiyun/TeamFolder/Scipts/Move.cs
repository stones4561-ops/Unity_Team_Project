using System;
using System.Collections;
using UnityEngine;

public class Move : MonoBehaviour
{
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

    private bool isAttacking = false;
    private bool nextInputReady = false;
    private bool canCombo = false;

    [Header("Dash Settings")]
    public float dashSpeed = 5f;
    public float dashTime = 0.5f; // 너무 길면 조작이 답답하니 0.5초 정도로 권장
    public float dashCooldown = 3f;
    private bool isDashing = false;
    private bool canDash = true;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        
        // 물리 이동을 위해 필수 설정
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }

    void Update()
    {
        // 1. 대쉬 중엔 이동, 점프, 공격 로직을 아예 실행하지 않음
        if (isDashing) return;

        // 2. 이동 로직 (Rigidbody 사용)
        if(!isAttacking && !Player.Instance.Die)
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
            SwordCollider.Instance.EnableAttack(1f);
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
                anim.SetBool("Down Attack",true);
        }

        if (Input.GetKeyDown(KeyCode.X) && !isDashing && canDash)
        {
            SwordCollider.Instance.EnableAttack(1f);
            StartCoroutine(DashRoutine());
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StartCoroutine(SpecialAttackRoutine(7f));
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

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 3. 대쉬 종료: 충돌 무시 설정 복구
        Physics.IgnoreLayerCollision(playerLayer, monsterLayer, false);

        rb.useGravity = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        int comboCount = 1;
        while (comboCount < 5)
        {
            Debug.Log(comboCount + "타 시작");
            canCombo = true;
            nextInputReady = false;
            float timer = 0f;
            while (timer < comboDelay)
            {
                timer += Time.deltaTime;
                if (nextInputReady) break;
                yield return null;
            }
            canCombo = false;
            if (!nextInputReady) break;
            comboCount++;
        }
        ResetCombo();
    }

    IEnumerator JumpCooldown()
    {
        canJump = false;
        yield return new WaitForSeconds(2.3f);
        canJump = true;
    }

    IEnumerator SpecialAttackRoutine(float duration)
    {
        isAttacking = true; // 공격 상태 시작
        Player.Instance.SetInvincible(true);
        // 공격 설정 및 애니메이션 실행
        SwordCollider.Instance.EnableAttack(6f);
        anim.SetTrigger("P");

        // 6초 동안 대기
        yield return new WaitForSeconds(duration);

        isAttacking = false; // 공격 상태 종료
        Player.Instance.SetInvincible(false);
        Debug.Log("특수 공격 종료");
    }

    void ResetCombo()
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
 */