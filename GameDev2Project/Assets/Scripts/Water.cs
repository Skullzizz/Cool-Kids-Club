using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float gravity = 10;
    float ogGrav;
    public bool inWater;
    public static Water instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ogGrav = gamemanager.instance.playerScript.gravity;
    }

    void Update()
    {
        if (inWater && gamemanager.instance.playerScript.locked||gamemanager.instance.isPaused)
        {
            ExitWater();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !gamemanager.instance.playerScript.locked)
        {
            EnterWater();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ExitWater();
        }
    }

    void EnterWater()
    {
        inWater = true;
        wallrunController.instance.ableToWallRun = false;
        gamemanager.instance.playerScript.gravity = gravity;
        gamemanager.instance.WaterScreen(true);
    }

    void ExitWater()
    {
        inWater = false;
        wallrunController.instance.ableToWallRun = true;
        gamemanager.instance.playerScript.gravity = ogGrav;
        gamemanager.instance.WaterScreen(false);
    }
}