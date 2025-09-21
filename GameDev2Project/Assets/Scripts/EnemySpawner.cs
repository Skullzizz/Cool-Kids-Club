using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnemySpawner : MonoBehaviour
{
    public static List<EnemySpawner> spawners = new List<EnemySpawner>();

    [SerializeField] GameObject particles;
    [SerializeField] GameObject[] enemies;
    [SerializeField] int enemyAmt;
    [SerializeField] Transform enemySpawn;
    bool isSummoning = false;

    public static EnemySpawner instance;

    private void Awake()
    {
        instance = this;
        spawners.Add(this);
    }
    private void OnDestroy()
    {
        spawners.Remove(this);
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
