using UnityEngine;

public class throwPhysics : MonoBehaviour
{
    Transform throwingPosition;
    [SerializeField] int pickUpDis;
    [SerializeField] int throwForce;
    [SerializeField] int pickUpForce;

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
            if (!isHolding)
            {
                TryPickup();
            }
            else
            {
                DropObject();
            }
        }

        if (Input.GetMouseButtonDown(1) && isHolding)
        {
            ThrowObject();
        }

        
        if (isHolding && throwable != null)
        {
            throwable.transform.position = throwingPosition.position;
            throwable.transform.rotation = throwingPosition.rotation;
        }
    }

    void TryPickup()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, pickUpDis))
        {
            if (hit.collider.gameObject.CompareTag("Throwable"))
            {
                throwable = hit.collider.gameObject;
                throwableRb = throwable.GetComponent<Rigidbody>();

                if (throwableRb != null)
                {
                    throwableRb.useGravity = false;
                    throwableRb.isKinematic = true;
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
            if (throwableRb != null)
            {
                throwableRb.isKinematic = false;
                throwableRb.useGravity = true;
            }
            throwable = null;
            throwableRb = null;
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
}