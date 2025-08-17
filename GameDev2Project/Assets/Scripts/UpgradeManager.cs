using System.Reflection;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public float soulsNeeded;

    private void Awake()
    {
        soulsNeeded = 3;
        instance = this;
    }

    private void Update()
    {
        
    }

    public void UpgradeHealth(int health)
    {
        gamemanager.instance.playerScript.Health = health;
    }

    public void UpgradeSpeed(int speed) 
    {
        gamemanager.instance.playerScript.Speed = speed;
    }

    public void UpgradeJumpCount(int jumpCount)
    {
        gamemanager.instance.playerScript.JumpMax = jumpCount;
    }

}
