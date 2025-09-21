using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private SceneDataSO[] sceneDataSOArray;
    private Dictionary<string, int> sceneIDtoIndexMap = new Dictionary<string, int>();

    private void Awake()
    {
        gamemanager.instance.sceneLoader = this;

        PopulateSceneMappings();
    }

    private void PopulateSceneMappings()
    {
        foreach (var sceneDataSO in sceneDataSOArray)
        {
            sceneIDtoIndexMap[sceneDataSO.SceneName] = sceneDataSO.SceneIndex;
        }
    }

    public void LoadSceneByIndex(string savedSceneID)
    {
        if (sceneIDtoIndexMap.TryGetValue(savedSceneID, out int sceneIndex))
        {
            SceneManager.LoadScene(sceneIndex);
        }
        else
        {
            Debug.LogError("No Scene found for ID: " +  savedSceneID);
        }
    }

    public async Task LoadSceneByIndexAsync(string savedSceneID)
    {
        if (sceneIDtoIndexMap.TryGetValue(savedSceneID, out int sceneIndex))
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
            asyncLoad.allowSceneActivation = false;

            while (!asyncLoad.isDone)
            {
                if (asyncLoad.progress >= 0.9f)
                {
                    asyncLoad.allowSceneActivation = true;
                    break;
                }
                await Task.Yield();
            }
        }
        else
        {
            Debug.LogError("No Scene Found for ID:" + savedSceneID);
        }
    }
}
