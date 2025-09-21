using UnityEngine;
using System.Collections;
using UnityEngine.AI;
using UnityEngine.XR;
using UnityEditor;

public class EnemyAI : MonoBehaviour, IDamage
{
    public enum EnemyState { Idle, Roaming, Chasing, Attacking, Dead }
    private EnemyState currentState;

    [Header("References")]
    [SerializeField] public Renderer model;
    [SerializeField] public NavMeshAgent agent;
    [SerializeField] Transform headPos;
    [SerializeField] Animator anim;
    [SerializeField] public string basePrefabPath;
    public GameObject basePrefab;

    [Header("Stats")]
    [SerializeField] public int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDistance;
    [SerializeField] int roamPauseTime;
    [SerializeField] int animTransSpeed;

  //  [Header("Audio")]
  //  public AudioSource deathAudio;

    [Header("Waypoints")]
    [SerializeField] private Waypoint waypoints;
    [SerializeField] private float waypointThreshold = 0.5f;
    private Transform currentWaypoint;

    bool isRagdolling = false;




    Color colorOrig;

    float roamTimer;
    float angleToPlayer;
    float stoppingDistOrig;

    bool playerInTrigger;
    bool isDead = false;


    public Vector3 playerDir;
    Vector3 startingPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        var basePrefabPathCheck = Resources.Load(basePrefabPath, typeof(GameObject));
        if (basePrefabPathCheck != null)
        {
            Debug.Log("Saved base prefab path object as type " + basePrefabPathCheck.GetType());
        }
        else
        {
            Debug.Log("Saved base prefab path object as null");
        }
        basePrefab = basePrefabPathCheck as GameObject;

        roamTimer = roamPauseTime;



        if (waypoints != null)
            currentWaypoint = waypoints.GetNextWaypoint(null);

     //   // death audio
     //   deathAudio = GetComponent<AudioSource>();
     //   deathAudio.volume = 0.5f; // Set volume between 0.0 and 1.0

    }

    // Update is called once per frame
    void Update()
    {
        setAnimLoco();
        if (!isRagdolling)
        {
            switch (currentState)
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
        }

        if (gamemanager.instance.player.GetComponent<playerController>().GetHP() <= 0)
        {
            playerInTrigger = false;
        }

     //   if (HP <= 0 && !isDead)
     //   {
     //       isDead = true;
     //       StartCoroutine(Die());
     //   }


     //   if (HP <= 0)
     //   {
     //       DeathSound();
     //   }

    }

 //  public void DeathSound()
 //   {
 //       if (deathAudio != null && !deathAudio.isPlaying)
 //       {
 //           deathAudio.Play();
 //           Destroy(gameObject, deathAudio.clip.length); // Wait for sound to finish
 //       }
 //       else
 //       {
 //           Destroy(gameObject); // Fallback
 //       }
 //   }

    void setAnimLoco()
    {
        if (anim == null)
        {
            Debug.LogWarning("The animator for " + this.gameObject.name + " has not been assigned. If it does not have a model yet, this warning should prevent the game from not working till a model is assigned");
            return;
        }
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
        if (waypoints != null)
        {
            if (agent.remainingDistance <= waypointThreshold)
            {
                currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                agent.SetDestination(currentWaypoint.position);
            }
        }
        else
        {

            if (agent.remainingDistance <= 0.1f)
                ChangeState(EnemyState.Idle);
        }

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
        if (waypoints != null)
        {
            agent.stoppingDistance = 0;
            if (currentWaypoint == null)
                currentWaypoint = waypoints.GetNextWaypoint(null);

            agent.SetDestination(currentWaypoint.position);
        }
        else
        {
            Vector3 ranPos = Random.insideUnitSphere * roamDistance;
            ranPos += startingPos;

            NavMeshHit hit;
            NavMesh.SamplePosition(ranPos, out hit, roamDistance, 1);
            agent.SetDestination(hit.position);
        }
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
        Vector3 dir = gamemanager.instance.player.transform.position - headPos.position;
        Quaternion rot = Quaternion.LookRotation(dir);
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

            if (GetComponent<RagdollController>() != null)
            {
                isRagdolling = true;
                GetComponent<RagdollController>().ActivateRagdoll();
                Destroy(gameObject, 5f);
            }
            else
            {
                Destroy(gameObject);
            }

            //Ragdoll Physics
            gamemanager.instance.updateEnemyDeaths(1);
        }
    }
  //  IEnumerator Die()
  //  {
  //      deathAudio.Play();
  //      yield return new WaitForSeconds(deathAudio.clip.length);
  //      Destroy(gameObject);
  //  }



    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
