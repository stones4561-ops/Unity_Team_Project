using UnityEngine;

public class Player : MonoBehaviour,IDamageable
{

    public Animator anim;

    private int hp = 2;
    public int Hp { get; private set; } = 2;

    private int att;
    public int Att { get; private set; }

    private int def;
    public int Def { get; private set; }

    private int speed;
    public int Speed { get; private set; }


    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void TakeDamage(int _damage)
    {
        Hp -= _damage;
        if (Hp <= 0)
        {
            anim.SetTrigger("Die");
        }
    }
}
