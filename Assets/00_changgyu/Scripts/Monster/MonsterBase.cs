using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MonsterBase : MonoBehaviour,IDamageable
{
    [SerializeField]
    private MonsterSO myData;
    [SerializeField]
    private Animator anim;

    [SerializeField]
    private Image hpFillBar;


    private string monsterName;
    
    private int monsterHP;
    private int maxHP;
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
        maxHP = myData.monsterHP;
        monsterHP=maxHP;
        monsterAtk = myData.monsterAtk;
        monsterSpeed = myData.monsterSpeed;
        rb = GetComponent<Rigidbody>();
    }

    public void TakeDamage(int _damage)
    {
        if (IsDead) return;

        monsterHP -= _damage;

        if(hpFillBar != null)
        {
            hpFillBar.fillAmount = (float)monsterHP/maxHP;
        }

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
        rb.linearVelocity = Vector3.zero;
        anim.SetTrigger("isDead");
        yield return new WaitForSeconds(4f);
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
