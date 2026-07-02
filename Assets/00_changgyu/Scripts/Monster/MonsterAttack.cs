using System.Collections;
using UnityEngine;

public class MonsterAttack : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    [SerializeField]
    float attackRange = 1.5f;
    private Animator anim;
    private bool isAttacking = false;


    private void Start()
    {
        anim=GetComponent<Animator>();
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);

        if(distance<=attackRange&&!isAttacking)
        {
            StartCoroutine(AttackRoutine());
        }
    }


    private IEnumerator AttackRoutine()
    {
        isAttacking = true;
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(1.0f);
        isAttacking=false;
    }

    public void DealDamage()
    {
        if(Vector3.Distance(transform.position, player.position)<=attackRange)
        {
            player.GetComponent<IDamageable>().TakeDamage(3);
        }
    }

}
