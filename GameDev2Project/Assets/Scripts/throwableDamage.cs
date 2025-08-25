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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.isTrigger) return;

        // Reduce this object's HP by throwDamage every collision
        throwableHP -= throwDamage;

        IDamage dmg = collision.collider.GetComponent<IDamage>();

        if (dmg != null && type == damageType.RAW)
        {
            if (!isDamaging)
            {
                StartCoroutine(Damage(dmg));
            }

           

            if (throwableHP <= 0)
            {
                if (storedAmount > 0)
                {
                    SpawnStoredItems();
                }
                Destroy(gameObject);
            }
        }
        else if (dmg != null && type == damageType.Explosive)
        {
            if (!isDamaging)
            {
                StartCoroutine(Damage(dmg));
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
        }
    }

    IEnumerator Damage(IDamage dmg)
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
}
