using UnityEngine;

public class OctopusArm : MonoBehaviour, IDamage
{
    [SerializeField] OctopusBoss boss;
    [SerializeField] Renderer model;

    public void takeDamage(int amount)
    {
        boss.takeDamage(amount);
        StartCoroutine(boss.flashRed());
    }
}
