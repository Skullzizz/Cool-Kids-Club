using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor.Rendering.Universal;

public class gamemanager : MonoBehaviour
{
    public static gamemanager instance;
    [SerializeField] public SaveLoad saveLoad;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject loseMenuRespawn;
    [SerializeField] GameObject loseMenuNoSaveRespawn;
    [SerializeField] GameObject menuUpgrade;
    [SerializeField] AudioSource UIAudio;
    [SerializeField] public GameObject loadingScreen;

    [SerializeField] TMP_Text gameGoalCountText;
    [SerializeField] TMP_Text playerLevelText;


    [SerializeField] private PauseDimmer pauseDimmer;
    [SerializeField] AudioClip pauseMenuMusic;

    public AudioClip prePauseMenuMusic;

    public Image playerHPBar;
    public Image playerXPBar;
    public Image playerArmorBar;
    public Image WeaponIcon;
    public GameObject PlayerDamageScreen;
    public Image PlayerDeathScreen;
    public TextMeshProUGUI storedWeaponText;
    public TextMeshProUGUI CollectibleText;
    public GameObject player;
    public playerController playerScript;
    public throwPhysics throwScript;
    public EnemySpawnManager enemySpawnManager;
    public ThrowableSpawnManager throwableSpawnManager;
    public CollectibleSpawnManager collectibleSpawnManager;
    public SceneData sceneData;
    public SceneLoader sceneLoader;
    public GameObject playerSpawnPos;
    public GameObject checkpointPopup;
    public GameObject collectiblePopup;
    public UIMusicManager uiMusicManager;

    public playerInventory playerInventory;
    public GameObject quitToDesktop;

    Camera minimapCam;

    [SerializeField] GameObject waterScreen;
    [SerializeField] GameObject spaceScreen;
    [SerializeField] GameObject tutorialScreen;
    [SerializeField] GameObject tutorialScreenWebGL;

    public bool isPaused;
    private bool isSaving;
    private bool isLoading;

    float timeScaleOrig;


    int playerLevelCount = 1;

    public int enemiesKilled = 0;

    public bool finalCountDown;
    public bool bossAlive = true;
    public GameObject enemyCountText;
    [SerializeField] GameObject levelUpReady;
    public int amtUpgrades = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;

        if (Time.timeScale == 0f)
            Time.timeScale = 1f;

