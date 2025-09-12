using UnityEngine;

public class EnemyRanged : EnemyAI
{
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] Animator anime;

    float shootTimer;

    

    void shoot()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootRate)
        {
            shootTimer = 0;

            Quaternion rot = Quaternion.LookRotation(this.playerDir);
            anime.SetTrigger("Shoot");
            Instantiate(bullet, shootPos.position, rot);
        }
    }

    protected override void Attack()
    {
        shoot();
    }
}
