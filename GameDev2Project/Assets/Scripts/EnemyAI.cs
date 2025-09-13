using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] public Renderer model;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;

    [SerializeField] public int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDistance;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTransSpeed;

  

    Color colorOrig;

    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    bool playerInTrigger;

    public Vector3 playerDir;
    Vector3 startingPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;

        roamTimer = roamPauseTime;
    }

    // Update is called once per frame
    void Update()
    {
        setAnimLoco();


        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;

        if (playerInTrigger && !canSeePlayer())
        {
            checkRoam();
        }
        else if (!playerInTrigger)
        {
            checkRoam();
        }

        if (gamemanager.instance.player.GetComponent<playerController>().GetHP() <= 0)
        {
            playerInTrigger = false;
        }
    }

    void setAnimLoco()
    {
        float agentSpeedCur = agent.velocity.normalized.magnitude;
        float animSpeedCurr = anim.GetFloat("Speed");

        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCurr,agentSpeedCur,Time.deltaTime*animTransSpeed));

    }

    void checkRoam()
    {
        if (roamTimer >= roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }

    }

    void roam()
    {
        roamTimer = 0;
        agent.stoppingDistance = 0;

        Vector3 ranPos = Random.insideUnitSphere * roamDistance;
        ranPos += startingPos;

        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDistance, 1);
        agent.SetDestination(hit.position);
        
    }

    protected virtual void Attack()
    {
        // ranged or melee will have there own attack method
    }

    bool canSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);
        Debug.DrawRay(headPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPos.position, playerDir, out hit))
        {
            // Enemy can see player!!!!
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                agent.SetDestination(gamemanager.instance.player.transform.position);
                    Attack();
                

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    faceTarget();
                }

                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }

        agent.stoppingDistance = 0;
        return false;

    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            agent.stoppingDistance = 0;
        }
    }

    

    public virtual void takeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            agent.SetDestination(gamemanager.instance.player.transform.position);
            StartCoroutine(flashRed());
        }
        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            gamemanager.instance.updateEnemyDeaths(1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