        timeScaleOrig = Time.timeScale;
        if (GameObject.FindWithTag("Player") != null)
            player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerScript = player.GetComponent<playerController>();
            playerSpawnPos = GameObject.FindWithTag("Player Spawn");
            throwScript = player.GetComponent<throwPhysics>();
            playerInventory = player.GetComponent<playerInventory>();
        }
        if (this.GetComponent<EnemySpawnManager>() != null)
            enemySpawnManager = this.GetComponent<EnemySpawnManager>();
        if (this.GetComponent<ThrowableSpawnManager>() != null)
            throwableSpawnManager = this.GetComponent<ThrowableSpawnManager>();
        if (this.GetComponent<CollectibleSpawnManager>() != null)
            collectibleSpawnManager = this.GetComponent<CollectibleSpawnManager>();

        uiMusicManager = UIAudio.GetComponent<UIMusicManager>();
        if (GameObject.FindWithTag("MinimapCam") != null)
            minimapCam = GameObject.FindWithTag("MinimapCam").GetComponent<Camera>();
        StartCoroutine(ShowTutorial());

        if (menuPause == null) menuPause = GameObject.Find("Pause Menu");
        if (menuWin == null) menuWin = GameObject.Find("Win Menu");
        if (menuLose == null) menuLose = GameObject.Find("Lose Menu");
        if (menuUpgrade == null) menuUpgrade = GameObject.Find("Upgrade Menu");
    }

    private void Start()
    {
        finalCountDown = SceneManager.GetActiveScene().name == "Space";
        if (SaveLoad.CheckSaveData() == false)
        {
            SaveAsync();
        }

        if (player != null)
        {
            LoadParial();
        }


        playerLevelText.text = playerLevelCount.ToString("F0");

        if (finalCountDown)
        {
            enemyCountText.SetActive(false);
            gameGoalCountText.text = "Boss";
        }
        else
            enemyCountText.SetActive(false);
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

        if (Input.GetButtonDown("Save") && !isSaving)
        {
            SaveAsync();
            //Debug.Log("Saving Game");
        }

        if (Input.GetButtonDown("Load") && !isLoading)
        {
            LoadAsync();
            //Debug.Log("Loading Game");
        }
        if (amtUpgrades > 0 && Input.GetKeyDown(KeyCode.U))
        {
            upgradeMenu();
        }
    }

    public void statePause()
    {
#if UNITY_WEBGL
        quitToDesktop.SetActive(false);
#endif
        prePauseMenuMusic = UIAudio.clip;
        ChangeMusic(pauseMenuMusic);
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        pauseDimmer.ShowDim();
        //FindFirstObjectByType<PauseMenuMusic>().PlayMusic();


        if (Water.instance != null && Water.instance.inWater)
            WaterScreen(false);
    }

    public void stateUnpause()
    {
        if (prePauseMenuMusic != null)
            ChangeMusic(prePauseMenuMusic);
        isPaused = false;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        pauseDimmer.HideDim();
        if (menuActive != null)
        {
            menuActive.SetActive(false);
            menuActive = null;
        }
        //if (FindFirstObjectByType<PauseMenuMusic>() != null)
        //FindFirstObjectByType<PauseMenuMusic>().StopMusic();


        if (Water.instance != null && Water.instance.inWater)
            WaterScreen(true);
    }

    public void updateGameGoal(int amount)
    {
        //if(!finalCountDown)
        //{
        //    gameGoalCount += amount;
        //
        //    gameGoalCountText.text = gameGoalCount.ToString("F0");
        //}

        if (finalCountDown && !bossAlive)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
            pauseDimmer.ShowDim();
        }

        //else if (!finalCountDown&&gameGoalCount <= 0)
        //{
        //    statePause();
        //    menuActive = menuWin;
        //    menuActive.SetActive(true);
        //    pauseDimmer.ShowDim();
        //}
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
                UpgradeManager.instance.soulsNeeded = Mathf.CeilToInt((float)(UpgradeManager.instance.soulsNeeded * 1.5));
                amtUpgrades++;
                levelUpReady.SetActive(true);
            }
            playerLevelText.text = playerLevelCount.ToString("F0");
        }
    }

    public void loseGame()
    {
        if (menuActive == null)
        {
            //PlayerDeathScreen.gameObject.SetActive(true);
            //minimapCam.enabled = false;
            statePause();
            menuActive = menuLose;
            menuActive.SetActive(true);
            if (SaveLoad.CheckSaveData() == false)
            {
                loseMenuRespawn.SetActive(false);
                loseMenuNoSaveRespawn.SetActive(true);
            }
            else
            {
                loseMenuRespawn.SetActive(true);
                loseMenuNoSaveRespawn.SetActive(false);
            }
            pauseDimmer.ShowDim();
        }
    }

    public void WaterScreen(bool isWater)
    {
        if (waterScreen != null)
            waterScreen.SetActive(isWater);
    }

    public void SpaceScreen(bool isSpace)
    {
        if (spaceScreen != null)
            spaceScreen.SetActive(isSpace);
    }

    public void RefreshUI()
    {
        if (playerHPBar == null)
            playerHPBar = GameObject.Find("Player HP").GetComponent<Image>();
        if (playerXPBar == null)
            playerXPBar = GameObject.Find("Player XP").GetComponent<Image>();
        if (playerArmorBar == null)
            playerArmorBar = GameObject.Find("Armor Fill").GetComponent<Image>();
        if (PlayerDamageScreen == null)
            PlayerDamageScreen = GameObject.Find("Player Damage Screen");
        if (PlayerDeathScreen == null)
            PlayerDeathScreen = GameObject.Find("Player Death Screen").GetComponent<Image>();
        if (player == null)
            player = GameObject.Find("Player");
        if (playerScript == null)
            playerScript = GameObject.Find("Player").GetComponent<playerController>();
        if (throwScript == null)
            throwScript = GameObject.Find("Player").GetComponent<throwPhysics>();
        if (playerSpawnPos == null)
            playerSpawnPos = GameObject.Find("Player Spawn");
        if (playerInventory == null)
            playerInventory = GameObject.Find("Player").GetComponent<playerInventory>();
    }

    public void ChangeMusic(AudioClip nextMusic)
    {
        uiMusicManager.FadeChange(ref UIAudio, nextMusic);
    }

    public async void SaveAsync()
    {
        isSaving = true;
        await SaveLoad.SaveAsynchronously();
        isSaving = false;
    }

    public async void LoadAsync()
    {
        isLoading = true;
        await SaveLoad.LoadAsync();
        isLoading = false;
    }

    public IEnumerator ShowTutorial()
    {
        if (SceneManager.GetActiveScene().name == "Armor")
        {
#if UNITY_WEBGL
            tutorialScreenWebGL.SetActive(true);
            yield return new WaitForSeconds(30f);
            tutorialScreenWebGL.SetActive(false);
#else
            tutorialScreen.SetActive(true);
            yield return new WaitForSeconds(30f);
            tutorialScreen.SetActive(false);
#endif
        }
    }

    public void upgradeMenu()
    {
        if (menuActive == null && !playerScript.isDead && amtUpgrades > 0)
        {
            statePause();
            menuActive = menuUpgrade;
            menuActive.SetActive(true);
            UpgradeManager.instance.ShowRandomUpgrades();
            amtUpgrades--;
            if (amtUpgrades <= 0)
                levelUpReady.SetActive(false);
        }
    }

    public void LoadParial()
    {
        SaveLoad.LoadParial();
    }

    public void Save(ref GameData gameData)
    {
        gameData.playerLevelCount = playerLevelCount;
        gameData.soulsNeeded = UpgradeManager.instance.soulsNeeded;
        gameData.enemiesKilled = enemiesKilled;
        gameData.amtUpgrades = amtUpgrades;
        gameData.healthLvl = UpgradeManager.instance.healthLvl;
        gameData.speedLvl = UpgradeManager.instance.speedLvl;
        gameData.jumpLvl = UpgradeManager.instance.jumpLvl;
    }

    public void Load(GameData gameData)
    {
        playerLevelCount = gameData.playerLevelCount;
        enemiesKilled = gameData.enemiesKilled;
        amtUpgrades = gameData.amtUpgrades;
        if (amtUpgrades > 0)
        {
            levelUpReady.SetActive(true);
        }
        UpgradeManager.instance.healthLvl = gameData.healthLvl;
        UpgradeManager.instance.speedLvl = gameData.speedLvl;
        UpgradeManager.instance.jumpLvl = gameData.jumpLvl;
        UpgradeManager.instance.soulsNeeded = gameData.soulsNeeded;
    }
}

[System.Serializable]
public struct GameData
{
    public int playerLevelCount;
    public float soulsNeeded;
    public int enemiesKilled;
    public int amtUpgrades;
    public int healthLvl;
    public int speedLvl;
    public int jumpLvl;
}
