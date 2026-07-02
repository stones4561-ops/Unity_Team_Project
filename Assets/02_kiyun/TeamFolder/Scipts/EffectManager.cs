using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    private static EffectManager instance;
    public static EffectManager Instance { get { return instance; } }

    [System.Serializable]
    public struct EffectData
    {
        public string effectName; // 예: "Hit", "Explosion"
        public GameObject prefab;
        public int poolSize;
        public Transform parentTransform;
    }

    public List<EffectData> effectList; // 인스펙터에서 여러 이펙트 등록 가능
    private Dictionary<string, Queue<GameObject>> pools = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this.gameObject);

        foreach (var data in effectList)
        {
            Queue<GameObject> pool = new Queue<GameObject>();
            for (int i = 0; i < data.poolSize; i++)
            {
                // 부모가 지정되어 있으면 그곳에, 없으면 EffectManager 자신을 부모로 사용
                Transform parent = data.parentTransform != null ? data.parentTransform : transform;
                GameObject obj = Instantiate(data.prefab, parent);

                obj.SetActive(false);
                pool.Enqueue(obj);
            }
            pools.Add(data.effectName, pool);
        }
    }

    public GameObject GetEffect(string effectName)
    {
        if (!pools.ContainsKey(effectName)) return null;

        var pool = pools[effectName];
        if (pool.Count == 0) return null; // 필요시 여기서 추가 생성 로직 추가

        GameObject obj = pool.Dequeue();
        pool.Enqueue(obj);
        return obj;
    }
}