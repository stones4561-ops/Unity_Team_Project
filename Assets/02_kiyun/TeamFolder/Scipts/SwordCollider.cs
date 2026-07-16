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

        if (other.CompareTag("Player")) return;
        if (other.CompareTag("Ground")) return;

        IDamageable idamageable = other.GetComponent<IDamageable>();

        if (idamageable != null)
        {
            idamageable.TakeDamage(Player.Instance.GetAttackPower());
        }

        //int playerDamage = Player.Instance.Att;

        // 2. 몬스터(EnemyHealth) 컴포넌트 찾기
        //EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        //if (enemy != null) { enemy.TakeDamage(playerDamage); }

        GameObject hitEffect = EffectManager.Instance.GetEffect("Hit");
        if (hitEffect != null)
        {
            // 1. 위치 설정
            hitEffect.transform.position = other.ClosestPointOnBounds(transform.position);

            // 2. 활성화
            hitEffect.SetActive(true);

            // 3. 파티클 시스템 컴포넌트 가져와서 재생 (중요!)
            ParticleSystem ps = hitEffect.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Stop(); // 혹시 재생 중이라면 멈추고
                ps.Play(); // 처음부터 다시 재생
            }
        }

        Debug.Log($"공격 적중: {other.gameObject.name}");
    }

}