using UnityEngine;

public class throwableDamage : MonoBehaviour
{
    [SerializeField] int throwDamage;
    [SerializeField] bool explode;
    [SerializeField] int throwableHP;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void onImpact()
    {
        if (throwableHP <= 0)
        {
            explode = true;
        }
    }
}
