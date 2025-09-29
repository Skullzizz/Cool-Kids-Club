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
    public TextMeshProUGUI[] upgradeLvlTexts;
    public int healthLvl=0;
    public int speedLvl=0;
    public int jumpLvl=0;
    public int maxLevel = 5;

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
        if (healthLvl >= maxLevel)
        {
            return;
        }
        gamemanager.instance.playerScript.updateStats(playerController.PlayerStats.Health, health);
        healthLvl++;
    }

    public void UpgradeSpeed(int speed) 
    {
        if (speedLvl >= maxLevel)
        {
            return;
        }
        gamemanager.instance.playerScript.updateStats(playerController.PlayerStats.Speed, speed);
        speedLvl++;
    }

    public void UpgradeJumpCount(int jumpCount)
    {
        if (jumpLvl >= maxLevel)
        {
            return;
        }
        gamemanager.instance.playerScript.updateStats(playerController.PlayerStats.JumpMax, jumpCount);
        jumpLvl++;
    }


    //displays the text and buttons for upgrades
    public void ShowRandomUpgrades()
    {
        List<Upgrades> choices = GetRandomUpgrades(3);

        for(int i = 0; i < choices.Count; i++)
        {
            Upgrades upgrades = choices[i];
            upgradeTexts[i].text = upgrades.ToString();
            upgradeLvlTexts[i].text = "Lvl: "+GetUpgradeLvl(upgrades).ToString()+" / "+maxLevel;
            if(GetUpgradeLvl(upgrades)>=maxLevel)
            {
                upgradeButtons[i].interactable=false;
            }
            else
            {
                upgradeButtons[i].interactable = true;
                upgradeButtons[i].onClick.RemoveAllListeners();
                upgradeButtons[i].onClick.AddListener(() => GiveUpgrades(upgrades));
                upgradeButtons[i].onClick.AddListener(() => gamemanager.instance.stateUnpause());
            }
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
                UpgradeSpeed(3);
                break;
            case Upgrades.JumpCount:
                UpgradeJumpCount(1);
                break;
        }
    }
    public int GetUpgradeLvl(Upgrades upgrades)
    {
        switch (upgrades)
        {
            case Upgrades.Health:
                return healthLvl;
            case Upgrades.Speed: 
                return speedLvl;
            case Upgrades.JumpCount:
                return jumpLvl;
        }
        return 0;
    }
    
}
