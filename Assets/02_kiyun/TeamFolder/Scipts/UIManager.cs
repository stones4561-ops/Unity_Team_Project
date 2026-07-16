using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;
    public static UIManager Instance {  get { return instance; } }

    [SerializeField]
    private Image ReSpawnImage;
    [SerializeField]
    private GameObject inven;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);
        ReSpawnImage.gameObject.SetActive(false);
    }

    public void ReSpawnUI()
    {
        StartCoroutine(WaitTwoSeconds());
    }

    private IEnumerator WaitTwoSeconds()
    {
        // 2초 동안 대기
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(FadeOutRoutine(2.0f));
        Player.Instance.PlayerReSpawn();
        // 2초 뒤에 실행할 코드 작성
        Debug.Log("2초가 지났습니다!");
    }
    //플레이어 리스본 시
    private IEnumerator FadeOutRoutine(float duration)
    {
       
        if (ReSpawnImage == null) yield break;

        Color startColor = ReSpawnImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            // 새로운 컬러를 생성하여 대입
            ReSpawnImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // 최종적으로 완전히 투명하게 설정
        ReSpawnImage.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
        ReSpawnImage.gameObject.SetActive(false);
    }
    //플레이어 사망 시
    public IEnumerator FadeInRoutine(float duration)
    {
        if (inven.activeSelf)
            inven.SetActive(false);
        if (ReSpawnImage == null) yield break;

        Color color = ReSpawnImage.color;
        ReSpawnImage.color = new Color(color.r, color.g, color.b, 0f);

        // 시작 전 오브젝트 활성화
        ReSpawnImage.gameObject.SetActive(true);

        Color startColor = ReSpawnImage.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            // 0(투명)에서 1(불투명)로 증가
            float alpha = Mathf.Lerp(0f, 1f, elapsed / duration);

            ReSpawnImage.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        // 최종적으로 완전히 불투명하게 설정
        ReSpawnImage.color = new Color(startColor.r, startColor.g, startColor.b, 1f);
    }
}
