using System.Collections;
using System.Collections.Generic;
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

    [SerializeField]
    private DropTableSO dropTableSO;

    private List<bool> dropBool = new List<bool>();

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

    //리스폰시 상태 초기화+드롭아이템 확률을 여기서 재설정
    private void OnEnable()
    {
        
        monsterHP = maxHP;
        hpFillBar.fillAmount = 1;
        IsDead = false;
        dropBool.Clear();
        for(int i = 0; i < dropTableSO.dropChance.Length; i++)
        {
            float chance = Random.Range(0, 100f);
            if (chance <= dropTableSO.dropChance[i])
                dropBool.Add(true);
            else
                dropBool.Add(false);
        }
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
        for(int i=0;i<dropBool.Count;i++)
        {
            if(dropBool[i])
            {
                DropItemManager.Instance.SpawnItem(dropTableSO.dropItems[i],transform.position);
            }

        }
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
