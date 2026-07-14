using UnityEngine;
using System.Collections.Generic;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject monPF;
    [SerializeField]
    private int monCount;
    [SerializeField]
    private float spawnRate;
    [SerializeField]
    private GameObject player;

    private List<GameObject> monList=new List<GameObject>();


    //몹 수만큼 생성
    private void Start()
    {
        for(int i = 0; i < monCount; i++)
        {
            GameObject mon = Instantiate(monPF,transform.position, Quaternion.Euler(0, 90, 0),transform);
            mon.SetActive(false);
            monList.Add(mon);

            mon.GetComponent<ChaseBase>()?.SetPlayer(player.transform);
        }

        InvokeRepeating("SpawnMonster",0,spawnRate);
    }

    /// <summary>
    /// 몬스터를 가로 랜덤범위에 최대로 생성
    /// </summary>
    private void SpawnMonster()
    {
        foreach(GameObject mon in monList)
        {
            if(!mon.activeSelf)
            {
                Vector3 spawnPos= transform.position;
                spawnPos.x += Random.Range(-3f, 3f);

                mon.transform.position=spawnPos;
                mon.SetActive(true);
            }
        }
    }

}
