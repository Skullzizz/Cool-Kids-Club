using UnityEngine;

public class throwPhysics : MonoBehaviour
{
    [SerializeField] Transform handPosition;
    [SerializeField] Transform throwPosition;
    [SerializeField] int pickUpDis;
    [SerializeField] int throwForce;

    // Fields for Equiping Gun - Deven
    [SerializeField] Transform equipPosition;

    public bool isEquiped = false;
    // ---

    public GameObject throwable;
    Rigidbody throwableRb;
    bool throwableRbDefaultGravity;
    bool isHolding = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        throwable = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (!gamemanager.instance.isPaused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (!isHolding && !isEquiped)
                {
                    TryPickup();
                }
                else if (!isEquiped)
                {
                    DropObject();
                }
            }

            if (Input.GetMouseButtonDown(1) && isHolding|| Input.GetKeyDown(KeyCode.B) && isHolding)
            {
                ThrowObject();
            }

            if (Input.GetButtonDown("Equip"))
            {
                if (isHolding)
                {
                    TryEquipObject();
                }
                else if (isEquiped)
                {
                    UnequipObject();
                }
            }

            if (Input.GetButtonDown("Store"))
            {
                if (isHolding && gamemanager.instance.playerInventory.equippedWeapon == null)
                {
                    //Update to Inventory UI
                    playerInventory playerInv = gamemanager.instance.playerInventory;
                    if (playerInv != null)
                    {
                        playerInv.AddItem(throwable);

                        playerInv.equippedWeapon = throwable;
                        playerInv.equippedWeaponIndex = playerInv.inventory.IndexOf(throwable);
                        throwable.SetActive(false);
                        throwable = null;
                        isHolding = false;
                        playerInv.UpdateWeaponUI();
                    }
                }
                else if (!isHolding && !isEquiped && gamemanager.instance.playerInventory.equippedWeapon != null)
                {
                    throwable = gamemanager.instance.playerInventory.equippedWeapon;
                    throwable.SetActive(true);
                    isHolding = true;
                    gamemanager.instance.playerInventory.RemoveItem();
                }

            }

            if (isHolding && throwable != null)
            {
                throwable.transform.position = handPosition.position;
                throwable.transform.rotation = handPosition.rotation;
            }

            if (isEquiped && throwable != null)
            {
                throwable.transform.position = equipPosition.position;
                throwable.transform.rotation = equipPosition.rotation;
            }
        }

    }

    void TryPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickUpDis))
        {
            if (hit.collider.gameObject.GetComponent<IThrowable>() != null)
            {
                throwable = hit.collider.gameObject;
                throwableRb = throwable.GetComponent<Rigidbody>();
                throwableRbDefaultGravity = throwableRb.useGravity;

                if (throwableRb != null)
                {
                    throwable.transform.SetParent(handPosition);
                    throwableRb.useGravity = false;
                    isHolding = true;
                }
            }
        }
    }

    void DropObject()
    {
        if (throwable != null)
        {
            isHolding = false;

            throwable.transform.SetParent(null);
            throwable.transform.position = throwPosition.position;
            throwable = null;
            throwableRb.useGravity = throwableRbDefaultGravity;
        }
    }

    void ThrowObject()
    {
            if (throwableRb != null)
            {
            DropObject();
            throwableRb.AddForce(throwableRb.transform.forward * throwForce, ForceMode.Impulse);
            }
            
    }


    void TryEquipObject()
    {
        if (throwable != null && throwable.GetComponent<throwableDamage>().gun != null && !isEquiped)
        {
            throwable.transform.SetParent(equipPosition);
            isHolding = false;
            isEquiped = true;

            gamemanager.instance.playerScript.equippedWeapon = throwable.GetComponent<throwableDamage>();
            gamemanager.instance.playerScript.shootDamage = throwable.GetComponent<throwableDamage>().gun.shootDamage;
            gamemanager.instance.playerScript.shootRate = throwable.GetComponent<throwableDamage>().gun.shootRate;
            gamemanager.instance.playerScript.shootDist = throwable.GetComponent<throwableDamage>().gun.shootDist;

        }
    }

    void StoreItem()
    {

    }

    void UnequipObject()
    {
        if (throwable != null && isEquiped)
        {
            throwable.transform.SetParent(handPosition);
            isHolding = true;
            isEquiped = false;

            gamemanager.instance.playerScript.shootDamage = 0;
            gamemanager.instance.playerScript.shootRate = 0;
            gamemanager.instance.playerScript.shootDist = 0;
            gamemanager.instance.playerScript.equippedWeapon = null;


        }
    }


}