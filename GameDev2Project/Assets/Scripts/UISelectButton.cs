using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UISelectButton : MonoBehaviour
{

    [SerializeField] Selectable firstButton;
 

    void OnEnable()
    {
        if(firstButton != null)
        {
            EventSystem.current?.SetSelectedGameObject(firstButton.gameObject);
        }
        
    }
}
