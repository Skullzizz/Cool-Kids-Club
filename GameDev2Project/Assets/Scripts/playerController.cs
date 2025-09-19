using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class playerController : MonoBehaviour, IDamage, ISaveable
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

    [Header("Movement Controls")]
    Vector3 moveDir;
    public Vector3 playerVel;

    [Header("Originals")]
    int HPOrig;
    float heightOrig;

    [Header("General Bools")]
    public bool isSprinting;
    public bool isCrouching;
    public bool isSliding;

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


    public enum PlayerStats
    {
        Health,
        Speed,
        JumpMax
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        heightOrig = controller.height;
        gamemanager.instance.updateEnemyDeaths(0);
        minimapCam = GameObject.FindWithTag("MinimapCam").GetComponent<Camera>();
        playerSpawnPointRef = gamemanager.instance.playerSpawnPos;
        updatePlayerUI();
        if (edgeBleedOverlay != null)
        {
            SetBleedAlpha(0f);
        }
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

        if (Input.GetButtonDown("EnterShowcase"))
        {
            EnterShowcaseLevel();
        }

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
        RaycastHit hit;

        shootTimer = 0;
        if (equippedWeapon.curAmmo > 0)
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignorelayer))
            {
                Debug.Log(hit.collider.name);

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
            Debug.Log("HIT BODY");
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
        controller.transform.rotation = gamemanager.instance.playerSpawnPos.transform.rotation;
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

    void EnterShowcaseLevel()
    {
        SceneManager.LoadScene("Showcase Level");
    }

    //public void SavePlayer()
    //{
    //    SaveLoad.SavePlayer(this);
    //}

    //public void LoadPlayer()
    //{
    //    PlayerData data = SaveLoad.LoadPlayer();
    //    if (data != null) return;

    //    HP = data.HP;
    //    speed = data.speed;
    //    jumpMax = data.jumpMax;
    //    Vector3 position;
    //    position.x = data.position[0];
    //    position.y = data.position[1];
    //    position.z = data.position[2];

    //    transform.position = position;

    //}

    public void LoadState(object state)
    {
        var playerData = (PlayerData)state;
        var player = gamemanager.instance.playerScript;
        Vector3 position;
        position.x = playerData.position[0];
        position.y = playerData.position[1];
        position.z = playerData.position[2];

        player.playerSpawnPointRef.transform.position = position;
        SpawnPlayer();
        player.HP = playerData.HP;
        player.speed = playerData.speed;
        player.jumpMax = playerData.jumpMax;
    }

    public object SaveState()
    {
        Debug.Log("Loading player data to save!");
        playerController player = gamemanager.instance.playerScript;
        return new PlayerData(player);
    }

    
}
[System.Serializable]
public struct PlayerData
{
    public int HP;
    public int speed;
    public int jumpMax;
    public float[] position;

    public PlayerData(playerController player)
    {
        HP = player.HP;
        speed = player.speed;
        jumpMax = player.jumpMax;
        position = new float[3];
        position[0] = player.transform.position.x;
        position[1] = player.transform.position.y;
        position[2] = player.transform.position.z;

    }


}