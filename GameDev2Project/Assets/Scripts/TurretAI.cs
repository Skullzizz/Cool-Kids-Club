using UnityEngine;
using System.Collections;

public class TurretAI : MonoBehaviour, IDamage
{
    public enum TurretState { Idle, Aiming, Attacking, Break, Dead }
    private TurretState currentState;

    [Header("Shoot Stats")]
    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;
    [SerializeField] Transform shootPos;

    float shootTimer;

    [Header("References")]
    [SerializeField] public Renderer model;
    [SerializeField] GameObject turretBase;
    [SerializeField] GameObject elevation;
    [SerializeField] Transform barrelPos;
    [SerializeField] Animator anim;

    [Header("Stats")]
    [SerializeField] public int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] float leftRotLimit;
    [SerializeField] float rightRotLimit;
    [SerializeField] float upRotLimit;
    [SerializeField] float downRotLimit;
    [SerializeField] int FOV;
    [SerializeField] int shootCone;

    Color colorOrig;

    bool playerInTrigger;
    float angleToPlayer;
    bool playerInCone;
    public Vector3 playerDir;
    string Orientation;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (transform.rotation.x != 0)
        {
            Orientation = "Z";
        }
        else if (transform.rotation.z != 0)
        {
            Orientation = "X";
        }
        else
        {
            Orientation = "Y";
        }    
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case TurretState.Idle:
                UpdateIdle();
                break;

            case TurretState.Aiming:
                UpdateAiming(); 
                break;

            case TurretState.Attacking:
                UpdateAttack();
                break;

                case TurretState.Break:
                UpdateBreak();
                break;


        }

        if (gamemanager.instance.player.GetComponent<playerController>().GetHP() <= 0)
        {
            playerInTrigger = false;
        }
    }

    private void UpdateBreak()
    {
        
    }

    private void UpdateAiming()
    {
        faceTarget();

        if (playerInCone)
        {
            ChangeState(TurretState.Attacking);
        }

        if (!playerInTrigger || !canSeePlayer())
        {
            ChangeState(TurretState.Idle);
        }
    }

    void UpdateIdle()
    { 
        if (playerInTrigger && canSeePlayer())
            ChangeState(TurretState.Aiming);
    }

    void UpdateAttack()
    {
        faceTarget();
        shoot();

        if (!playerInCone)
            ChangeState(TurretState.Aiming);
    }
    public virtual void takeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            // trigger aim code

            StartCoroutine(flashRed());
        }
        if (HP <= 0)
        {
            ChangeState(TurretState.Break);

        }
    }

    void ChangeState(TurretState newState)
    {
        currentState = newState;

        switch (newState)
        {
            case TurretState.Idle:

                break;

            case TurretState.Attacking:

                break;

            case TurretState.Dead:

                break;
        }
    }
    void shoot()
    {
        shootTimer += Time.deltaTime;

        if (shootTimer >= shootRate)
        {
            shootTimer = 0;

            Quaternion rot = Quaternion.LookRotation(this.playerDir);
            anim.SetTrigger("Shoot");
            Instantiate(bullet, shootPos.position, rot);
        }
    }

    bool canSeePlayer()
    {
        playerDir = gamemanager.instance.player.transform.position - barrelPos.position;
        angleToPlayer = Vector3.Angle(playerDir, barrelPos.transform.forward);
        Debug.DrawRay(barrelPos.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(barrelPos.position, playerDir, out hit))
        {
            // Enemy can see player!!!!
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {
                return true;
            }
        }

        return false;

    }
    void faceTarget()
    {
        Quaternion rotHorizontal = Quaternion.LookRotation(playerDir);
        Quaternion rotVertical = Quaternion.LookRotation(playerDir);

        if (Orientation == "Y")
        {
            rotHorizontal.x = transform.rotation.x;
            rotHorizontal.z = transform.rotation.z;
        }
        else if (Orientation == "X")
        {
            rotHorizontal.x = transform.rotation.x;
            rotHorizontal.z = transform.rotation.z;
        }
        else if (Orientation == "Z")
        {
            rotHorizontal.x = transform.rotation.x;
            rotHorizontal.z = transform.rotation.z;
        }
            rotVertical.z = turretBase.transform.rotation.z;
        rotVertical.y = turretBase.transform.rotation.y;
        turretBase.transform.rotation = Quaternion.Lerp(turretBase.transform.rotation, rotHorizontal, Time.deltaTime * faceTargetSpeed);
        elevation.transform.rotation = Quaternion.Lerp(elevation.transform.rotation, rotVertical, Time.deltaTime * faceTargetSpeed);
        // Rotate correct parts to aim gun

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
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}
