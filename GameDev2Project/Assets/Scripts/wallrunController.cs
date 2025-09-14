using UnityEngine;

public class wallrunController : MonoBehaviour
{
    public static wallrunController instance;

    // Wallrunning Components
    LayerMask ignoreLayer;
    [SerializeField] public int wallrunGravMod;
    [SerializeField] public int wallrunStartBoost;
    [SerializeField] int wallClimbBoost;
    int wallrunBoostsUsed;
    [SerializeField] int wallBoostMax;
    [SerializeField] Camera cam;
    [SerializeField] float tiltAmt = 15f;
    [SerializeField] float tiltSpeed = 5f;
    [SerializeField] float wallRunFov = 90f;
    [SerializeField] float fovLerpSpeed = 5f;
    float currFov;
    float currTilt = 0f;


    // Detection
    [SerializeField] float wallCheckDist;
    bool wallLeft;
    bool wallRight;
    bool wallClimb;
    public bool isWallRunning;
    float wallrunCooldown = 0.2f;
    float wallrunTimer = 0;
    public bool ableToWallRun = true;

    Collider lastWall;
    Collider currentWall;




    // References
    CharacterController playerMovement;
    [SerializeField] playerController pController;

    float gravOrig;

    private void Awake()
    {
        instance = this;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerMovement = gamemanager.instance.player.GetComponent<CharacterController>();
        gravOrig = pController.gravity;
        wallrunBoostsUsed = 0;
        currFov = cam.fieldOfView;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerMovement.isGrounded)
        {
            wallrunBoostsUsed = 0;
        }
        
        if(wallrunTimer > 0)
            wallrunTimer-=Time.deltaTime;

        detectWall();
        startWallrun();
        wallrunning();
        wallJump();
        startWallClimb();
        cameraTilt();

        Debug.DrawRay(transform.position, transform.right * wallCheckDist, Color.azure);
        Debug.DrawRay(transform.position, transform.right * wallCheckDist * -1, Color.azure);
        Debug.DrawRay(transform.position, transform.forward * wallCheckDist, Color.azure);
    }

    void cameraTilt()
    {
        float tiltDir = 0;
        float fov = currFov;
        if (isWallRunning)
        {
            fov = wallRunFov;
            if (wallLeft)
                tiltDir = -tiltAmt;
            else if(wallRight) tiltDir = tiltAmt;
        }
        currTilt=Mathf.Lerp(currTilt,tiltDir,Time.deltaTime * tiltSpeed);
        cam.transform.localRotation = Quaternion.Euler(cam.transform.localRotation.eulerAngles.x, cam.transform.localRotation.eulerAngles.y, currTilt);
        cam.fieldOfView= Mathf.Lerp(cam.fieldOfView,fov,Time.deltaTime * fovLerpSpeed);
    }

    public void ResetWallrunCamera()
    {
        currTilt = 0f;
        if (cam != null)
            cam.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
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
                currentWall = right.collider;
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
                currentWall = left.collider;
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
        if (!ableToWallRun/*||Water.instance.inWater*/)
        { return; }
            if (Input.GetButtonDown("Jump") && (wallLeft || wallRight) && !playerMovement.isGrounded && !isWallRunning && !pController.isCrouching)
            {
                startWallrunAgain(true);
                Debug.Log("First");
            }
            else if ((wallLeft || wallRight) && !playerMovement.isGrounded && !isWallRunning && !pController.isCrouching && pController.playerVel.y > 0.1f && currentWall != lastWall)
            {
                startWallrunAgain(false);
                lastWall = currentWall;
                Debug.Log("Again");
            }
    }

    void startWallrunAgain(bool jumped)
    {
        isWallRunning = true;
        wallrunTimer = wallrunCooldown;
        if (wallrunBoostsUsed < wallBoostMax)
        {
            wallrunBoostsUsed++;
            pController.gravity /= wallrunGravMod;
            if (jumped)
                pController.playerVel.y = wallClimbBoost;
            else
                pController.playerVel.y = Mathf.Max(pController.playerVel.y, 5f);

            Vector3 forward = transform.forward;
            pController.playerVel += forward * 2f;
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
            else if (pController.isCrouching)
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
                
                pController.playerVel.y = 0 + wallClimbBoost;
            }
        }
    }

    void wallJump()
    {
        if(isWallRunning&&Input.GetButtonDown("Jump")&&wallrunTimer<=0f&&wallrunBoostsUsed<wallBoostMax)
        {
            wallrunBoostsUsed++;
            
            Vector3 jump = transform.up*0.8f;
            if(wallLeft)
            {
                jump += transform.right;
            }
            else if (wallRight)
            {
                jump-=transform.right;
            }
            jump += transform.forward;
            pController.playerVel = jump*wallrunStartBoost;
            wallrunTimer = wallrunCooldown;
        }
    }

}
