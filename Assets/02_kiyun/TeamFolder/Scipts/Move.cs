using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Move : MonoBehaviour
{
    private static Move instance;
    public static Move Instance { get { return instance; } }

    public SwordCollider sword;

    [Header("Movement Settings")]
    private float horizontalInput;
    public float runSpeed = 7f;
    public float skillMoveSpeed = 2f;
    public float jumpForce = 5f;
    public float minX;             // 화면 왼쪽 끝 제한 (최소 X값)
    public float maxX;             // 화면 오른쪽 끝 제한 (최대 X값)

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
    public float dashCoolTime = 3f;
    private bool isDashing = false;
    private bool canDash = true;
    [SerializeField]
    private Image dashSkill_Image;

    public bool c_key_Skill;
    public float c_key_SkillCoolTime = 20f;
    public Image c_key_Skill_Image;
    public GameObject skillPF;

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
        horizontalInput = Input.GetAxis("Horizontal");
        // 1. 대쉬 중엔 이동, 점프, 공격 로직을 아예 실행하지 않음
        if (isDashing || Player.Instance.IsUsingSkill) return;

        // 2. 이동 로직 (Rigidbody 사용)
        if (!isAttacking && !Player.Instance.Die)
            HandleMovement();

        // 3. 레이캐스트 및 점프/공격 입력
        int groundLayer = LayerMask.GetMask("Ground");
        isGrounded = Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, 0.15f, groundLayer);
        if (isGrounded) { anim.SetBool("Jump", false); anim.SetBool("Down Attack", false); }

        if ((Input.GetKeyDown(KeyCode.UpArrow)) && isGrounded && canJump)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
            anim.SetBool("Jump", true);
            StartCoroutine(JumpCooldown());
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
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
            {
                anim.SetBool("Down Attack", true);
            }
                
        }

        if (Input.GetKeyDown(KeyCode.X) && !isDashing && canDash)
        {
            StartCoroutine(DashRoutine());
            StartCoroutine(DashCooldownRoutine());
        }
        if (Input.GetKeyDown(KeyCode.C) && !c_key_Skill)
        {
            //Player.Instance.IsUsingSkill = true;
            c_key_Skill = true;
            SpawnSkillTr();
            //StartCoroutine(SpecialAttackRoutine(4.4f));
            StartCoroutine(CooldownRoutine(c_key_SkillCoolTime));

            //anim.SetTrigger("P");
        }
        if(Input.GetKeyDown(KeyCode.V))
        {
            anim.SetTrigger("P");
        }
    }
    //걷기 삭제 후 달리기만 가능하도록 수정
    void HandleMovement()
    {
        // 공격 중이면 이동 속도를 0으로, 아니면 원래 속도로
        float currentSpeed = isAttacking ? 0f : runSpeed;

        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            // 공격 중이 아니거나, 아주 살짝만 움직이게 하고 싶다면 currentSpeed를 조절
            anim.SetFloat("Blend", 1f);

            Vector3 moveDirection = new Vector3(horizontalInput, 0, 0);
            Vector3 targetPosition = rb.position + (moveDirection * currentSpeed * Time.deltaTime);

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            rb.MovePosition(targetPosition);

            // 공격 중에는 회전하지 않게 막을 수도 있습니다.
            if (!isAttacking)
            {
                transform.rotation = Quaternion.Euler(0f, horizontalInput > 0 ? 90f : 270f, 0f);
            }
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
            Vector3 nextPos = rb.position + (dashDirection * currentDashSpeed * Time.deltaTime);
            nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
            rb.MovePosition(nextPos);

            Player.Instance.SetInvincible(true);
            elapsed += Time.deltaTime;
            yield return null;
        }

        Player.Instance.SetInvincible(false);
        rb.useGravity = originalGravity;
        isDashing = false;

        // 동작 끝난 후 잠시 대기할 필요가 있다면 여기서 처리
        //yield return new WaitForSeconds(0.1f);
    }

    IEnumerator DashCooldownRoutine()
    {
        dashSkill_Image.fillAmount = 0f;
        float elapsed = 0f;

        while (elapsed < dashCoolTime)
        {
            elapsed += Time.deltaTime;
            dashSkill_Image.fillAmount = elapsed / dashCoolTime;
            yield return null;
        }

        dashSkill_Image.fillAmount = 1f;
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
    // 1. 공격 자체를 수행하는 코루틴
    IEnumerator SpecialAttackRoutine(float duration)
    {
        isAttacking = true;
        Player.Instance.SetInvincible(true);
        anim.SetTrigger("P");

        // 2. 공격 동작만 수행하고 즉시 종료
        yield return new WaitForSeconds(duration);

        isAttacking = false;
        Player.Instance.SetInvincible(false);
        Player.Instance.IsUsingSkill = false;

        Debug.Log("특수 공격 동작 종료");
    }

    // 4. 쿨타임만 전담하는 별도의 코루틴
    IEnumerator CooldownRoutine(float coolTime)
    {
        c_key_Skill_Image.fillAmount = 0f;
        float elapsed = 0f;

        while (elapsed < coolTime)
        {
            elapsed += Time.deltaTime;
            c_key_Skill_Image.fillAmount = elapsed / coolTime;
            yield return null;
        }

        c_key_Skill_Image.fillAmount = 1f;
        c_key_Skill = false; // 쿨타임 끝! 다시 사용 가능
        Debug.Log("쿨타임 종료");
    }

    public void ResetCombo()
    {
        Debug.Log("콤보 리셋");
        isAttacking = false;
        canCombo = false;
        nextInputReady = false;
    }

    public void SpawnSkillTr()
    {
        // 1. 오브젝트 생성
        GameObject obj = Instantiate(skillPF, this.transform.position, this.transform.rotation);
        // 3. 4.3초 뒤 삭제
        Destroy(obj, 4.3f);
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

    #region 검 콜라이더 관련 코드
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

    public void StartHitAimation()
    {
        sword.DisableAttack();
    }
    #endregion
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