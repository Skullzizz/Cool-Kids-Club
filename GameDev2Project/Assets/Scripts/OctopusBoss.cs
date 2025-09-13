using System.Collections;
using UnityEngine;

public class OctopusBoss : MonoBehaviour, IDamage
{
    

    public Transform[] legs = new Transform[8];


    [SerializeField] public int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] public Renderer model;
    [SerializeField] Transform headPos;
    [SerializeField] float grappleTime;
    [SerializeField] float grappleObjectTime;
    [SerializeField] int octoThrowForce;
    Color colorOrig;
    public Vector3 playerDir;
    [SerializeField] GameObject armColliders;

    [SerializeField] float actionCooldown = 2f;
    bool isOnCooldown = false;
    bool isActing = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        colorOrig = model.material.color;
        gamemanager.instance.updateGameGoal(1);
        
    }

    // Update is called once per frame
    void Update()
    {
        canSeePlayer();

        octoAction();
    }

    void octoAction()
    {
        if (!isActing && !isOnCooldown)
        {
            int choice = Random.Range(0, 2);
            if (choice == 0)
                StartCoroutine(searchRescueAttack());
            else
                StartCoroutine(grapple());
        }
    }

    IEnumerator cooldownAction()
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(actionCooldown);
        isOnCooldown = false;
    }

    public virtual void takeDamage(int amount)
    {
        if (HP > 0)
        {
            HP -= amount;
            StartCoroutine(flashRed());
        }
        if (HP <= 0)
        {
            gamemanager.instance.updateGameGoal(-1);
            gamemanager.instance.updateEnemyDeaths(1);
            Destroy(gameObject);
        }
    }

    public IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }

    bool canSeePlayer()
    {
            playerDir = gamemanager.instance.player.transform.position - headPos.position;
            playerDir.x *= -1;
            playerDir.y *= -1;
            playerDir.z *= -1;
            Debug.DrawRay(headPos.position, playerDir);
            faceTarget();
        return true;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDir);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);

    }

    IEnumerator grapple()
    {
        isActing = true;
        
            Transform leg = randomLeg();
            int angleVert = Random.Range(-45, 45);
            int angleHori = Random.Range(-30, 30);
            Vector3 legDir = leg.up;

            Quaternion rot = Quaternion.Euler(angleVert, angleHori, 0);

            Vector3 grappleDir = rot * legDir;

            RaycastHit hit;
            Vector3 octoPos = transform.position;
            if (Physics.Raycast(leg.position, grappleDir, out hit, Mathf.Infinity))
            {
                Vector3 grapplePos = hit.transform.position;
                if (grapplePos.x > 37)
                    grapplePos.x = 37;
                if (grapplePos.x < -17)
                    grapplePos.x = -17;
                if (grapplePos.y < 2)
                    grapplePos.y = 2;
                if (grapplePos.y > 20)
                    grapplePos.y = 20;
                if (grapplePos.z > 120)
                    grapplePos.z = 120;
                if (grapplePos.z > 62)
                    grapplePos.z = 62;

                Debug.DrawRay(leg.position, grappleDir * hit.distance, Color.yellow);
                Debug.Log(grapplePos);
                float timeGrapple = 0f;
                while (timeGrapple < grappleTime)
                {
                    transform.position = Vector3.Lerp(octoPos, grapplePos, timeGrapple / grappleTime);
                    timeGrapple += Time.deltaTime;
                    yield return null;
                }
            }
            isActing = false;
            StartCoroutine(cooldownAction());
    }

    IEnumerator searchRescueAttack()
    {
        isActing = true;
       
        LayerMask mask = LayerMask.GetMask("throwable");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 100, mask);
        if (hitColliders.Length == 0)
        {

            isActing = false;
            yield break;
        }

        
        float bestDist = float.MaxValue;
        int bestLeg = 0;
        int bestObj = 0;
        for (int j = 0; j < hitColliders.Length; ++j)
        {
            for (int i = 0; i < legs.Length; ++i)
            {
                float closeDist = (legs[i].position - hitColliders[j].transform.position).sqrMagnitude;
                if (closeDist < bestDist)
                {
                    bestDist = closeDist;
                    bestLeg = i;
                    bestObj = j;
                }
            }
        }

        Collider target = hitColliders[bestObj];
        if (target == null) 
        { 
            ResetGrabState(); 
            yield break; 
        }

        target.tag = "octopusThrow";
        Rigidbody objRb = target.GetComponent<Rigidbody>();
        objRb.isKinematic = true;

        
        Vector3 objPos = target.transform.position;
        float timeGrappleObject = 0f;
        while (timeGrappleObject < grappleObjectTime)
        {
            if (target == null) 
            { 
                ResetGrabState(); 
                yield break; 
            }
            target.transform.position = Vector3.Lerp(objPos, legs[bestLeg].position, timeGrappleObject / grappleObjectTime);
            timeGrappleObject += Time.deltaTime;
            yield return null;
        }

       
        if (target == null) 
        { 
            ResetGrabState(); 
            yield break; 
        }

        target.transform.SetParent(legs[bestLeg], true);

       
        yield return new WaitForSeconds(Random.Range(2, 5));

        if (target == null) { ResetGrabState(); yield break; }
        target.transform.SetParent(null, true);

        target.tag = "throwable";
        objRb.isKinematic = false;

        Vector3 throwDir = (gamemanager.instance.player.transform.position - target.transform.position);
        throwDir.y += 2.5f;
        armColliders.SetActive(false);
        objRb.AddForce(throwDir * octoThrowForce, ForceMode.Impulse);

        ResetGrabState();
        yield return new WaitForSeconds(0.5f);
        armColliders.SetActive(true);
    }

    void ResetGrabState()
    {
        isActing = false;
        StartCoroutine(cooldownAction());
    }

    Transform randomLeg()
    {
        int num = Random.Range(0, 8);
        return legs[num];
    }
}
