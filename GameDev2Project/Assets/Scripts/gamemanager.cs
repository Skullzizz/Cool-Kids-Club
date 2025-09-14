using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuUpgrade;

    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text playerLevelText;
 

    [SerializeField] private PauseDimmer pauseDimmer;

    public Image playerHPBar;
    public Image playerXPBar;
    public Image playerArmorBar;
    public Image WeaponIcon;
    public GameObject PlayerDamageScreen;
    public Image PlayerDeathScreen;
    public TextMeshProUGUI storedWeaponText;

    public GameObject player;
    public playerController playerScript;
    public throwPhysics throwScript;
    public GameObject playerSpawnPos;
    public GameObject checkpointPopup;

    public playerInventory playerInventory;

    Camera minimapCam;

    [SerializeField] GameObject waterScreen;


    public bool isPaused;

    float timeScaleOrig;

    int gameGoalCount;

    int playerLevelCount = 1;

    public int enemiesKilled = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        throwScript = player.GetComponent<throwPhysics>();
        playerInventory = player.GetComponent<playerInventory>();

        minimapCam = GameObject.FindWithTag("MinimapCam").GetComponent<Camera>();
        playerSpawnPos = GameObject.FindWithTag("Player Spawn");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
            {
                stateUnpause();
            }
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseDimmer.ShowDim();
        FindFirstObjectByType<PauseMenuMusic>().PlayMusic();
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseDimmer.HideDim();
        menuActive.SetActive(false);
        menuActive = null;
        FindFirstObjectByType<PauseMenuMusic>().StopMusic();
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;

        gameGoalCountText.text = gameGoalCount.ToString("F0");

        if (gameGoalCount <= 0)
        {
            // You Won!
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
            pauseDimmer.ShowDim();
        }
    }

    public void updateEnemyDeaths(int amt)
    {
        if (menuActive == null)
        {
            enemiesKilled += amt;


            playerScript.updatePlayerUI();

            if (enemiesKilled >= UpgradeManager.instance.soulsNeeded)
            {
                playerLevelCount++;
                enemiesKilled = 0;
                //Show Upgrades
                UpgradeManager.instance.soulsNeeded = Mathf.CeilToInt((float)(UpgradeManager.instance.soulsNeeded * 1.5));
                statePause();
                menuActive = menuUpgrade;
                menuActive.SetActive(true);
                UpgradeManager.instance.ShowRandomUpgrades();
            }
            playerLevelText.text = playerLevelCount.ToString("F0");
        }
    }

    public void loseGame()
    {
        if (menuActive == null)
        {
            PlayerDeathScreen.gameObject.SetActive(true);
            //minimapCam.enabled = false;
            statePause();
            menuActive = menuLose;
            menuActive.SetActive(true);
            pauseDimmer.ShowDim();
        }
    }

    public void WaterScreen(bool isWater)
    {
        waterScreen.SetActive(isWater);
    }
}
