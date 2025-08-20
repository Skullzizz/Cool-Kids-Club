using UnityEngine;
using System.Collections;

public class throwableDamage : MonoBehaviour
{
    [SerializeField] int throwDamage;
    [SerializeField] damageType type;
    [SerializeField] int HP;
    enum damageType { explosive, RAW }

    bool isDamaging;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnImpact()
    {
        if (HP <= 0 && type == damageType.explosive)
        {
            //break item and apply Explosion() and DOT damage
            Explosion();
        }
        else if (HP <= 0 && type == damageType.RAW)
        {
            // break item and apply RAW damage
        }
       
    }

    private void Explosion(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type != damageType.explosive)
        {
            if (!isDamaging)
            {
                StartCoroutine(Damage(dmg));
            }
        }
    }

    private void RAWDMG()
    {

    }

    IEnumerator Damage(IDamage dmg)
    {
        isDamaging = true;
        dmg.takeDamage(throwDamage);
        return;
    }
}
