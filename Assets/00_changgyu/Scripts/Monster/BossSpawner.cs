using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    [SerializeField]
    private GameObject bossPF;
    [SerializeField]
    private float spawnRate;
    [SerializeField]
    private GameObject player;

    private GameObject boss;
    private bool reswpawning=false;





    
    private void Start()
    {
        GameObject mon = Instantiate(bossPF, transform.position, Quaternion.Euler(0, 90, 0), transform);
        mon.GetComponent<ChaseBase>()?.SetPlayer(player.transform);
        mon.GetComponent<MonsterBase>()?.SetPlayer(player.transform);
        boss = mon;

    }

    private void Update()
    {
        if(!boss.activeSelf&&!reswpawning)
        {
            Invoke("Respawn", spawnRate);
            reswpawning = true;
        }
        
    }

    private void Respawn()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.x += Random.Range(-3f, 3f);
        boss.transform.position = spawnPos;
        boss.SetActive(true);
        reswpawning = false;
    }

   
}
