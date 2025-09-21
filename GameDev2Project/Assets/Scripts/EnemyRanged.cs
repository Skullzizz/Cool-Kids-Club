using UnityEngine;

public class EnemyRanged : EnemyAI
{
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;
    [SerializeField] Animator anime;

 //   [Header("Audio")]
 //   public AudioSource deathAudio;

    float shootTimer;


    void shoot()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootRate)
        {
            shootTimer = 0;

            Vector3 dirPlayer = (gamemanager.instance.player.transform.position - shootPos.position).normalized;

            Quaternion rot = Quaternion.LookRotation(dirPlayer);
            anime.SetTrigger("Shoot");
            Instantiate(bullet, shootPos.position, rot);
        }
    }

    protected override void Attack()
    {
        shoot();
    }
}
