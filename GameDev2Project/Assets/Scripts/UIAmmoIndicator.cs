using TMPro;
using UnityEngine;

public class UIAmmoIndicator : MonoBehaviour
{
    [SerializeField] private playerInventory inventory;
    [SerializeField] private TextMeshProUGUI ammoText;


    private IAmmoSource current;




    void OnEnable()
    {

        if (!inventory) inventory = Object.FindFirstObjectByType<playerInventory>();
        if (!ammoText) ammoText = GetComponentInChildren<TextMeshProUGUI>(true);

        if (inventory)
        {
            inventory.OnEquippedWeaponChanged += HandleEquippedChanged;
            HandleEquippedChanged(inventory.equippedWeapon);
        }
        else
        {
            Debug.LogWarning("[UIAmmoIndicator] No playerInventoryFound");
        }
    }
    void OnDisable()
    {
        if (inventory) inventory.OnEquippedWeaponChanged -= HandleEquippedChanged;
        if (current != null) current.OnAmmoChanged -= UpdateUI;
    }

    void HandleEquippedChanged(GameObject go)
    {
        if (current != null) current.OnAmmoChanged -= UpdateUI;
        current = null;

        Debug.LogWarning("[UIAmmoIndicator] Equipped changed to:" + (go ? go.name : "null"));

        if (go)
        {
            current = go.GetComponent<IAmmoSource>() 
                ?? go.GetComponentInChildren<IAmmoSource>(true)
                ?? go.GetComponentInParent<IAmmoSource>();

            Debug.Log("[UIAmmoIndicator] Found ammo source");

            if (current != null) current.OnAmmoChanged += UpdateUI;
        }
        UpdateUI();
    }

    void UpdateUI()
    {
        if (!ammoText) return;

        if (current == null)
        {
            ammoText.text = "";
            return;
        }
        if (current.MaxAmmo <= 0)
        {
            ammoText.text = "";
            return;
        }

        ammoText.text  = current.IsReloading
            ? "Reloading..."
            : $"{current.CurrentAmmo}/ {current.MaxAmmo}";
    }
}
