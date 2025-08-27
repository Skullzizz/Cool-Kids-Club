using UnityEngine;

public class EnemyRanged : EnemyAI
{
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    float shootTimer;

    

    void shoot()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootRate)
        {
            shootTimer = 0;

            Quaternion rot = Quaternion.LookRotation(this.playerDir);

            Instantiate(bullet, shootPos.position, rot);
        }
    }

    protected override void Attack()
    {
        shoot();
    }
}
