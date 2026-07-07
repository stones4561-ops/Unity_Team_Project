using UnityEngine;

public class RabbitPatrol : MonoBehaviour
{
    [SerializeField]
    private float minActionTime;
    [SerializeField]
    private float maxActionTime;


    private MonsterBase mBase;
    private Rigidbody rb;
    private Animator anim;


    private float moveSpeed;
    private float actionTimer;
    private int moveDirection = 0;







    private void Awake()
    {
        mBase = GetComponent<MonsterBase>();
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        ChooseNextAction();
        moveSpeed = mBase.MonsterSpeed;

    }

    private void Update()
    {
        if (mBase.IsDead) return;
        actionTimer -= Time.deltaTime;

        if (actionTimer <= 0)
        {
            ChooseNextAction();
        }

        Vector3 frontVec = new Vector3(transform.position.x + moveDirection * 0.2f, transform.position.y+0.1f, transform.position.z);


        bool isGrounded = Physics.Raycast(frontVec, Vector3.down, out RaycastHit hit, transform.localScale.y / 2 + 0.2f, LayerMask.GetMask("Ground"));
        

        if (!isGrounded && moveDirection != 0)
        {
            moveDirection = 0;
            anim.SetBool("isWalking", false);
        }

        rb.linearVelocity = new Vector3(moveDirection * moveSpeed, rb.linearVelocity.y, rb.linearVelocity.z);

        

    }



    private void ChooseNextAction()
    {

        int randomAction = Random.Range(0, 3);

        if (randomAction == 0)
        {
            moveDirection = 0;
            anim.SetBool("isWalking", false);
        }
        else if (randomAction == 1)
        {
            moveDirection = -1;
            transform.rotation = Quaternion.LookRotation(Vector3.left);
            anim.SetBool("isWalking", true);
        }
        else if (randomAction == 2)
        {
            moveDirection = 1;
            transform.rotation = Quaternion.LookRotation(Vector3.right);
            anim.SetBool("isWalking", true);
        }

        actionTimer = Random.Range(minActionTime, maxActionTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 frontVec = new Vector3(transform.position.x + moveDirection * 0.2f, transform.position.y+0.1f, transform.position.z);
        Gizmos.DrawRay(frontVec, Vector3.down * (transform.localScale.y / 2 + 0.2f));
    }
}
