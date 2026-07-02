using System.Collections;
using UnityEngine;

public abstract class ChaseBase : MonoBehaviour
{
    [Header("추적 및 공격 설정")] 
    [SerializeField] protected Transform player;
    [SerializeField] private float chaseRange = 5f;
    [SerializeField] private float attackDetectedRange = 1.5f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] private float minActionTime = 1f;
    [SerializeField] private float maxActionTime = 3f;

    protected MonsterBase mBase;
    protected Rigidbody rb;
    protected Animator anim;

    private float moveSpeed;
    private float actionTimer;
    private int moveDirection = 0;
    protected bool isAttacking = false;

    [SerializeField]
    private float chaseTime;
    private float chaseTimer;
    private Vector3 lastPosition;

    [SerializeField]
    private float chaseCooldownTime;
    private float chaseCooldownTimer = 0f;


    private enum State { Patrol, Chase, Attack }
    private State currentState = State.Patrol;

    private void Awake()
    {
        mBase = GetComponent<MonsterBase>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        moveSpeed = mBase.MonsterSpeed;
        ChooseNextAction();
    }

    private void Update()
    {
        if (mBase.IsHit || mBase.IsDead) return;

        if (chaseCooldownTimer > 0)
        {
            chaseCooldownTimer -= Time.deltaTime;
        }

        // 플레이어와의 거리 계산
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // 1. 상태 판단 (공격 중이 아닐 때만 상태 변경)
        if (!isAttacking)
        {
            if (distanceToPlayer <= attackDetectedRange)
            {
                currentState = State.Attack; // 사거리 내: 공격
            }
            else if (distanceToPlayer <= chaseRange && chaseCooldownTimer <= 0)
            {
                if (currentState != State.Chase)
                {
                    chaseTimer = 2f;
                    lastPosition = transform.position;
                }
                currentState = State.Chase;  // 발견 거리 내: 추적
            }
            else
            {
                currentState = State.Patrol; // 그 외: 순찰
            }
        }

        // 2. 현재 상태에 따른 행동 실행
        switch (currentState)
        {
            case State.Patrol:
                UpdatePatrol();
                break;
            case State.Chase:
                UpdateChase(distanceToPlayer);
                ChaseEnd();
                break;
            case State.Attack:
                if (!isAttacking) StartCoroutine(AttackRoutine());
                break;
        }

    }

    // --- [순찰 로직] ---
    private void UpdatePatrol()
    {
        actionTimer -= Time.deltaTime;
        if (actionTimer <= 0)
        {
            ChooseNextAction();
        }



        Vector3 frontVec = new Vector3(transform.position.x + moveDirection * 0.2f, transform.position.y + 0.2f, transform.position.z);


        bool isGrounded = Physics.Raycast(frontVec, Vector3.down, out RaycastHit hit, transform.localScale.y / 2 + 0.2f, LayerMask.GetMask("Ground"));

        if (!isGrounded && moveDirection != 0)
        {
            moveDirection = 0;

        }

        rb.linearVelocity = new Vector3(moveDirection * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);
        // 걷기: 블렌드 트리의 Speed 파라미터에 1을 전달
        anim.SetFloat("Speed", moveDirection != 0 ? 1f : 0f);
    }

    private void ChooseNextAction()
    {
        int randomAction = Random.Range(0, 3);
        if (randomAction == 0)
        {
            moveDirection = 0;
        }
        else if (randomAction == 1)
        {
            moveDirection = -1;
            transform.rotation = Quaternion.LookRotation(Vector3.left);
        }
        else if (randomAction == 2)
        {
            moveDirection = 1;
            transform.rotation = Quaternion.LookRotation(Vector3.right);
        }
        actionTimer = Random.Range(minActionTime, maxActionTime);
    }

    // --- [추적 로직] ---
    private void UpdateChase(float distance)
    {
        // 플레이어가 있는 방향 계산
        Vector3 directionToPlayer = (player.position - transform.position).normalized;
        moveDirection = directionToPlayer.x > 0 ? 1 : -1;

        // 해당 방향 바라보기
        transform.rotation = Quaternion.LookRotation(moveDirection == 1 ? Vector3.right : Vector3.left);

        // 추적 시 이동 속도를 더 빠르게 적용 (예: 기본 속도의 1.5배)
        float runSpeed = moveSpeed * 4f;


        Vector3 frontVec = new Vector3(transform.position.x + moveDirection * 0.2f, transform.position.y + 0.2f, transform.position.z);


        bool isGrounded = Physics.Raycast(frontVec, Vector3.down, out RaycastHit hit, transform.localScale.y / 2 + 0.2f, LayerMask.GetMask("Ground"));

        if (!isGrounded && moveDirection != 0)
        {
            moveDirection = 0;

        }

        rb.linearVelocity = new Vector3(moveDirection * runSpeed, rb.linearVelocity.y, rb.linearVelocity.z);

        // 달리기: 블렌드 트리의 Speed 파라미터에 2를 전달
        anim.SetFloat("Speed", 2f);
    }

    private void ChaseEnd()
    {

        if (Vector3.Distance(lastPosition, transform.position) > 0.1f)
        {
            lastPosition = transform.position;
            chaseTimer = chaseTime;
        }
        else
        {
            chaseTimer -= Time.deltaTime;

            if (chaseTimer <= 0)
            {
                chaseCooldownTimer = chaseCooldownTime;
                currentState = State.Patrol;
                ChooseNextAction();

            }
        }



    }

    protected abstract IEnumerator AttackRoutine();
    

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 frontVec = new Vector3(transform.position.x + moveDirection * 0.2f, transform.position.y + 0.2f, transform.position.z);
        Gizmos.DrawRay(frontVec, Vector3.down * (transform.localScale.y / 2 + 0.2f));
    }
}
