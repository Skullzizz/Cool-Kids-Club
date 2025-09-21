using UnityEngine;
using System.IO;
using System.Threading.Tasks;



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
        public SceneSaveData sceneData;
        public SceneCollectibleData CollectibleData;
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
        CollectibleSpawnManager cSpawnManager = gamemanager.instance.collectibleSpawnManager;
        if (cSpawnManager != null)
        {
            cSpawnManager.Save(ref saveData.CollectibleData);
        }
        gamemanager.instance.sceneData.Save(ref saveData.sceneData);
    }

    public static async Task SaveAsynchronously()
    {
        await SaveAsync();
    }

    private static async Task SaveAsync()
    {
        HandleSaveData();

        await File.WriteAllTextAsync(SaveFileName(), JsonUtility.ToJson(saveData, true));
    }

    public static void Load()
    {
        string saveFile = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveFile);
        HandleLoadData();
    }

    private static void HandleLoadData()
    {
        gamemanager.instance.sceneData.Load(saveData.sceneData);
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
        CollectibleSpawnManager cSpawnManager = gamemanager.instance.collectibleSpawnManager;
        if (cSpawnManager != null)
        {
            cSpawnManager.Load(saveData.CollectibleData);
        }
    }

    public static async Task LoadAsync()
    {
        string saveContent = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveContent);

        await HandleLoadDataAsync();
    }

    private static async Task HandleLoadDataAsync()
    {
        await gamemanager.instance.sceneData.LoadAsync(saveData.sceneData);

        await gamemanager.instance.sceneData.WaitForSceneToBeFullyLoaded();

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
        CollectibleSpawnManager cSpawnManager = gamemanager.instance.collectibleSpawnManager;
        if (cSpawnManager != null)
        {
            cSpawnManager.Load(saveData.CollectibleData);
        }
    }
    
}
