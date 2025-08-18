using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunctions : MonoBehaviour
{

    public void resume()
    {
        gamemanager.instance.stateUnpause();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

    public void HealthUpgrade()
    {
        UpgradeManager.instance.UpgradeHealth(10);
        gamemanager.instance.stateUnpause();
    }

    public void JumpUpgrade()
    {
        UpgradeManager.instance.UpgradeJumpCount(1);
        gamemanager.instance.stateUnpause();
    }

    public void SpeedUpgrade()
    {
        UpgradeManager.instance.UpgradeSpeed(5);
        gamemanager.instance.stateUnpause();
    }
}
