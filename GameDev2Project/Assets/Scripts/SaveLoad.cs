using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;



public class SaveLoad
{
    private static SaveData saveData = new SaveData();

    [System.Serializable]
    public struct SaveData
    {
        public PlayerData PlayerData;
        public PlayerInventoryData InventoryData;
        public SceneThrowableData ThrowableData;
        public SceneEnemyData EnemyData;
    }


    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/data" + ".sav";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    private static void HandleSaveData()
    {
        gamemanager.instance.playerScript.Save(ref saveData.PlayerData);
        //gamemanager.instance.playerInventory.Save(ref saveData.InventoryData);
        EnemySpawnManager eSpawnManager = gamemanager.instance.enemySpawnManager;
        if (eSpawnManager != null)
        {
            eSpawnManager.Save(ref saveData.EnemyData);
        }
        ThrowableSpawnManager tSpawnManager = gamemanager.instance.throwableSpawnManager;
        if (tSpawnManager != null)
        {
            tSpawnManager.Save(ref saveData.ThrowableData);
        }
    }

    public static void Load()
    {
        string saveFile = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveFile);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        gamemanager.instance.playerScript.Load(saveData.PlayerData);
        //gamemanager.instance.playerInventory.Load(saveData.InventoryData);

        EnemySpawnManager spawnManager = gamemanager.instance.enemySpawnManager;
        if (spawnManager != null)
        {
            spawnManager.Load(saveData.EnemyData);
        }
        ThrowableSpawnManager tSpawnManager = gamemanager.instance.throwableSpawnManager;
        if (tSpawnManager != null)
        {
            tSpawnManager.Load(saveData.ThrowableData);
        }
    }
}
