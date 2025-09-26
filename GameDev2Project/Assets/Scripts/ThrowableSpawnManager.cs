using System.Collections.Generic;
using UnityEngine;

public class ThrowableSpawnManager : MonoBehaviour
{
    private List<GameObject> spawnedThrowables = new List<GameObject>();
    private Dictionary<GameObject, GameObject> throwableToPrefabMap = new Dictionary<GameObject, GameObject>();

    void Awake()
    {
        throwableDamage[] ThrowablesOnLoad = Object.FindObjectsByType<throwableDamage>(FindObjectsSortMode.None);
        foreach (throwableDamage Throwable in ThrowablesOnLoad)
        {
            spawnedThrowables.Add(Throwable.gameObject);
            throwableToPrefabMap[Throwable.gameObject] = Resources.Load(Throwable.basePrefabPath, typeof(GameObject)) as GameObject;
        }

    }


    public void Save(ref SceneThrowableData data)
    {
        List<ThrowableSaveData> ThrowableSavedataList = new List<ThrowableSaveData>();

        for (int i = spawnedThrowables.Count - 1; i >= 0; i--)
        {
            if (spawnedThrowables[i] != null)
            {
                GameObject throwable = spawnedThrowables[i];
                //Debug.Log(throwable.name + " is being saved!");
                ThrowableSaveData saveData = new ThrowableSaveData
                {
                    HP = throwable.GetComponent<throwableDamage>().throwableHP,
                    isInInventory = throwable.GetComponent<throwableDamage>().isInInventory,
                    isHeld = throwable.GetComponent<throwableDamage>().isHeld,
                    Position = throwable.transform.position,
                    rotation = throwable.transform.rotation,
                    ThrowablePrefab = throwableToPrefabMap[throwable]
                };

                ThrowableSavedataList.Add(saveData);
            }

            else
            {
                spawnedThrowables.RemoveAt(i);
            }
        }

        data.Throwables = ThrowableSavedataList.ToArray();
    }

    public void Load(SceneThrowableData data)
    {
        foreach (var throwable in spawnedThrowables)
        {
            if (throwable != null)
            {
                Destroy(throwable);
            }
        }

        spawnedThrowables.Clear();
        throwableToPrefabMap.Clear();

        foreach (var throwable in data.Throwables)
        {
            if (throwable.ThrowablePrefab != null)
            {
                GameObject spawnedThrowable = Instantiate(throwable.ThrowablePrefab, throwable.Position, throwable.rotation);
                spawnedThrowables.Add(spawnedThrowable);
                throwableToPrefabMap[spawnedThrowable] = throwable.ThrowablePrefab;
                spawnedThrowable.GetComponent<throwableDamage>().throwableHP = throwable.HP;
                if (throwable.isInInventory)
                {
                    playerInventory playerInv = gamemanager.instance.playerInventory;
                    if (playerInv != null)
                    {
                        playerInv.AddItem(spawnedThrowable);

                        playerInv.equippedWeapon = spawnedThrowable;
                        playerInv.equippedWeaponIndex = playerInv.inventory.IndexOf(spawnedThrowable);
                        spawnedThrowable.GetComponent<Rigidbody>().useGravity = false;
                        spawnedThrowable.SetActive(false);
                        playerInv.UpdateWeaponUI();
                    }
                }
                if (throwable.isHeld)
                {
                    gamemanager.instance.throwScript.TryPickup(spawnedThrowable);
                }
            }
        }
    }
}

[System.Serializable]
public struct SceneThrowableData
{
    public ThrowableSaveData[] Throwables;
}

[System.Serializable]
public struct ThrowableSaveData
{
    public bool isInInventory;
    public bool isHeld;
    public int HP;
    public Vector3 Position;
    public Quaternion rotation;
    public GameObject ThrowablePrefab;
}