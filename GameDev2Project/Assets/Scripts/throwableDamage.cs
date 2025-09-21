using System.Collections;
using UnityEngine;

public class throwableDamage : MonoBehaviour, IThrowable, IDamage, IAmmoSource
{
    [Header("Base Throwable Stats")]
    [SerializeField] int throwDamage;
    [SerializeField] public int throwableHP;
    [SerializeField] int damageRate;
    [SerializeField] float minDMGVel;
    [SerializeField] float knockbackForce;
    [SerializeField] float explKnockMult;
    [SerializeField] public string basePrefabPath;
    public GameObject basePrefab;

    // animator
    [SerializeField] public Animator gunAnimator;

    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    enum damageType { Explosive, RAW }

    [Header("Explosive Stats")]
    [SerializeField] public int maxHits;
    [SerializeField] public float Radius;
    [SerializeField] public int explosiveMaxDamage;
    [SerializeField] public int explosiveMinDamage;
    [SerializeField] public float explosiveForce;
    [SerializeField] ParticleSystem ParticleSystemPrefab;
    public LayerMask targetLayer;
    public LayerMask blockDamageLayer;
    Collider[] hitList;

    [Header("Gun Stats")]
    // Weapon Ammo - Deven
    [SerializeField] public int maxAmmo;
    [SerializeField] public int curAmmo;
    [SerializeField] public gunStats gun;
    // ---

    [SerializeField] private bool startFull = true;
    [SerializeField] private bool isReloading;

    public event System.Action OnAmmoChanged;
    public int CurrentAmmo => curAmmo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => isReloading;

    // Stored Item - Deven
    [SerializeField] GameObject[] storedObjectList;
    [SerializeField] int storedAmount;
    [SerializeField] int spawnForce;
    // ---


    bool isDamaging;
    public bool isInInventory;

    float impactSpeed;
    float velFactor;
    float velDMG;

    void Awake()
    {
        hitList = new Collider[maxHits];

        if (startFull && maxAmmo > 0 && curAmmo <= 0) 
            curAmmo = maxAmmo;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var basePrefabPathCheck = Resources.Load(basePrefabPath, typeof(GameObject));
        if (basePrefabPathCheck != null)
        {
            //Debug.Log("Saved base prefab path object as type " + basePrefabPathCheck.GetType());
        }
        else
        {
            //Debug.Log("Saved base prefab path object as null");
        }
        basePrefab = basePrefabPathCheck as GameObject;
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
                StartCoroutine(Explode());

            }

            //Explosive knockback
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
                StartCoroutine(Explode());

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
        //Debug.Log("Spawning Started");
        GameObject spawnThis = null;
        GameObject spawnedObject = null;
        Vector3 spawnDirection = Vector3.zero;
        Vector3 spawnPosition = gameObject.transform.position;
        for (int spawn = 0; spawn < storedAmount; spawn++)
        {
            spawnThis = storedObjectList[UnityEngine.Random.Range(0, storedObjectList.Length - 1)];
            spawnDirection.y = 1;
            spawnDirection.x = UnityEngine.Random.value;
            spawnDirection.z = UnityEngine.Random.value;
            spawnedObject = Instantiate(spawnThis, spawnPosition, Quaternion.Euler(0, 0, 0));
            spawnedObject.GetComponent<Rigidbody>().AddForce(spawnDirection * spawnForce, ForceMode.Impulse);
            //Debug.Log("Spawned Item: " + spawnedObject);
        }
    }

    IEnumerator spawnItemDelay()
    {
        yield return new WaitForSeconds(0.1f);
        //Debug.Log("Finished Spawn Item Delay");
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

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(0.01f);
        if (ParticleSystemPrefab != null)
            Instantiate(ParticleSystemPrefab, transform.position, Quaternion.identity);
        int hits = Physics.OverlapSphereNonAlloc(transform.position, Radius, hitList, targetLayer);
        MonoBehaviour script;
        IDamage explodeDmg;
        for (int i = 0; i < hits; i++)
        {
            if (hitList[i].gameObject != gameObject)
            {
                //Debug.Log(hitList[i].name + " has been hit with explosive");
                if (hitList[i].TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
                {
                    float distance = Vector3.Distance(transform.position, rigidbody.position);

                    if (!Physics.Raycast(transform.position, (hitList[i].transform.position - transform.position).normalized, distance, blockDamageLayer.value))
                    {
                        rigidbody.AddExplosionForce(explosiveForce, transform.position, Radius);

                    }
                }

                if (hitList[i].TryGetComponent<MonoBehaviour>(out script))
                {
                    float distance = Vector3.Distance(transform.position, script.gameObject.transform.position);
                    //Debug.Log(script.gameObject.name + " testing damage through " + script + " script");
                    explodeDmg = script.GetComponent<IDamage>();
                    explodeDmg.takeDamage((Mathf.FloorToInt(Mathf.Lerp(explosiveMaxDamage, explosiveMinDamage, distance / Radius))));
                }
            }
        }
        Destroy(gameObject);
    }

    public void takeDamage(int amount)
    {
        throwableHP -= amount;

        if (throwableHP <= 0)
        {
            if (storedAmount > 0)
            {
                SpawnStoredItems();
            }

            if (type == damageType.Explosive)
            {
                StartCoroutine(Explode());
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
    public bool Consume(int amount = 1)
    {
        if (MaxAmmo <= 0) return true;
        if (curAmmo <= 0) return false;

        int next = Mathf.Clamp(curAmmo - Mathf.Abs(amount), 0, MaxAmmo);
        if (next != curAmmo)
        {
            curAmmo = next;
            OnAmmoChanged?.Invoke();
        }
        return curAmmo > 0;
       
    }

    public void Reload()
    {
        if(MaxAmmo <= 0)return;

        curAmmo = MaxAmmo;
        isReloading = false;
        OnAmmoChanged?.Invoke();
    }
     public void SetAmmo(int current, int max)
    {
        maxAmmo = Mathf.Max(0, max);
        curAmmo = Mathf.Clamp(current, 0, maxAmmo);
        OnAmmoChanged?.Invoke();
    }


}

[System.Serializable]
public struct ThrowableData
{
    public int throwableHP;
    public int curAmmo;
    public float[] position;
    public float[] rotation;


    public ThrowableData(throwableDamage throwable)
    {
        throwableHP = throwable.throwableHP;
        curAmmo = throwable.curAmmo;

        position = new float[3];
        rotation = new float[3];

        position[0] = throwable.transform.position.x;
        position[1] = throwable.transform.position.y;
        position[2] = throwable.transform.position.z;

        rotation[0] = throwable.transform.rotation.x;
        rotation[1] = throwable.transform.rotation.y;
        rotation[2] = throwable.transform.rotation.z;

    }
}