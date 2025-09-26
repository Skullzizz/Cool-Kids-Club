using System.Collections;
using UnityEngine;

public class throwPhysics : MonoBehaviour
{
    [SerializeField] public Transform handPosition;
    [SerializeField] Transform throwPosition;
    [SerializeField] int pickUpDis;
    [SerializeField] int throwForce;

    [SerializeField] Transform pickupPos;
    [SerializeField] float pickupRadius;

    // Fields for Equiping Gun - Deven
    [SerializeField] Transform equipPosition;

    public bool isEquiped = false;
    // ---

    public GameObject throwable;
    public Rigidbody throwableRb;
    public bool throwableRbDefaultGravity;
    public bool isHolding = false;

    private readonly Collider[] colliders = new Collider[3];
    [SerializeField] private int numberFound;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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

            if (Input.GetMouseButtonDown(1) && isHolding || Input.GetKeyDown(KeyCode.B) && isHolding)
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
                    throwableRb = throwable.GetComponent<Rigidbody>();
                    throwableRbDefaultGravity = true;

                    if (throwableRb != null)
                    {
                        throwable.transform.SetParent(handPosition);
                        throwableRb.useGravity = false;
                        isHolding = true;
                    }
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(pickupPos.position, pickupRadius);
    }

    void TryPickup()
    {
        numberFound = Physics.OverlapSphereNonAlloc(pickupPos.position, pickupRadius, colliders, LayerMask.GetMask("throwable"));
        RaycastHit hit = new RaycastHit();
        float closestToCenter = 1;
        if (numberFound > 0)
        //if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickUpDis))
        {
            for (int i = 0; i < numberFound; i++)
            {
                if (colliders[i] != null)
                {
                    Vector3 throwableDir = colliders[i].transform.position - Camera.main.transform.position;
                    float cameraCentering = Mathf.Abs(Vector3.Dot(Camera.main.transform.InverseTransformDirection(Vector3.forward), throwableDir.normalized));
                    //Debug.Log(cameraCentering);
                    RaycastHit hitCheck;
                    if (Physics.Raycast(Camera.main.transform.position, throwableDir, out hitCheck, LayerMask.GetMask("throwable")))
                    {
                        if (hitCheck.collider.CompareTag("throwable"))
                        {
                            if (cameraCentering < closestToCenter)
                            {
                                closestToCenter = cameraCentering;
                                hit = hitCheck;
                                //Debug.Log(hit.collider.gameObject.name);
                            }
                        }
                    }
                }
            }

            if (hit.collider.gameObject.GetComponent<IThrowable>() != null)
            {
                throwable = hit.collider.gameObject;
                throwableRb = throwable.GetComponent<Rigidbody>();
                throwableRbDefaultGravity = throwableRb.useGravity;

                if (throwableRb != null)
                {
                    if (throwable.GetComponent<throwableDamage>().type == throwableDamage.damageType.Explosive)
                    {
                        throwable.transform.localScale = Vector3.one / 2;
                    }
                    throwable.transform.SetParent(handPosition);
                    throwableRb.useGravity = false;
                    isHolding = true;
                    throwable.GetComponent<throwableDamage>().isHeld = true;
                }
            }
        }
        
        numberFound = 0;
        PickupDelay();
    }

    public void TryPickup(GameObject setThrowable)
    {
        throwable = setThrowable;
        throwableRb = throwable.GetComponent<Rigidbody>();
        throwableRbDefaultGravity = throwableRb.useGravity;

        if (throwableRb != null)
        {
            if (throwable.GetComponent<throwableDamage>().type == throwableDamage.damageType.Explosive)
            {
                throwable.transform.localScale = Vector3.one / 2;
            }
            throwable.transform.SetParent(handPosition);
            throwableRb.useGravity = false;
            isHolding = true;
            throwable.GetComponent<throwableDamage>().isHeld = true;
        }
    }

    void DropObject()
    {
        if (throwable != null)
        {
            isHolding = false;
            throwable.GetComponent<throwableDamage>().isHeld = false;
            if (throwable.GetComponent<throwableDamage>().type == throwableDamage.damageType.Explosive)
            {
                throwable.transform.localScale = Vector3.one;
            }
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

    IEnumerator PickupDelay()
    {
        yield return new WaitForSeconds(0.2f);
    }
}