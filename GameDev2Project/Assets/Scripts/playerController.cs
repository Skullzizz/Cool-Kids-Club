using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class playerController : MonoBehaviour, IDamage
{

    [SerializeField] LayerMask ignorelayer;

    [Header("Controllers")]
    [SerializeField] CharacterController controller;
    [SerializeField] wallrunController wallrunController;
    public Rigidbody rb;
   


    [Header("Player Statistics")]
    [SerializeField] public int HP;
    [SerializeField] public int speed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchHeight;
    [SerializeField] float slideBoost;
    [SerializeField] float airSlideBoost;
    [SerializeField] float slideFriction;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] public int jumpMax;
    [SerializeField] float airControlMod;
    [SerializeField] public float gravity;
    [SerializeField] float coyoteTimeMax;
    float lastGroundedTime;
    int jumpCount;

    [Header("Player Combat Statistics")]
    [SerializeField] public int shootDamage;
    [SerializeField] public float shootRate;
    [SerializeField] public int shootDist;
    float shootTimer;


    [Header("Throwable Management")]

    public throwableDamage equippedWeapon;
    public bool isEquipping = false;
    public float equipDuration = 0.5f;

    [Header("Movement Controls")]
    Vector3 moveDir;
    public Vector3 playerVel;
    private Vector3 lastPosition;

    [Header("Originals")]
    int HPOrig;
    float heightOrig;

    [Header("General Bools")]
    public bool isSprinting;
    public bool isCrouching;
    public bool isSliding;

    private bool isMoving;
    private bool wasMoving;

    public bool isGrappling;
    public bool activeGrapple;
    public bool hasShield;

    [Header("Shield Systems")]
    public int shieldCharge;
    [SerializeField] int maxShield = 100;
    [SerializeField] int shieldRegenRate = 5;
    [SerializeField] float shieldRegenInterval = 0.2f;
    [SerializeField] float shieldChargeCooldown = 3f;
    float lastHitTime;
    float regenTimer;
    public bool locked = false;

    [SerializeField] Image edgeBleedOverlay;
    [SerializeField] float lowHealthThreshold = 0.40f; //when bleed appears %
    [SerializeField] float maxBleed = 0.55f;
    [SerializeField] float pulseSpeed = 2f;
 
    bool deathCoroutineRun = false;
    bool isDead;

    Camera minimapCam;

    [Header("Save/Load Refs")]
    GameObject playerSpawnPointRef;

    [Header("Audio")]
  //  public AudioSource deathAudio;
    public AudioSource walkAudio;

    [Header("Running FOV")]
    public Camera playerCamera;
    public float normalFOV = 60f;
    public float runningFOV = 90f;
    public float fovTransitionSpeed = 5f;

 //   [Header("Head Bob")]
 //
 //   public Transform cameraTransform;
 //   public float bobFrequency = 1.5f;
 //   public float bobAmplitude = 1f;
 //   public float bobSpeedMultiplier = 1f;
 //   private float bobTimer = 0f;
 //   private Vector3 initialCameraPosition;




    public enum PlayerStats
    {
        Health,
        Speed,
        JumpMax
    }

    void Awake()
    {
        playerSpawnPointRef = gamemanager.instance.playerSpawnPos;
        HPOrig = HP;
        heightOrig = controller.height;
        minimapCam = GameObject.FindWithTag("MinimapCam").GetComponent<Camera>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        gamemanager.instance.updateEnemyDeaths(0);
        
        updatePlayerUI();

        if (edgeBleedOverlay == null)
        {
            var go = GameObject.FindWithTag("EdgeBleedUI");
            if(go != null) edgeBleedOverlay = go.GetComponent<Image>();
        }

        if (edgeBleedOverlay != null)
        {
            SetBleedAlpha(0f);
        }

        // Apart of the movement check for audio
        lastPosition = transform.position;

        // Camera Bob
        //     initialCameraPosition = cameraTransform.localPosition;

      //  // death audio
      //  deathAudio = GetComponent<AudioSource>();
      //  deathAudio.volume = 0.5f; // Set volume between 0.0 and 1.0

    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            movement();
            sprint();
            crouch();
        }

        // Running FOV
        float targetFOV = isSprinting ? runningFOV : normalFOV;
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, Time.deltaTime * fovTransitionSpeed);



            Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);

        ShieldRecharge();

        if(edgeBleedOverlay != null)
        {
            float healthPct = Mathf.Clamp01((float)HP / Mathf.Max(1, HPOrig));
            if (healthPct < lowHealthThreshold)
            {
                float baseBleed = Mathf.Lerp(0f, maxBleed, 1f - (healthPct / lowHealthThreshold));
                float pulse = 0.5f + 0.5f * Mathf.Sin(Time.time * pulseSpeed);
                float finalBleed = baseBleed * pulse;

                SetBleedAlpha(finalBleed);

            }

            else
            {
                SetBleedAlpha(0f);
            }
                //float bleed = (healthPct < lowHealthThreshold)
                   // ? Mathf.Lerp(0f, maxBleed, 1f - (healthPct / lowHealthThreshold))
                  //  : 0f;
           // SetBleedAlpha(bleed);

        }


        // check for movement
        float distanceMoved = Vector3.Distance(transform.position, lastPosition);
        isMoving = distanceMoved > 0.01f;

        WalkingToggle();

        lastPosition = transform.position;

      //  if (HP <= 0)
      //  {
      //      DeathSound();
      //  }

        //      HeadBob();

    }

  //  public void DeathSound()
  //  {
  //      if (deathAudio != null && !deathAudio.isPlaying)
  //      {
  //          deathAudio.Play();
  //          Destroy(gameObject, deathAudio.clip.length); // Wait for sound to finish
  //      }
  //      else
  //      {
  //          Destroy(gameObject); // Fallback
  //      }
  //  }

    //  void HeadBob()
    //  {
    //      if (isMoving)
    //      {
    //          float speedMultiplier = isSprinting ? bobSpeedMultiplier * 1.5f : bobSpeedMultiplier;
    //          bobTimer += Time.deltaTime * bobFrequency * speedMultiplier;
    //
    //          float bobOffsetY = Mathf.Sin(bobTimer) * bobAmplitude;
    //          float bobOffsetX = Mathf.Cos(bobTimer / 2f) * bobAmplitude * 0.5f;
    //
    //          cameraTransform.localPosition = initialCameraPosition + new Vector3(bobOffsetX, bobOffsetY, 0f);
    //      }
    //      else
    //      {
    //          bobTimer = 0f;
    //          cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, initialCameraPosition, Time.deltaTime * 5f);
    //      }
    //  }


    void WalkingToggle()
    {
        if (isMoving && !wasMoving)
        {
            walkAudio.Play();
        }
        else if (!isMoving && wasMoving)
        {
            walkAudio.Stop();
        }

        wasMoving = isMoving;
    }
    void movement()
    {

        if (controller.isGrounded)
        {
            lastGroundedTime = Time.time;
        }

        shootTimer += Time.deltaTime;

        if (controller.isGrounded && !isSliding)
        {
            jumpCount = 0;
            playerVel = Vector3.zero;
        }
        else if (controller.isGrounded && isSliding)
        {
            jumpCount = 0;
            playerVel -= playerVel / slideFriction * Time.deltaTime;
            if (playerVel.x < 1f && playerVel.z < 1f && playerVel.x > -1f && playerVel.z > -1f)
            {
                playerVel = Vector3.zero;
            }

        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        moveDir = (Input.GetAxis("Horizontal") * transform.right) +
                   (Input.GetAxis("Vertical") * transform.forward);

        if (controller.isGrounded)
        {
            if (!isCrouching)
            {
                controller.Move(moveDir * speed * Time.deltaTime);
            }
            else if (isSliding)
            {

            }
            else
            {
                controller.Move(moveDir * crouchSpeed * Time.deltaTime);
            }
        }
        else
        {
            if (!isCrouching)
            {
                controller.Move(moveDir * speed / airControlMod * Time.deltaTime);
            }
            else if (isSliding)
            {

            }
            else
            {
                controller.Move(moveDir * crouchSpeed / airControlMod * Time.deltaTime);
            }
        }

        jump();

        controller.Move(playerVel * Time.deltaTime);

        //if (Input.GetButtonDown("EnterShowcase"))
        //{
        //    EnterShowcaseLevel();
        //}

        if (Input.GetButton("Fire1") && shootTimer >= shootRate && equippedWeapon != null)
        {
            shoot();
        }
    }

    void jump()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < jumpMax && !isSliding)
        {
            ++jumpCount;
            playerVel.x += moveDir.x;
            playerVel.z += moveDir.z;
            playerVel.y = jumpSpeed;
        }

    }

    void crouch()
    {
#if UNITY_WEBGL

        if (Input.GetKey(KeyCode.C))
        {   
            if (Input.GetKeyDown(KeyCode.C))
            {
                if (isSprinting && (controller.isGrounded || Time.time - lastGroundedTime <= coyoteTimeMax))
                {
                    isSliding = true;
                    playerVel.x += moveDir.x * slideBoost;
                    playerVel.z += moveDir.z * slideBoost;
                }
            }
            isCrouching = true;
            controller.height = Mathf.MoveTowards(controller.height, crouchHeight, crouchSpeed * Time.deltaTime);

        }
        else
        {
            isCrouching = false;
            controller.height = Mathf.MoveTowards(controller.height, heightOrig, crouchSpeed * Time.deltaTime);
            if (isSliding)
            {
                isSliding = false;
            }
        }

#else

        if (Input.GetButton("Crouch"))
        {
            if (Input.GetButtonDown("Crouch"))
            {
                if (isSprinting && (controller.isGrounded || Time.time - lastGroundedTime <= coyoteTimeMax))
                {
                    isSliding = true;
                    playerVel.x += moveDir.x * slideBoost;
                    playerVel.z += moveDir.z * slideBoost;
                }
            }
            isCrouching = true;
            controller.height = Mathf.MoveTowards(controller.height, crouchHeight, crouchSpeed * Time.deltaTime);

        }
        else
        {
            isCrouching = false;
            controller.height = Mathf.MoveTowards(controller.height, heightOrig, crouchSpeed * Time.deltaTime);
            if (isSliding)
            {
                isSliding = false;
            }
        }

#endif
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
        }
    }

    void shoot()
    {
        if (isEquipping)
            return;

        RaycastHit hit;

        shootTimer = 0;
        if (equippedWeapon.curAmmo > 0)
        {
            if (equippedWeapon.gunAnimator != null)
            {
                equippedWeapon.gunAnimator.SetTrigger("Shooting");
            }

            //if (equippedWeapon.gunAnimator != null && equippedWeapon.gun.shootRate <= 0.2f)
            //{
            //    if (Input.GetButton("Fire1"))
            //    {
            //        equippedWeapon.gunAnimator.SetBool("FAShooting", true);
            //    }
            //    else
            //        equippedWeapon.gunAnimator.SetBool("FAShooting", false);
            //}

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignorelayer))
            {
                //Debug.Log(hit.collider.name);

                Instantiate(equippedWeapon.gun.hitEffect, hit.point, Quaternion.identity);
                equippedWeapon.curAmmo--;

                IDamage dmg = hit.collider.GetComponent<IDamage>();

                if (dmg != null)
                {
                    dmg.takeDamage(shootDamage);
                }
            }
        }
    }

    void EquipWeapon(throwableDamage newWeapon)
    {
        equippedWeapon = newWeapon;

        if (Input.GetButton("Equip"))
        {
            if (equippedWeapon.gunAnimator != null)
                equippedWeapon.gunAnimator.SetTrigger("Equip");

            isEquipping = true;

            StartCoroutine(EquipCooldown(equipDuration));
        }
    }

    private IEnumerator EquipCooldown(float howLong)
    {
        yield return new WaitForSeconds(howLong);
        isEquipping = false;
    }

    public void takeDamage(int amount)
    {
        if (hasShield && shieldCharge > 0)
        {
            shieldCharge -= amount;
            
            if(shieldCharge < 0)
            {
                HP += shieldCharge;
                shieldCharge = 0;
            }
        }
        else
        {
            HP -= amount;
            //Debug.Log("HIT BODY");
        }
        lastHitTime=Time.time;
        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

        if (HP <= 0)
        {
            locked = true;                
            isDead = true;
            if (!deathCoroutineRun)
            {
                StartCoroutine(OnDeath());
            }

        }
    }

    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.GetComponent<UISmoothFillBar>().SetFill((float)HP / HPOrig);
        if (hasShield)
            gamemanager.instance.playerArmorBar.GetComponent<UISmoothFillBar>().SetFill((float)shieldCharge / maxShield);
        gamemanager.instance.playerXPBar.GetComponent<UISmoothFillBar>().SetFill(gamemanager.instance.enemiesKilled / UpgradeManager.instance.soulsNeeded);

    }

    IEnumerator flashDamageScreen()
    {
        gamemanager.instance.PlayerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gamemanager.instance.PlayerDamageScreen.SetActive(false);
    }

    
    public void updateStats(PlayerStats stat, int amt)
    {
        switch (stat)
        {
            case PlayerStats.Health:
                HP += amt;
                updatePlayerUI();
                break;

            case PlayerStats.Speed:
                speed += amt;
                updatePlayerUI();
                break;

            case PlayerStats.JumpMax:
                jumpMax += amt;
                updatePlayerUI();
                break;
        }
    }

    public Vector3 CalculateGrappleVelocity(Vector3 startpoint, Vector3 endPoint, float trajectoryHeight)
    {

        float gravity = Physics.gravity.y; // gravity is negative
        float displacementY = endPoint.y - startpoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startpoint.x, 0f, endPoint.z - startpoint.z);

        // Calculate initial vertical velocity to reach the trajectory height
        float velocityY = Mathf.Sqrt(-2 * gravity * trajectoryHeight);

        // Time to reach the peak
        float timeToPeak = Mathf.Sqrt(2 * trajectoryHeight / -gravity);

        // Time to fall from peak to end point
        float timeFromPeakToEnd = Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / -gravity);

        // Total time of flight
        float totalTime = timeToPeak + timeFromPeakToEnd;

        // Calculate horizontal velocity
        Vector3 velocityXZ = displacementXZ / totalTime;

        // Combine vertical and horizontal velocities
        return velocityXZ + Vector3.up * velocityY;
    }
    public void SpawnPlayer()
    {
        controller.enabled = false;
        controller.transform.position = gamemanager.instance.playerSpawnPos.transform.position;
        transform.rotation = gamemanager.instance.playerSpawnPos.transform.rotation;
        controller.enabled = true;
        minimapCam.enabled = true;
        playerVel = Vector3.zero;
        HP = HPOrig;
        isDead = false;
        GetComponent<Animator>().enabled = false;
        GetComponent<Animator>().Rebind();
        deathCoroutineRun = false;
        updatePlayerUI();

    }

    void ShieldRecharge()
    {
        if (!hasShield)
            return;

        if(Time.time-lastHitTime>=shieldChargeCooldown&&shieldCharge<maxShield)
        {
            regenTimer += Time.deltaTime;
            if(regenTimer>=shieldRegenInterval)
            {
                regenTimer = 0;
                shieldCharge += shieldRegenRate;
                if(shieldCharge>maxShield)
                    shieldCharge = maxShield;

                updatePlayerUI();
            }
        }
        else
        {
            regenTimer = 0f;
        }
    }

    public IEnumerator OnDeath()
    {
        deathCoroutineRun = true;
        isDead = true;        
        minimapCam.enabled = false;
        GetComponent<Animator>().enabled = true;

        yield return new WaitForSeconds(1.2f);

        gamemanager.instance.loseGame();

        yield break;
    }

    public int GetHP()
    {
        return HP;
    }

    void SetBleedAlpha(float a)
    {
        var c = edgeBleedOverlay.color;
        c.a = Mathf.Clamp01(a);
        edgeBleedOverlay.color = c;
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("ForwardScenePortal"))
    //    {
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    //    }
    //    else if (other.CompareTag("BackwardScenePortal"))
    //    {
    //        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    //    }
    //}

    //void EnterShowcaseLevel()
    //{
    //    SceneManager.LoadScene("Showcase Level");
    //}

    public void Save(ref PlayerData data)
    {
        data.position = transform.position;
        data.rotation = transform.rotation;
        data.HP = HP;
        data.speed = speed;
        data.jumpMax = jumpMax;
        data.hasShield = hasShield;
    }

    public void Load(PlayerData data)
    {
        playerSpawnPointRef.transform.position = data.position;
        playerSpawnPointRef.transform.rotation = data.rotation;
        SpawnPlayer();
        controller.enabled = false;
        transform.rotation = data.rotation;
        controller.enabled = true;
        HP = data.HP;
        speed = data.speed;
        jumpMax = data.jumpMax;
        hasShield = data.hasShield;
    }
}


[System.Serializable]
public struct PlayerData
{
    public int HP;
    public int speed;
    public int jumpMax;
    public Vector3 position;
    public Quaternion rotation;
    public bool hasShield;

}