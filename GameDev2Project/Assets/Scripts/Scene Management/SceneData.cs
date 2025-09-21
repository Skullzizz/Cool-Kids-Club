using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SceneData : MonoBehaviour
{
    public SceneDataSO Data;

    private void Awake()
    {
        gamemanager.instance.sceneData = this;
    }

    public void Save(ref SceneSaveData data)
    {
        data.ID = Data.SceneName;
    }

    public void Load(SceneSaveData data)
    {
        gamemanager.instance.sceneLoader.LoadSceneByIndex(data.ID);
    }

    public async Task LoadAsync(SceneSaveData data)
    {
        await gamemanager.instance.sceneLoader.LoadSceneByIndexAsync(data.ID);
    }

    public Task WaitForSceneToBeFullyLoaded()
    {
        TaskCompletionSource<bool> taskCompletion = new TaskCompletionSource<bool>();

        UnityEngine.Events.UnityAction<Scene, LoadSceneMode> sceneLoaderHandler = null;

        sceneLoaderHandler = (scene, mode) =>
        {
            taskCompletion.SetResult(true);
            SceneManager.sceneLoaded -= sceneLoaderHandler;
        };

        SceneManager.sceneLoaded += sceneLoaderHandler;

        return taskCompletion.Task;
    }
}

[System.Serializable]
public struct SceneSaveData
{
    public string ID;

}