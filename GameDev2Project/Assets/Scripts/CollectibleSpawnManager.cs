using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawnManager : MonoBehaviour
{
    private List<GameObject> spawnedCollectibles = new List<GameObject>();
    private Dictionary<GameObject, GameObject> collectibleToPrefabMap = new Dictionary<GameObject, GameObject>();

    void Awake()
    {
        Collectible[] CollectiblesOnLoad = Object.FindObjectsByType<Collectible>(FindObjectsSortMode.None);
        foreach (Collectible collectible in CollectiblesOnLoad)
        {
            spawnedCollectibles.Add(collectible.gameObject);
            collectibleToPrefabMap[collectible.gameObject] = Resources.Load(collectible.basePrefabPath, typeof(GameObject)) as GameObject;
        }

    }


    public void Save(ref SceneCollectibleData data)
    {
        List<CollectibleSaveData> CollectibleSavedataList = new List<CollectibleSaveData>();

        for (int i = spawnedCollectibles.Count - 1; i >= 0; i--)
        {
            if (spawnedCollectibles[i] != null)
            {
                GameObject collectible = spawnedCollectibles[i];
                //Debug.Log(collectible.name + " is being saved!");
                CollectibleSaveData saveData = new CollectibleSaveData
                {
                    Position = collectible.transform.position,
                    rotation = collectible.transform.rotation,
                    CollectiblePrefab = collectibleToPrefabMap[collectible]
                };

                CollectibleSavedataList.Add(saveData);
            }

            else
            {
                spawnedCollectibles.RemoveAt(i);
            }
        }

        data.Collectibles = CollectibleSavedataList.ToArray();
    }

    public void Load(SceneCollectibleData data)
    {
        foreach (var collectible in spawnedCollectibles)
        {
            if (collectible != null)
            {
                Destroy(collectible);
            }
        }

        spawnedCollectibles.Clear();
        collectibleToPrefabMap.Clear();

        foreach (var colectible in data.Collectibles)
        {
            if (colectible.CollectiblePrefab != null)
            {
                GameObject spawnedCollectible = Instantiate(colectible.CollectiblePrefab, colectible.Position, colectible.rotation);
                spawnedCollectibles.Add(spawnedCollectible);
                collectibleToPrefabMap[spawnedCollectible] = colectible.CollectiblePrefab;
            }
        }
    }
}

[System.Serializable]
public struct SceneCollectibleData
{
    public CollectibleSaveData[] Collectibles;
}

[System.Serializable]
public struct CollectibleSaveData
{
    public Vector3 Position;
    public Quaternion rotation;
    public GameObject CollectiblePrefab;
}