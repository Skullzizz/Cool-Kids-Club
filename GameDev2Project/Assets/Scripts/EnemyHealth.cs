using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] Image barFill;
    

    public Transform healthBar;
    public Vector3 offset = new Vector3(0,2f,0);

    [SerializeField] private EnemyAI enemyAI;

    private int maxHP = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!enemyAI) enemyAI = GetComponent<EnemyAI>();
        //maxHP = enemyAI.HP;
        UpdateBar();
    }

    void LateUpdate()
    {
        if (healthBar != null)
        {

            healthBar.position = transform.position + offset;
            healthBar.LookAt(Camera.main.transform);
        }
    }
    public void SetHP(int current, int max)
    {
        maxHP = Mathf.Max(1, max);
        
        UpdateBar();
    }

    
    void UpdateBar()
    {
        if (!barFill) return;
        //float ratio = (float)enemyAI.HP / maxHP;
        //barFill.fillAmount = Mathf.Clamp01(ratio);
    }
}
