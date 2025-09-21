using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class ButtonFunctions : MonoBehaviour, IPointerEnterHandler
{

    [Header("Main Menu Panels")]
    [SerializeField] private GameObject panelMain;
    [SerializeField] private GameObject panelOptions;
    [SerializeField] private GameObject panelCredits;
    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }

    public void restart()
    {
        //Destroy(gamemanager.instance.playerScript.gameObject);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gamemanager.instance.stateUnpause();
        //gamemanager.instance.RefreshUI();
    }

    public void Respawn()
    {
        gamemanager.instance.playerScript.SpawnPlayer();
        gamemanager.instance.stateUnpause();
    }

    public void quit()
    {

#if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying = false; 
#else
        Application.Quit();
#endif
    }

    public void startGame(string sceneName)
    {
        SceneManager.LoadScene(sceneName); // main game scene needs to be added here
        gamemanager.instance.stateUnpause();
    }                                      // SEE: QuitToMain below
    public void openOptions()
    {
        panelMain.SetActive(false);
        panelOptions.SetActive(true);
    }

    public void openCredits()
    {
        panelMain.SetActive(false);
        panelCredits.SetActive(true);
    }

    public void LoadGame()
    {
        gamemanager.instance.LoadAsync();
    }

    public void SaveGame()
    {
        gamemanager.instance.SaveAsync();
    }    

    public void backToMain(GameObject currentPanel)
    {
        if (currentPanel != null && panelMain != null){


            currentPanel.SetActive(false);
            panelMain.SetActive(true);
        }

    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        SceneManager.LoadScene("MainMenu");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null)
            EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
