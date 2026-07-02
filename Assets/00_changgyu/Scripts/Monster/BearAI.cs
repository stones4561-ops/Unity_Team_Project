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


    protected override IEnumerator AttackRoutine()
    {
        isAttacking = true;

        rb.linearVelocity= new Vector3(0,rb.linearVelocity.y,0);
        anim.SetFloat("Speed", 0f);


        int randomAttack = Random.Range(0, 3);
        float currentWaitTime = 0f;

        switch(randomAttack)
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

        isAttacking=false;
    }

    public void FootDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackRange)
        {
            player.GetComponent<IDamageable>()?.TakeDamage(mBase.MonsterAtk);
        }
    }

    public void BiteDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackBiteRange)
        {
            player.GetComponent<IDamageable>()?.TakeDamage(mBase.MonsterAtk);
        }
    }

    public void StompDamage()
    {
        if (Vector3.Distance(transform.position, player.position) <= attackStompRange)
        {
            player.GetComponent<IDamageable>()?.TakeDamage(mBase.MonsterAtk);
        }
    }
}
