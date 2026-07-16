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

    [SerializeField]
    private Transform targetPlayer;

    public void SetPlayer(Transform _player)
    {
        targetPlayer = _player;
    }

    
   
    private float knockbackForce = 3f;

    private List<bool> dropBool = new List<bool>();

    private string monsterName;
    
    private int monsterHP;
    private int maxHP;
    private int monsterAtk;
    private float monsterDeathTime;
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

    private Coroutine hitCoroutine;
    

    private void Awake()
    {
        monsterName = myData.monsterName;
        maxHP = myData.monsterHP;
        monsterHP=maxHP;
        monsterAtk = myData.monsterAtk;
        monsterSpeed = myData.monsterSpeed;
        monsterDeathTime = myData.monsterDeathTime;
        rb = GetComponent<Rigidbody>();
    }

    //리스폰시 상태 초기화+드롭아이템 확률을 여기서 재설정
    private void OnEnable()
    {
        
        monsterHP = maxHP;
        hpFillBar.fillAmount = 1;
        IsDead = false;
        IsHit = false;
        

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
            if(hitCoroutine != null) StopCoroutine(hitCoroutine);
            StartCoroutine(Dead());
        }
        else
        {
            if (hitCoroutine != null) StopCoroutine(hitCoroutine);
            hitCoroutine=StartCoroutine(HitCorou());
        }

    }

    private IEnumerator Dead()
    {
        rb.linearVelocity = Vector3.zero;
        anim.SetTrigger("isDead");
        yield return new WaitForSeconds(monsterDeathTime);
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

        if(targetPlayer != null)
        {
            float directionX= targetPlayer.position.x-transform.position.x;

            if (directionX > 0)
                transform.rotation = Quaternion.LookRotation(Vector3.right);
            else if(directionX < 0)
                transform.rotation= Quaternion.LookRotation(Vector3.left);
                
        }

        anim.SetTrigger("isHit");

        rb.linearVelocity = Vector3.zero;

        Vector3 knockbackDir=-transform.forward;

        knockbackDir.y = 0;

        rb.AddForce(knockbackDir.normalized*knockbackForce,ForceMode.Impulse);

        yield return new WaitForSeconds(0.5f);

        rb.linearVelocity = Vector3.zero;

        IsHit = false;
    }

   


}
