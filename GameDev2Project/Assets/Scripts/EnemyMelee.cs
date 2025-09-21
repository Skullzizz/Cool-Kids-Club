using UnityEngine;

public class EnemyMelee : EnemyAI
{
    [SerializeField] int meleeRange;
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeRate;

    float meleeTimer;

    Transform currentTarget;
    IDamage currentTargetDamage;

    Transform player;
    //IDamage playerDamage;

    

    //void attackPlayer()
    //{
    //    IDamage dmgPlayer = gamemanager.instance.player.GetComponent<IDamage>();
    //
    //    if (dmgPlayer != null)
    //    {
    //        dmgPlayer.takeDamage(meleeDamage);
    //    }
    //}

    void meleeAttack()
    {
        UpdateCurrentTarget();

        meleeTimer += Time.deltaTime;

        if (meleeTimer >= meleeRate)
        {
            meleeTimer = 0;

            // check if player is within attack range
            if (currentTarget != null && Vector3.Distance(transform.position, gamemanager.instance.player.transform.position) <= meleeRange)
            {
                transform.LookAt(gamemanager.instance.player.transform.position);

                currentTargetDamage?.takeDamage(meleeDamage);

                
            }
        }
    }
    void UpdateCurrentTarget()
    {
        // Default target is player
        Transform closestTarget = gamemanager.instance.player.transform;
        IDamage closestDamage = gamemanager.instance.player.GetComponent<IDamage>();
        float closestDist = Vector3.Distance(transform.position, closestTarget.position);
        

        currentTarget = closestTarget;
        currentTargetDamage = closestDamage;

        if (currentTarget != null)
            agent.SetDestination(currentTarget.position);
    }

    protected override void Attack()
    {
        meleeAttack();
    }

    public override void takeDamage(int amount)
    {
        base.takeDamage(amount);

    }
 

    protected new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        
            if (other.TryGetComponent<EnemyMelee>(out EnemyMelee enemy) && enemy != this)
            {
                currentTarget = enemy.transform;
                currentTargetDamage = enemy.GetComponent<IDamage>();
            }
        
    }

    protected new void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (currentTarget == other.transform)
        {
            currentTarget = gamemanager.instance.player.transform;
            currentTargetDamage = gamemanager.instance.player.GetComponent<IDamage>();
        }
    }
}
