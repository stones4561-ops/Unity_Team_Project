using UnityEngine;
using System.Collections.Generic;

public class TempPlayer : MonoBehaviour,IDamageable
{
    private int hp = 10;
    private float moveSpeed = 5f;
    private Rigidbody rb;
    private List<IDamageable> enemies = new List<IDamageable>();
    

    public void TakeDamage(int _damage)
    {
        hp-=_damage;
        Debug.Log("아야,현재체력:"+hp);
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        float moveX=Input.GetAxisRaw("Horizontal");

        rb.linearVelocity = new Vector3(moveX * moveSpeed, rb.linearVelocity.y, 0);

        if(Input.GetKeyDown(KeyCode.Z))
        {
            Attack();
        }
    }

    private void Attack()
    {
        
        foreach (IDamageable enemy in enemies)
            enemy.TakeDamage(1);
    }

    private void OnTriggerStay(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if(damageable != null&&!enemies.Contains(damageable))
        {
            enemies.Add(damageable);
        }
    }

}
