using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TabSwitcher : MonoBehaviour
{
    [SerializeField] Toggle tabPause;
    [SerializeField] Toggle tabOptions;

    [SerializeField] GameObject panelPauseMain;
    [SerializeField] GameObject panelPauseOptions;

    [SerializeField] private GameObject firstPauseSelectable;
    [SerializeField] private GameObject firstOptionsSelectable;

    void OnEnable()
    {
        if (tabPause && tabOptions)
        {
            if (!tabPause.isOn && !tabOptions.isOn) tabPause.isOn = true;
        }
        else if (tabPause && !tabPause.isOn)
        {
            tabPause.isOn = true;
        }

        Apply();
    }

    public void Apply()
    {
        bool pauseOn = tabPause && tabPause.isOn;
        bool optionsOn = tabOptions && tabOptions.isOn;

        if (panelPauseMain) panelPauseMain.SetActive(pauseOn);
        if (panelPauseOptions) panelPauseOptions.SetActive(optionsOn);

        if (!EventSystem.current) return;

        GameObject target =
            pauseOn ? (firstPauseSelectable ?? panelPauseMain?.GetComponentInChildren<Selectable>()?.gameObject)
          : optionsOn ? (firstOptionsSelectable ?? panelPauseOptions?.GetComponentInChildren<Selectable>()?.gameObject)
          : null;

        if (target) EventSystem.current.SetSelectedGameObject(target);
    }
}

