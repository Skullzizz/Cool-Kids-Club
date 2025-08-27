using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using NUnit.Framework;

public class throwableDamage : MonoBehaviour
{
    [SerializeField] int throwDamage;
    [SerializeField] int throwableHP;
    [SerializeField] int damageRate;
    [SerializeField] float minDMGVel;
    [SerializeField] float knockbackForce;
    [SerializeField] float explKnockMult;

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    enum damageType { Explosive, RAW }

    
    // Weapon Ammo - Deven
    [SerializeField] public int maxAmmo;
    [SerializeField] public int curAmmo;
    [SerializeField] public gunStats gun;
    // ---

    // Stored Item - Deven
    [SerializeField] GameObject[] storedObjectList;
    [SerializeField] int storedAmount;
    [SerializeField] int spawnForce;
    // ---


    bool isDamaging;

    float impactSpeed;
    float velFactor;
    float velDMG;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        VisualClipping();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.isTrigger || gamemanager.instance.throwScript.throwable == gameObject)
        {
            return;
        }

        // Velocity damage scaler
        velFactor = impactSpeed / minDMGVel;
        velDMG = throwDamage * velFactor;

        // Velocity check
        impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < minDMGVel)
        {
            return;
        }
        else
        {
            // Reduce object HP on collisions with enough velocity to deal damage
            throwableHP -= throwDamage;
        }

        // Reduce this object's HP by throwDamage every collision
        // throwableHP -= throwDamage;

        IDamage dmg = collision.collider.GetComponent<IDamage>();

        if (dmg != null && type == damageType.RAW)
        {
            if (!isDamaging)
            {
                StartCoroutine(Damage(dmg, velDMG));
            }

            if (throwableHP <= 0)
            {
                if (storedAmount > 0)
                {
                    SpawnStoredItems();
                }
                Destroy(gameObject);
            }

            // Knockback
            Rigidbody targetRb = collision.collider.attachedRigidbody;
            if (targetRb != null)
            {
                float knockback = knockbackForce * velFactor;
                if (type == damageType.RAW)
                {
                    Vector3 knockbackDir = (collision.collider.transform.position - transform.position).normalized;
                    targetRb.AddForce(knockbackDir * knockbackForce * velFactor, ForceMode.Impulse);
                }
            }
        }
        else if (dmg != null && type == damageType.Explosive)
        {
            if (!isDamaging)
            {
                StartCoroutine(Damage(dmg, velDMG));
            }

            if (throwableHP <= 0)
            {
                throwDamage *= 2;
                if (storedAmount > 0)
                {
                    SpawnStoredItems();
                }
                Destroy(gameObject);
            }

            // Explosive knockback
            Rigidbody targetRb = collision.collider.attachedRigidbody;
            if (targetRb != null)
            {

                float knockback = knockbackForce * velFactor * explKnockMult;

                Vector3 knockbackDir = (collision.collider.transform.position - transform.position).normalized;
                targetRb.AddForce(knockbackDir * knockback, ForceMode.Impulse);
            }
        }
        else if (dmg == null)
        {
            if (throwableHP <= 0)
            {
                if (storedAmount > 0)
                {
                    SpawnStoredItems();
                }
                Destroy(gameObject);

            }
        }
    }

    IEnumerator Damage(IDamage dmg, float velDMG)
    {
        isDamaging = true;
        dmg.takeDamage(throwDamage);
        yield return new WaitForSeconds(damageRate);
        isDamaging = false;
    }

    // Spawn Items from Storage
    void SpawnStoredItems()
    {
        StartCoroutine(spawnItemDelay());
        Debug.Log("Spawning Started");
        GameObject spawnThis = null;
        GameObject spawnedObject = null;
        Vector3 spawnDirection = Vector3.zero;
        Vector3 spawnPosition = gameObject.transform.position;
        for (int spawn = 0; spawn < storedAmount; spawn++)
        {
            spawnThis = storedObjectList[Random.Range(0, storedObjectList.Length - 1)];
            spawnDirection.y = 1;
            spawnDirection.x = Random.value;
            spawnDirection.z = Random.value;
            spawnedObject = Instantiate(spawnThis, spawnPosition, Quaternion.Euler(0,0,0));
            spawnedObject.GetComponent<Rigidbody>().AddForce(spawnDirection * spawnForce, ForceMode.Impulse);
            Debug.Log("Spawned Item: " + spawnedObject);
        }
    }

    IEnumerator spawnItemDelay()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Finished Spawn Item Delay");
    }

    void VisualClipping()
    {
        if (gamemanager.instance.throwScript.throwable == gameObject)
        {
            gameObject.layer = 12;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = 12;
                foreach (Transform subchild in child.transform)
                {
                    subchild.gameObject.layer = 12;
                }
            }
        }
        else
        {
            gameObject.layer = 10;
            foreach (Transform child in gameObject.transform)
            {
                child.gameObject.layer = 10;
                foreach (Transform subchild in child.transform)
                {
                    subchild.gameObject.layer = 10;
                }
            }
        }
    }
}
