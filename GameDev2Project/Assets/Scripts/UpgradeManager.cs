using System.Reflection;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public playerController playerScript;

    private void Awake()
    {
        playerScript= GameObject.FindWithTag("Player").GetComponent<playerController>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            UpgradeHealth(10);
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            UpgradeJumpCount(1);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            UpgradeSpeed(3);
        }
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
