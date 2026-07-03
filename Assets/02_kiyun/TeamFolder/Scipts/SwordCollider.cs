using UnityEngine;

public class SwordCollider : MonoBehaviour
{
    private static SwordCollider instance;
    public static SwordCollider Instance {  get { return instance; } }

    public Collider swordCollider; // 검의 콜라이더를 인스펙터에서 연결

    private bool isAttacking;
    private Coroutine disableCoroutine;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
    }

    private void Start()
    {
        swordCollider.enabled = false; // 평소엔 끔
    }

    public void EnableAttack(float duration)
    {
        swordCollider.enabled = true;
        isAttacking = true;

        // 만약 이전 코루틴이 돌고 있다면 멈춤
        if (disableCoroutine != null) StopCoroutine(disableCoroutine);

        // 설정한 초(duration)가 지나면 DisableAttack 호출
        disableCoroutine = StartCoroutine(DisableAfterSeconds(duration));
    }

    private System.Collections.IEnumerator DisableAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        DisableAttack();
    }

    public void DisableAttack()
    {
        swordCollider.enabled = false;
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 실제로 누가 닿았는지 확인
        Debug.Log($"닿은 대상: {other.gameObject.name}, 대상의 태그: {other.gameObject.tag}"); 

        if (other.CompareTag("Player")) return;
        if (other.CompareTag("Ground")) return;

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
