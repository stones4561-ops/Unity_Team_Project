using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    public Collider swordCollider; // 검의 콜라이더를 인스펙터에서 연결

    private bool isAttacking;
    private Coroutine disableCoroutine;


    private void Start()
    {
        swordCollider.enabled = false; // 평소엔 끔
    }

    public void OnAttack()
    {
        swordCollider.enabled = true;
        isAttacking = true;
    }

    public void DisableAttack()
    {
        swordCollider.enabled = false;
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Ground")) return;

        IDamageable idamageable = other.GetComponent<IDamageable>();
        MonsterBase monster = other.GetComponent<MonsterBase>(); // 여기서 null일 수 있음

        // 1. 데미지 가능한 오브젝트인지 확인
        if (idamageable != null)
        {
            // 몬스터 컴포넌트가 존재할 때만 죽음 여부 확인
            if (monster == null || !monster.IsDead)
            {
                

                // 2. 피격 이펙트 처리 (몬스터 정보가 확실할 때만)
                if (monster != null && !monster.IsDead)
                {
                    PlayHitEffect(other);
                    idamageable.TakeDamage(Player.Instance.GetAttackPower());
                }
            }
        }
    }

    // 이펙트 로직을 별도 함수로 분리하여 코드 가독성을 높임
    private void PlayHitEffect(Collider other)
    {
        GameObject hitEffect = EffectManager.Instance.GetEffect("Hit");
        if (hitEffect != null)
        {
            hitEffect.transform.position = other.ClosestPointOnBounds(transform.position);
            hitEffect.SetActive(true);

            ParticleSystem ps = hitEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop();
                ps.Play();
            }
        }
    }

}