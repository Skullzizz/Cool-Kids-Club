using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class throwableDamage : MonoBehaviour
{
    [SerializeField] int throwDamage;
    [SerializeField] int throwableHP;
    [SerializeField] int damageRate;
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;
    enum damageType { Explosive, RAW }

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
}
