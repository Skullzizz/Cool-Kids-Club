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
        public GameData GameData;
    }


    public static string SaveFileName()
    {
        string saveFile = Application.persistentDataPath + "/data" + ".sav";
        return saveFile;
    }

    public static void Save()
    {
        HandleSaveData();

#if UNITY_WEBGL

        PlayerPrefs.SetString("Saves",JsonUtility.ToJson(saveData,true));
        PlayerPrefs.Save();

#else
        File.WriteAllText(SaveFileName(), JsonUtility.ToJson(saveData, true));
#endif
    }

    private static void HandleSaveData()
    {
        gamemanager.instance.Save(ref saveData.GameData);
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

#if UNITY_WEBGL
        PlayerPrefs.SetString("Saves", JsonUtility.ToJson(saveData, true));
        PlayerPrefs.Save();
        await Task.Yield();
#else
    await File.WriteAllTextAsync(SaveFileName(), JsonUtility.ToJson(saveData, true));
#endif
    }

    public static void Load()
    {
#if UNITY_WEBGL

        saveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("Saves"));

#else
        string saveFile = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveFile);
#endif

        HandleLoadData();
    }

    public static void LoadParial()
    {
#if UNITY_WEBGL

        saveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("Saves"));

#else
        string saveFile = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveFile);
#endif
        
        HandleLoadParialData();
    }

    private static void HandleLoadData()
    {

        gamemanager.instance.sceneData.Load(saveData.sceneData);
        gamemanager.instance.Load(saveData.GameData);
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
#if UNITY_WEBGL
        saveData = JsonUtility.FromJson<SaveData>(PlayerPrefs.GetString("Saves"));
#else
        string saveContent = File.ReadAllText(SaveFileName());

        saveData = JsonUtility.FromJson<SaveData>(saveContent);
#endif
        await HandleLoadDataAsync();
    }

    private static async Task HandleLoadDataAsync()
    {
        gamemanager.instance.loadingScreen.SetActive(true);
        await gamemanager.instance.sceneData.LoadAsync(saveData.sceneData);
        

        await gamemanager.instance.sceneData.WaitForSceneToBeFullyLoaded();
        
        gamemanager.instance.playerScript.Load(saveData.PlayerData);
        //gamemanager.instance.playerInventory.Load(saveData.InventoryData);
        gamemanager.instance.Load(saveData.GameData);
        EnemySpawnManager spawnManager = gamemanager.instance.enemySpawnManager;
        if (spawnManager != null)
        {
            spawnManager.Load(saveData.EnemyData);
        }
        ThrowableSpawnManager tSpawnManager = gamemanager.instance.throwableSpawnManager;
        if (tSpawnManager != null)
        {
            tSpawnManager.Load(saveData.ThrowableData);
            tSpawnManager.LoadInventoryAndHeld(saveData.ThrowableData);
        }
        CollectibleSpawnManager cSpawnManager = gamemanager.instance.collectibleSpawnManager;
        if (cSpawnManager != null)
        {
            cSpawnManager.Load(saveData.CollectibleData);
        }
        gamemanager.instance.loadingScreen.SetActive(false);
    }

    public static void HandleLoadParialData()
    {
        gamemanager.instance.playerScript.LoadParial(saveData.PlayerData);
        ThrowableSpawnManager tSpawnManager = gamemanager.instance.throwableSpawnManager;
        if (tSpawnManager != null)
        {
            tSpawnManager.LoadInventoryAndHeld(saveData.ThrowableData);
        }

    }

    public static void DeleteSaveData()
    {
        File.Delete(SaveFileName());
        UnityEditor.AssetDatabase.Refresh();
    }

    public static bool CheckSaveData()
    {
        if (File.Exists(SaveFileName()))
        {
            return true;
        }
        return false;
    }

    public static string GetSaveString()
    {
        HandleSaveData();
        return JsonUtility.ToJson(saveData, true);
    }

    public static void LoadSaveString(string save)
    {
        saveData = JsonUtility.FromJson<SaveData>(save);
        HandleLoadData();
    }

}
