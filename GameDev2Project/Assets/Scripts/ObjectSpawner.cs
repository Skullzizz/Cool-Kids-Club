using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{
    public static List<ObjectSpawner> spawners = new List<ObjectSpawner>();
    [SerializeField] GameObject particles;
    [SerializeField] GameObject[] objects;
    [SerializeField] int objectAmt;
    [SerializeField] Transform objectSpawn;
    bool isSummoning = false;

    public static ObjectSpawner instance;

    private void Awake()
    {
        instance = this;
        spawners.Add(this);
    }
    private void OnDestroy()
    {
        spawners.Remove(this);
    }

    public IEnumerator SummonObjects()
    {
        if (isSummoning)
            yield break;
        isSummoning = true;
        for (int i = 0; i < objectAmt; ++i)
        {
            particles.SetActive(true);
            int random = Random.Range(0, objects.Length);
            Vector3 posObj = objectSpawn.position + transform.forward * 3f;
            Instantiate(objects[random], posObj, Quaternion.identity);
            yield return new WaitForSeconds(1);
        }
        particles.SetActive(false);
        isSummoning = false;
    }
}
