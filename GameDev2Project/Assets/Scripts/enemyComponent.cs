using System.Collections;
using UnityEngine;

public class EnemyComponent : MonoBehaviour, IDamage
{
    [SerializeField] EnemyAI enemy;
    [SerializeField] Renderer model;

    Color colorOrig;

    public void takeDamage(int amount)
    {
        enemy.takeDamage(amount);
        StartCoroutine(flashRed());
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
