using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public float soulsNeeded;

    public Button[] upgradeButtons;
    public TextMeshProUGUI[] upgradeTexts;

    //type of upgrades
    public enum Upgrades
    {
        Health, Speed, JumpCount
    }

    //list of the upgrades
    public List<Upgrades> upgrades = new List<Upgrades>
    {
        Upgrades.Health, Upgrades.Speed, Upgrades.JumpCount
    };

    private void Awake()
    {
        instance = this;
    }
    
    //updating stats based on upgrade chosen
    public void UpgradeHealth(int health)
    {
        gamemanager.instance.playerScript.updateStats(playerController.PlayerStats.Health, health);
    }

    public void UpgradeSpeed(int speed) 
    {
        gamemanager.instance.playerScript.updateStats(playerController.PlayerStats.Speed, speed);
    }

    public void UpgradeJumpCount(int jumpCount)
    {
        gamemanager.instance.playerScript.updateStats(playerController.PlayerStats.JumpMax, jumpCount);
    }


    //displays the text and buttons for upgrades
    public void ShowRandomUpgrades()
    {
        List<Upgrades> choices = GetRandomUpgrades(3);

        for(int i = 0; i < choices.Count; i++)
        {
            Upgrades upgrades = choices[i];
            upgradeTexts[i].text = upgrades.ToString();
            upgradeButtons[i].onClick.RemoveAllListeners();
            upgradeButtons[i].onClick.AddListener(()=>GiveUpgrades(upgrades));
            upgradeButtons[i].onClick.AddListener(() => gamemanager.instance.stateUnpause());
        }       
    }

    //gets three random upgrades
    public List<Upgrades> GetRandomUpgrades(int amtUpgrades)
    {
        List<Upgrades> upgradeChoices = new();
       
        while(upgradeChoices.Count < amtUpgrades)
        {
            int choice = Random.Range(0, upgrades.Count);
            if(!upgradeChoices.Contains(upgrades[choice]))
            {
                upgradeChoices.Add(upgrades[choice]);
            }
        }
        
        return upgradeChoices;
    }

    //calls the upgrade method
    public void GiveUpgrades(Upgrades upgrades)
    {
        switch (upgrades)
        {
            case Upgrades.Health:
                UpgradeHealth(10);
                break;
            case Upgrades.Speed:
                UpgradeSpeed(5);
                break;
            case Upgrades.JumpCount:
                UpgradeJumpCount(1);
                break;
        }
    }
    
}
