using System.Reflection;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public playerController playerScript;
    public float soulsNeeded;

    private void Awake()
    {
        soulsNeeded = 3;
        instance = this;
        playerScript = GameObject.FindWithTag("Player").GetComponent<playerController>();
    }

    private void Update()
    {
        
    }

    public void UpgradeHealth(int health)
    {
        int currHP= (int)typeof(playerController).GetField("HP",BindingFlags.NonPublic | BindingFlags.Instance).GetValue(playerScript);
        typeof(playerController).GetField("HP", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(playerScript, health+currHP);
        playerScript.updatePlayerUI();
    }

    public void UpgradeSpeed(int speed) 
    {
        int currSpeed = (int)typeof(playerController).GetField("speed", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(playerScript);
        typeof(playerController).GetField("speed", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(playerScript, speed + currSpeed);
    }

    public void UpgradeJumpCount(int jumpCount)
    {
        int currJump = (int)typeof(playerController).GetField("jumpMax", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(playerScript);
        typeof(playerController).GetField("jumpMax", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(playerScript, jumpCount + currJump);
    }

}
