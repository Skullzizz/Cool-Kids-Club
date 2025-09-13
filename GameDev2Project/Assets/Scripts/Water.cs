using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float gravity = 10;
    float ogGrav;
    public bool inWater;
    public static Water instance;
    bool wasInWater = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        ogGrav=gamemanager.instance.playerScript.gravity;
    }

    // Update is called once per frame
    void Update()
    {
        if (gamemanager.instance.isPaused)
            return;

        if(gamemanager.instance.playerScript.locked)
        {
            waterFilter(false);
            return;
        }
        if (inWater)
        {
            waterFilter(true);
        }
        else
        {
            waterFilter(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!gamemanager.instance.playerScript.locked)
        {
            wallrunController.instance.ableToWallRun = false;
            inWater= true;
            gamemanager.instance.playerScript.gravity = gravity;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            wallrunController.instance.ableToWallRun = true;
            inWater= false;
            gamemanager.instance.playerScript.gravity = ogGrav;
        }
    }

    void waterFilter(bool isWater)
    {
        gamemanager.instance.WaterScreen(isWater);
    }

}
