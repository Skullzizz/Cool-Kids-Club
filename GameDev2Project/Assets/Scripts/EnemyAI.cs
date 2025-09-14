using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.XR;

public class EnemyAI : MonoBehaviour, IDamage
{
    public enum EnemyState { Idle, Roaming, Chasing, Attacking, Dead }
    private EnemyState currentState;

    [Header("References")]
    [SerializeField] public Renderer model;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;

    [Header("Stats")]
    [SerializeField] public int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDistance;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTransSpeed;


    private RagdollToggle ragdollToggle;


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


        ragdollToggle = GetComponent<RagdollToggle>();

        roamTimer = roamPauseTime;

    }

    // Update is called once per frame
    void Update()
    {
        setAnimLoco();

        switch(currentState)
        {
            case EnemyState.Idle:
                UpdateIdle();
                break;

            case EnemyState.Roaming:
                UpdateRoam();
                break;

            case EnemyState.Chasing:
                UpdateChase();
                break;

            case EnemyState.Attacking:
                UpdateAttack();
                break;

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

        anim.SetFloat("Speed", Mathf.Lerp(animSpeedCurr, agentSpeedCur, Time.deltaTime * animTransSpeed));

    }

    // STATE LOGIC

    void UpdateIdle()
    {
        roamTimer += Time.deltaTime;
        if (roamTimer >= roamPauseTime)
            ChangeState(EnemyState.Roaming);

        if (playerInTrigger && canSeePlayer())
            ChangeState(EnemyState.Chasing);
    }

    void UpdateRoam()
    {
        if (agent.remainingDistance <= 0.1f)
            ChangeState(EnemyState.Idle);

        if (playerInTrigger && canSeePlayer())
            ChangeState(EnemyState.Chasing);
    }

    void UpdateChase()
    {
        agent.SetDestination(gamemanager.instance.player.transform.position);

        if (agent.remainingDistance <= agent.stoppingDistance + 0.5f)
            ChangeState(EnemyState.Attacking);
        else if (!canSeePlayer())
            ChangeState(EnemyState.Idle);
    }

    void UpdateAttack()
    {
        faceTarget();
        Attack();

        if (agent.remainingDistance > agent.stoppingDistance + 0.5f)
            ChangeState(EnemyState.Chasing);
    }

    void ChangeState(EnemyState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case EnemyState.Idle:
                roamTimer = 0;
                agent.stoppingDistance = 0;
                agent.ResetPath();
                break;

            case EnemyState.Roaming:
                roamTimer = 0;
                roam();
                break;

            case EnemyState.Chasing:
                agent.stoppingDistance = stoppingDistOrig;
                break;

            case EnemyState.Attacking:
                agent.stoppingDistance = stoppingDistOrig;
                break;

            case EnemyState.Dead:
                Destroy(gameObject);
                break;
        }
    }

    void roam()
    {
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


            //Ragdoll Physics
            ragdollToggle.ToggleRagdoll(true);
            Rigidbody hipsRigidbody = GetComponentInChildren<Rigidbody>();
            if (hipsRigidbody != null)
            {
                hipsRigidbody.AddForce(Vector3.up * 5, ForceMode.Impulse);
            }
            Destroy(gameObject, 5f);

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
