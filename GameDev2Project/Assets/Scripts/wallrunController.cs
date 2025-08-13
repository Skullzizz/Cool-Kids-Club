using UnityEngine;

public class wallrunController : MonoBehaviour
{
    // Wallrunning Components
    LayerMask ignoreLayer;
    [SerializeField] public int wallrunGravMod;
    [SerializeField] public int wallrunStartBoost;
    [SerializeField] int wallClimbBoost;
    int wallrunBoostsUsed;
    [SerializeField] int wallBoostMax;
    [SerializeField] float cameraTilt;

    // Detection
    [SerializeField] float wallCheckDist;
    bool wallLeft;
    bool wallRight;
    bool wallClimb;
    public bool isWallRunning;

    // References
    CharacterController playerMovement;
    [SerializeField] playerController pController;

    float gravOrig;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = gamemanager.instance.player.GetComponent<CharacterController>();
        gravOrig = pController.gravity;
        wallrunBoostsUsed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.isGrounded)
        {
            wallrunBoostsUsed = 0;
        }

        detectWall();
        startWallrun();
        wallrunning();
        startWallClimb();

        Debug.DrawRay(transform.position, transform.right * wallCheckDist, Color.azure);
        Debug.DrawRay(transform.position, transform.right * wallCheckDist * -1, Color.azure);
        Debug.DrawRay(transform.position, transform.forward * wallCheckDist, Color.azure);
    }

    // Detect Wall
    void detectWall()
    {
        RaycastHit right;
        RaycastHit left;
        RaycastHit forward;

        if (Physics.Raycast(transform.position, transform.right, out right, wallCheckDist, ~ignoreLayer))
        {
            if (right.collider.tag == "RunnableWall")
            {
                wallRight = true;
            }
            else
            {
                wallRight = false;
            }
            
        }
        else
        {
            wallRight= false;
        }


        if (Physics.Raycast(transform.position, transform.right * -1, out left, wallCheckDist, ~ignoreLayer))
        {
            if (left.collider.tag == "RunnableWall")
            {
                wallLeft = true;
            }
            else
            {
                wallLeft = false;
            }
           
        }
        else
        {
            wallLeft = false;
        }


        if (Physics.Raycast(transform.position, transform.forward, out forward, wallCheckDist, ~ignoreLayer))
        {
            if (forward.collider.tag == "RunnableWall")
            {
                wallClimb = true;
            }
            else
            {
                wallClimb = false;
            }
           
        }
        else
        {
            wallClimb = false;
        }
    }

    // Start Wallrunning
    void startWallrun()
    {
        if (Input.GetButtonDown("Jump") && (wallLeft || wallRight) && !playerMovement.isGrounded && !isWallRunning)
        {
            isWallRunning = true;
            if (wallrunBoostsUsed < wallBoostMax)
            {
                wallrunBoostsUsed += 1;
                pController.gravity /= wallrunGravMod;
                pController.playerVel.y = 0 + wallClimbBoost;
            }
        }
    }

    void wallrunning()
    {
        if (isWallRunning) 
        {
            if (!wallRight && !wallLeft)
            {
                endWallrun();
            }
            else if (playerMovement.isGrounded)
            {
                endWallrun();
            }
        }
    }

    void endWallrun()
    {
        pController.gravity = gravOrig;
        isWallRunning = false;
    }

    // Start Wallclimbing

    void startWallClimb()
    {
        if (wallClimb && !playerMovement.isGrounded && !wallLeft && !wallRight)
        {
            if (Input.GetButtonDown("Jump"))
            {
                wallrunBoostsUsed += 1;
                pController.playerVel.y = 0 + wallClimbBoost;
            }
        }
    }


}
