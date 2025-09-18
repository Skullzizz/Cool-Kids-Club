using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;



public class SaveLoad : MonoBehaviour
{
    public static SaveLoad instance;
    static string saveDataPath => $"{Application.persistentDataPath}/data.sav";

    private void Awake()
    {
        instance = this;
    }

    [ContextMenu("Save")]
    public void Save()
    {
        var state = LoadFile();
        CaptureState(state);
        SaveFile(state);
    }

    [ContextMenu("Load")]
    public void Load()
    {
        var state = LoadFile();
        RestoreState(state);
    }

    void SaveFile(object state)
    {
        using (var stream = File.Open(saveDataPath, FileMode.Create))
        {
            var formatter = new BinaryFormatter();
            formatter.Serialize(stream, state);
        }
    }

    Dictionary<string, object> LoadFile()
    {
        if (!File.Exists(saveDataPath))
        {
            return new Dictionary<string, object>();
        }

        using (FileStream stream = File.Open(saveDataPath, FileMode.Open))
        {
            var formatter = new BinaryFormatter();
            return (Dictionary<string, object>)formatter.Deserialize(stream);
        }
    }

    void CaptureState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None))
        {
            Debug.Log("Saving " + saveable.name + " with ID " + saveable.ID);
            state[saveable.ID] = saveable.CaptureStates();
        }
    }

    static void RestoreState(Dictionary<string, object> state)
    {
        foreach (var saveable in FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None))
        {
            if (state.TryGetValue(saveable.ID, out object value))
            {
                saveable.RestoreStates(value);
                Debug.Log("Loading " + saveable.name + " with ID " + saveable.ID);
            }
        }

    }

}
