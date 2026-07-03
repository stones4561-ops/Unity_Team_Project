using UnityEditor.Build;
using UnityEngine;

public class Player : MonoBehaviour,IDamageable
{
    private static Player instance;
    public static Player Instance { get { return instance; } }

    public Animator anim;

    [SerializeField]
    private int hp;
    [SerializeField]
    private int maxHp; // Inspector에서 설정 가능하도록 public

    public int Hp
    {
        get => hp;
        private set => hp = Mathf.Clamp(value, 0, maxHp); // 체력은 0~maxHp 사이 유지
    }
    [SerializeField]
    private int att; // 실제 값 저장
    public int Att
    {
        get => att;
        private set => att = value;
    }
    [SerializeField]
    private int def;
    public int Def
    {
        get => def;
        private set => def = value;
    }
    [SerializeField]
    private int speed;
    public int Speed
    {
        get => speed;
        private set => speed = value;
    }

    private bool isInvincible = false; // 무적 상태 변수
    public bool IsInvincible
    {
        get => isInvincible;
        set => isInvincible = value;
    }

    [SerializeField]
    private bool die;
    public bool Die
    {
        get => die;
        private set => die = value;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        Hp = maxHp;
        Die = false;
        anim = GetComponent<Animator>();
    }

    
    public void TakeDamage(int _damage)
    {
        if (Die || isInvincible) return;
        Hp -= _damage;

        float damagePercentage = (float)_damage / maxHp;

        if (damagePercentage >= 0.03f) // 3% 이상일 때
        {
            anim.SetTrigger("Hit");
        }
        else // 3% 미만일 때
        {
            anim.SetTrigger("Hit2");
        }
        if (Hp <= 0)
        {
            anim.SetTrigger("Die");
            //Die = true;
        }
    }

    // 클래스 내부에서 값을 설정할 때
    public void SetAttackPower(int value)
    {
        Att = value; // 또는 Att = value; 둘 다 가능
    }

    public void SetInvincible(bool value)
    {
        IsInvincible = value;
    }
}
