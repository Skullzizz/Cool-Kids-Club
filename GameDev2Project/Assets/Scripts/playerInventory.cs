using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class playerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<GameObject> inventory = new List<GameObject>();

    //the following to be used to display with the UI
    public Transform inventoryUIParent;
    public GameObject inventorySlotPrefab;
    public TextMeshProUGUI equippedWeaponText;
    public Image weaponIcon;

    public GameObject equippedWeapon;

    public void AddItem(GameObject item)
    {
        inventory.Add(item);
        Debug.Log("Added item " + item.name);

        if(inventorySlotPrefab != null && inventoryUIParent != null)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIParent);
            TextMeshProUGUI textComponent = slot.GetComponentInChildren<TextMeshProUGUI>();

            if(textComponent != null)
            {
                textComponent.text = item.name;
            }
            else
            {
                Debug.LogWarning("No component found in inventory slot");
            }
        }
        equippedWeapon = item;
        UpdateWeaponUI();
    }

    public void UpdateWeaponUI()
    {
        if (equippedWeapon != null)
        {
            if(equippedWeaponText != null)
            {
                equippedWeaponText.text = equippedWeapon.name;
            }

            if (weaponIcon != null) 
            { 
                var td = equippedWeapon.GetComponent<throwableDamage>();
                if (td != null && td.gun != null && td.gun.weaponIcon != null)
                {
                    weaponIcon.sprite = td.gun.weaponIcon;
                    weaponIcon.enabled = true;
                }
                else weaponIcon.enabled = false;
            }
        }
        else
        {
            if (equippedWeaponText != null)
                equippedWeaponText.text = "No Weapon";

            if (weaponIcon != null)
            {
                weaponIcon.sprite = null;
                weaponIcon.enabled = false;
            }
        }
    }
}
