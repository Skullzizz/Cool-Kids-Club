using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] Image barFill;
    [SerializeField] int maxHP = 50; int hp;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hp = maxHP;
        UpdateBar();
    }

    public void SetHP(int current, int max)
    {
        maxHP = Mathf.Max(1, max);
        hp = Mathf.Clamp(current, 0 , maxHP);
        UpdateBar();
    }

    public void ApplyDamage(int amount)
    {
        hp = Mathf.Max(0, hp - amount);
        UpdateBar() ;
    }
    void UpdateBar()
    {
        if (!barFill) return;
        float ratio = (float)hp / maxHP;
        barFill.fillAmount = Mathf.Clamp01(ratio);
    }
}
