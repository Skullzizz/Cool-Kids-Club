using System.Collections;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject particles;
    [SerializeField] GameObject[] enemies;
    [SerializeField] int enemyAmt;
    [SerializeField] Transform enemySpawn;
    bool isSummoning = false;

    public static EnemySpawner instance;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Summon()
    {
        if(isSummoning)
            yield break;
        isSummoning = true;
        for (int i = 0; i < enemyAmt; ++i)
        {
            particles.SetActive(true);
            int random = Random.Range(0, enemies.Length);
            Vector3 posEnemy = enemySpawn.position + transform.forward * 3f;
            Quaternion rotEnemy = enemySpawn.rotation;
            Instantiate(enemies[random], posEnemy, rotEnemy);
            yield return new WaitForSeconds(1);
        }
        particles.SetActive(false);
        isSummoning=false;
    }
}
