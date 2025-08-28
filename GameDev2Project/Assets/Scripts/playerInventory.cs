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
    public Sprite noWeaponIcon;

    [Header("UI Colors")]
    public Color defaultEmptyColor = Color.red;
    public Color normalColor = Color.white;

    public GameObject equippedWeapon;
    public int equippedWeaponIndex = -1;


    private void Start()
    {
        weaponIcon = gamemanager.instance.WeaponIcon;
        equippedWeaponText = gamemanager.instance.storedWeaponText;
        UpdateWeaponUI();
    }

    private void Update()
    {
        selectGun();
    }
    public void AddItem(GameObject item)
    {
        inventory.Add(item);
        Debug.Log("Added item " + item.name);

        if (inventorySlotPrefab != null && inventoryUIParent != null)
        {
            GameObject slot = Instantiate(inventorySlotPrefab, inventoryUIParent);
            TextMeshProUGUI textComponent = slot.GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent != null)
            {
                textComponent.text = item.name;
            }
            else
            {
                Debug.LogWarning("No component found in inventory slot");
            }
        }
        equippedWeapon = item;
        equippedWeaponIndex = inventory.Count - 1;
        UpdateWeaponUI();
    }

    public void RemoveItem()
    {
        if (equippedWeapon != null)
        {
            inventory.Remove(equippedWeapon);
            equippedWeaponIndex -= 1;
            UpdateWeaponUI();
            if (inventory[equippedWeaponIndex] != null)
            {
                equippedWeapon = inventory[equippedWeaponIndex];
            }
            else
            {
                equippedWeapon = null;
            }
            
        }
    }

    void selectGun()
    {
        if (inventory.Count == 0) return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            equippedWeaponIndex++;
            if (equippedWeaponIndex >= inventory.Count)
                equippedWeaponIndex = 0;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            equippedWeaponIndex--;
            if (equippedWeaponIndex < 0)
                equippedWeaponIndex = inventory.Count - 1;
        }

        if (equippedWeaponIndex >= 0 && equippedWeaponIndex < inventory.Count)
        {
            equippedWeapon = inventory[equippedWeaponIndex];
            UpdateWeaponUI();
        }
    }

    public void UpdateWeaponUI()
    {
        if (equippedWeapon != null && equippedWeaponIndex >= 0 && equippedWeaponIndex < inventory.Count)
        {
            if (equippedWeaponText != null)
                equippedWeaponText.text = equippedWeapon.name;


            if (weaponIcon != null)
            {
                var td = equippedWeapon.GetComponent<throwableDamage>();
                if (td != null && td.gun != null && td.gun.weaponIcon != null)
                {
                    weaponIcon.sprite = td.gun.weaponIcon;
                    weaponIcon.color = normalColor;
                    weaponIcon.enabled = true;
                }
                else
                {
                    weaponIcon.sprite = noWeaponIcon;
                    weaponIcon.color = defaultEmptyColor;
                    weaponIcon.enabled = true;
                }
            }
        }
        else
        {
            if (equippedWeaponText != null)
                equippedWeaponText.text = "No Weapon";

            if (weaponIcon != null)
            {
                weaponIcon.sprite = noWeaponIcon;
                weaponIcon.color = defaultEmptyColor;
                weaponIcon.enabled = true;
            }
        }
    }
}
