using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class SelectOnEnable : MonoBehaviour
{
    [SerializeField] private Selectable first;

    void OnEnable()
    {
        StartCoroutine(NextFrame());
    }

    IEnumerator NextFrame()
    {
        yield return null; 
        EventSystem.current.SetSelectedGameObject(null);
        first.Select();
    }
}
