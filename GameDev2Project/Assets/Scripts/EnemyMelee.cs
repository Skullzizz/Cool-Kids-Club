using UnityEngine;

public class EnemyMelee : EnemyAI
{
    [SerializeField] int meleeRange;
    [SerializeField] int meleeDamage;
    [SerializeField] float meleeRate;

    float meleeTimer;

    [SerializeField] bool isPsycho;
    [SerializeField] float enrageMultiplier;
    [SerializeField] int soulsNeeded;
    [SerializeField] float targetScan;  // radius to scan for nearby enemies

    int soulsTaken = 0;
    bool enraged = false;

    Transform currentTarget;
    IDamage currentTargetDamage;

    Transform player;
    IDamage playerDamage;

    

    void attackPlayer()
    {
        IDamage dmgPlayer = gamemanager.instance.player.GetComponent<IDamage>();

        if (dmgPlayer != null)
        {
            dmgPlayer.takeDamage(meleeDamage);
        }
    }

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

                if (isPsycho && currentTarget.TryGetComponent<EnemyMelee>(out EnemyMelee enemy))
                {
                    if (enemy.HP <= 0)
                        takeSoul();
                }
            }
        }
    }
    void UpdateCurrentTarget()
    {
        // Default target is player
        Transform closestTarget = gamemanager.instance.player.transform;
        IDamage closestDamage = gamemanager.instance.player.GetComponent<IDamage>();
        float closestDist = Vector3.Distance(transform.position, closestTarget.position);

        if (isPsycho && enraged)
        {
            // Scan for nearby enemies
            Collider[] hits = Physics.OverlapSphere(transform.position, targetScan);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<EnemyMelee>(out EnemyMelee enemy) && enemy != this)
                {
                    float dist = Vector3.Distance(transform.position, enemy.transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestTarget = enemy.transform;
                        closestDamage = enemy.GetComponent<IDamage>();
                    }
                }
            }
        }

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

        if (isPsycho && !enraged)
        {
            enraged = true;
            meleeRange *= 2;
            meleeDamage = Mathf.RoundToInt(meleeDamage * enrageMultiplier);
            HP = Mathf.RoundToInt(HP * enrageMultiplier);
            transform.localScale *= 2f;
            model.material.color = Color.yellow;
        }
    }

    void takeSoul()
    {
        if (!isPsycho)
            return;

        soulsTaken++;

        if (soulsTaken >= soulsNeeded)
        {
            Enrage();
        }
    }

    void Enrage()
    {
        HP *= 2;
        meleeDamage *= 2;
        transform.localScale *= 1.5f;
        model.material.color = Color.purple;
    }

    protected new void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);

        if (isPsycho && enraged)
        {
            if (other.TryGetComponent<EnemyMelee>(out EnemyMelee enemy) && enemy != this)
            {
                currentTarget = enemy.transform;
                currentTargetDamage = enemy.GetComponent<IDamage>();
            }
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
