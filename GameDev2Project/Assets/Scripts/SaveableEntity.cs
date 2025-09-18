using System;
using System.Collections.Generic;
using UnityEngine;

public class SaveableEntity : MonoBehaviour
{
    [SerializeField] string id = string.Empty;

    public string ID => id;

    [ContextMenu("Generate ID")]
    private void GenerateID() => id = Guid.NewGuid().ToString();

    public object CaptureStates()
    {
        var state = new Dictionary <string, object>();

        foreach (var saveable in GetComponents<ISaveable>())
        {
            Debug.Log("Beginning SaveableEntity CaptureStates for " + saveable.GetType().ToString());
            state[saveable.GetType().ToString()] = saveable.SaveState();
        }

        return state;
    }

    public void RestoreStates(object state)
    {
        var stateDictionary = (Dictionary<string, object>)state;

        foreach (var saveable in GetComponents<ISaveable>())
        {
            string typeName = saveable.GetType().ToString();

            if (stateDictionary.TryGetValue(typeName, out object value))
            {
                saveable.LoadState(value);
            }
        }
    }
}
