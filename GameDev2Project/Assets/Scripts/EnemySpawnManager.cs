using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawnManager : MonoBehaviour
{


    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private Dictionary<GameObject,GameObject> enemyToPrefabMap = new Dictionary<GameObject,GameObject>();

    void Awake()
    {
        EnemyAI[] EnemiesOnLoad = Object.FindObjectsByType<EnemyAI>(FindObjectsSortMode.None);
        EnemyAI[] RangedEnemiesOnLoad = Object.FindObjectsByType<EnemyRanged>(FindObjectsSortMode.None);
        EnemyAI[] MeleeEnemiesOnLoad = Object.FindObjectsByType<EnemyMelee>(FindObjectsSortMode.None);
        EnemiesOnLoad.Concat(RangedEnemiesOnLoad);
        EnemiesOnLoad.Concat(MeleeEnemiesOnLoad);
        foreach (EnemyAI enemy in EnemiesOnLoad)
        {
            spawnedEnemies.Add(enemy.gameObject);
            enemyToPrefabMap[enemy.gameObject] = Resources.Load(enemy.basePrefabPath, typeof(GameObject)) as GameObject;
        }

    }


    public void Save(ref SceneEnemyData data)
    {
        List<EnemySaveData> enemySavedataList = new List<EnemySaveData>();

        for (int i = spawnedEnemies.Count - 1; i >= 0; i--)
        {
            if (spawnedEnemies[i] != null)
            {
                GameObject enemy = spawnedEnemies[i];
                //Debug.Log(enemy.name + " is being saved!");
                EnemySaveData saveData = new EnemySaveData
                {
                    HP = enemy.GetComponent<EnemyAI>().HP,
                    Position = enemy.transform.position,
                    rotation = enemy.transform.rotation,
                    EnemyPrefab = enemyToPrefabMap[enemy]
                };

                enemySavedataList.Add(saveData);
            }

            else
            {
                spawnedEnemies.RemoveAt(i);
            }
        }

        data.Enemies = enemySavedataList.ToArray();
    }

    public void Load(SceneEnemyData data)
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }

        spawnedEnemies.Clear();
        enemyToPrefabMap.Clear();

        foreach (var enemy in data.Enemies)
        {
            if ( enemy.EnemyPrefab != null )
            {
                GameObject spawnedEnemy = Instantiate(enemy.EnemyPrefab, enemy.Position, enemy.rotation);
                spawnedEnemies.Add(spawnedEnemy);
                enemyToPrefabMap[spawnedEnemy] = enemy.EnemyPrefab;
            }
        }
    }
}

[System.Serializable]
public struct SceneEnemyData
{
    public EnemySaveData[] Enemies;
}

[System.Serializable]
public struct EnemySaveData
{
    public int HP;
    public Vector3 Position;
    public Quaternion rotation;
    public GameObject EnemyPrefab;
}