using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class playerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public List<GameObject> inventory = new List<GameObject>();

    //the following to be used to display with the UI
    public Transform inventoryUIParent;
    public GameObject inventorySlotPrefab;
    public TextMeshProUGUI equippedWeaponText;

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
        if (equippedWeapon != null && equippedWeaponText != null)
        {
            equippedWeaponText.text = equippedWeapon.name;
        }
        else
        {
            Debug.LogWarning(" equipped weapon text is missing");
        }
    }
}
