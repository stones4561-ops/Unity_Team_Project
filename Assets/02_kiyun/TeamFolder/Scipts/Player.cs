using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using JetBrains.Annotations;

public class Player : MonoBehaviour,IDamageable
{
    private static Player instance;
    public static Player Instance { get { return instance; } }

    public Animator anim;

    public CameraShake cameraShake;

    [SerializeField]
    private int hp;
    [SerializeField]
    private int maxHp; // Inspector에서 설정 가능하도록 public

    [SerializeField]
    private Image hpFillBar;

    [SerializeField]
    private Transform ReSpawnPointTrs;

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

    private bool isUsingSkill;
    public bool IsUsingSkill
    {
        get => isUsingSkill;
        set => isUsingSkill = value;
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

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.position = ReSpawnPointTrs.position;
        rb.MovePosition(ReSpawnPointTrs.position);
    }

    public void TakeDamage(int _damage)
    {
        if (Die || isInvincible) return;
        Hp -= _damage;
        Debug.Log("현재 hp : "+ hp);
        Debug.Log("현재 Hp : " + Hp);
        float damagePercentage = (float)_damage / maxHp;

        if (hpFillBar != null)
        {
            hpFillBar.fillAmount = (float)hp / maxHp;
            Debug.Log((float)hp / maxHp);
        }

        if (damagePercentage >= 0.05f) // 5% 이상일 때
        {
            anim.SetTrigger("Hit");
            IsInvincible = true;
            StartCoroutine(DamgeIsInvincible(1.5f));
        }
        else // 5% 미만일 때
        {
            cameraShake.Shake(0.1f, 0.2f);
            anim.SetTrigger("Hit2");
            IsInvincible = true;
            StartCoroutine(DamgeIsInvincible(1f));
        }
        if (Hp <= 0)
        {
            anim.SetTrigger("Die");
            Die = true;
            StartCoroutine(PlayerWaitTwoSeconds());
        }
    }

    private IEnumerator PlayerWaitTwoSeconds()
    {
        // 2초 동안 대기
        yield return new WaitForSeconds(2.0f);
        StartCoroutine(UIManager.Instance.FadeInRoutine(1f));
        UIManager.Instance.ReSpawnUI();
        // 2초 뒤에 실행할 코드 작성
        Debug.Log("2초가 지났습니다!");
    }

    public void PlayerReSpawn()
    {
        Hp = maxHp;
        hpFillBar.fillAmount = 1f;
        anim.Play("Blend Tree");
        this.gameObject.transform.position = ReSpawnPointTrs.position;
        Die = false;
    }

    // 클래스 내부에서 값을 설정할 때
    public void SetAttackPower(int value)
    {
        Att = value; // 또는 Att = value; 둘 다 가능
    }

    public int GetAttackPower()
    {
        return Att;
    }

    public void SetInvincible(bool value)
    {
        IsInvincible = value;
    }
    IEnumerator DamgeIsInvincible(float time)
    {
        yield return new WaitForSeconds(time);
        IsInvincible = false;
    }

    public void OnHit()
    {
        // 0.2초 동안 0.1f 강도로 흔들기
        cameraShake.Shake(0.2f, 0.1f);
    }
}
