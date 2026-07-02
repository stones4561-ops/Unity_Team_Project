using System.Collections;
using UnityEngine;

public class MonsterBase : MonoBehaviour,IDamageable
{
    [SerializeField]
    private MonsterSO myData;
    [SerializeField]
    private Animator anim;


    private string monsterName;
    
    private int monsterHP;
    private int monsterAtk;
    public int MonsterAtk
    { get { return monsterAtk;} }
    private float monsterSpeed;
    public float MonsterSpeed
    { get { return monsterSpeed; } }

    private Rigidbody rb;

    public bool IsHit
    { get; set; } = false;
    public bool IsDead
    { get; set; } = false;
    

    

    private void Awake()
    {
        monsterName = myData.monsterName;
        monsterHP = myData.monsterHP;
        monsterAtk = myData.monsterAtk;
        monsterSpeed = myData.monsterSpeed;
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int _damage)
    {
        if (IsDead) return;

        monsterHP -= _damage;

        if (monsterHP <= 0)
        {
            IsDead = true;
            StartCoroutine(Dead());
        }
        else
        {
            StartCoroutine(HitCorou());
        }



    }

    private IEnumerator Dead()
    {
        anim.SetBool("isDead", true);
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    
    private IEnumerator HitCorou()
    {
        IsHit = true;
        anim.SetTrigger("isHit");

        rb.linearVelocity = Vector3.zero;

        yield return new WaitForSeconds(0.5f);

        IsHit = false;
    }
    
   
}
