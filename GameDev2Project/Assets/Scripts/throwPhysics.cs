using UnityEngine;

public class throwPhysics : MonoBehaviour
{
    [SerializeField] Transform throwingPosition;
    [SerializeField] int pickUpDis;
    [SerializeField] int throwForce;

    // Fields for Equiping Gun - Deven
    [SerializeField] Transform equipPosition;

    public bool isEquiped = false;
    // ---

    GameObject throwable;
    Rigidbody throwableRb;
    bool isHolding = false;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
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

        if (Input.GetMouseButtonDown(1) && isHolding)
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
        
        if (isHolding && throwable != null)
        {
            throwable.transform.position = throwingPosition.position;
            throwable.transform.rotation = throwingPosition.rotation;
        }

        if (isEquiped && throwable != null)
        {
            throwable.transform.position = equipPosition.position;
            throwable.transform.rotation = equipPosition.rotation;
        }

       
    }

    void TryPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickUpDis))
        {
            if (hit.collider.gameObject.CompareTag("throwable"))
            {
                throwable = hit.collider.gameObject;
                throwableRb = throwable.GetComponent<Rigidbody>();

                if (throwableRb != null)
                {
                    throwable.transform.SetParent(throwingPosition);
                    isHolding = true;
                }
            }
        }
    }

    void DropObject()
    {
        if (throwable != null)
        {
            throwable.transform.SetParent(null);
            throwable = null;
            isHolding = false;
        }
    }

    void ThrowObject()
    {
        if (throwable != null)
        {
            DropObject();
            if (throwableRb != null)
            {
                throwableRb.AddForce(Camera.main.transform.forward * throwForce, ForceMode.Impulse);
            }
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

            //Update to Inventory UI
            playerInventory playerInv = gamemanager.instance.playerScript.GetComponent<playerInventory>();
            if (playerInv != null)
            {
                playerInv.equippedWeapon = throwable;
                playerInv.UpdateWeaponUI();
            }

        }
    }

    void UnequipObject()
    {
        if (throwable != null && isEquiped)
        {
            throwable.transform.SetParent(throwingPosition);
            isHolding = true;
            isEquiped = false;

            gamemanager.instance.playerScript.equippedWeapon = null;
            gamemanager.instance.playerScript.shootDamage = 0;
            gamemanager.instance.playerScript.shootRate = 0;
            gamemanager.instance.playerScript.shootDist = 0;
        }
    }

  
}