using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] float gravity = 10;
    float ogGrav;
    bool inWater;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ogGrav=playerController.instance.gravity;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerController.instance.locked)
        {
            waterFilter(false);
            return;
        }
        if (inWater)
        {
            waterGrav(gravity);
            waterFilter(true);
        }
        else
        {
            waterGrav(ogGrav);
            waterFilter(false);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!playerController.instance.locked)
        {
            inWater= true;
            wallrunController.instance.ableToWallRun = false;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inWater= false;
            wallrunController.instance.ableToWallRun = true;
        }
    }

    void waterGrav(float grav)
    {
        playerController.instance.gravity = grav;
    }

    void waterFilter(bool isWater)
    {
        gamemanager.instance.WaterScreen(isWater);
    }

}
