using System.Collections;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;


public class playerController : MonoBehaviour, IDamage
{
    [SerializeField] LayerMask ignorelayer;

    [SerializeField] CharacterController controller;
    [SerializeField] wallrunController wallrunController;

    [SerializeField] int HP;
    [SerializeField] int speed;
    [SerializeField] float crouchSpeed;
    [SerializeField] float crouchHeight;
    [SerializeField] float slideBoost;
    [SerializeField] float airSlideBoost;
    [SerializeField] float slideFriction;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpMax;
    [SerializeField] float airControlMod;
    [SerializeField] public float gravity;


    [SerializeField] public int shootDamage;
    [SerializeField] public float shootRate;
    [SerializeField] public int shootDist;

    public throwableDamage equippedWeapon;

    Vector3 moveDir;
    public Vector3 playerVel;

    int jumpCount;
    int HPOrig;
    float heightOrig;

    public bool isSprinting;
    public bool isCrouching;
    public bool isSliding;

    float shootTimer;

    bool deathCoroutineRun = false;

    Camera minimapCam;

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
        updatePlayerUI();
    }

    // Update is called once per frame
    void Update()
    {
        movement();
        sprint();
        crouch();

        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.green);
    }


    void movement()
    {

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
                if (isSprinting && controller.isGrounded)
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
        HP -= amount;
        Debug.Log("HIT");

        updatePlayerUI();
        StartCoroutine(flashDamageScreen());

        if (HP <= 0)
        {
            if (!deathCoroutineRun)
            {
                StartCoroutine(OnDeath());
            }
        }
    }

    public void updatePlayerUI()
    {
        gamemanager.instance.playerHPBar.GetComponent<UISmoothFillBar>().SetFill((float)HP / HPOrig);
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

    public void SpawnPlayer()
    {
        controller.enabled = false;
        controller.transform.position = gamemanager.instance.playerSpawnPos.transform.position;
        controller.transform.rotation = gamemanager.instance.playerSpawnPos.transform.rotation;
        controller.enabled = true;
        minimapCam.enabled = true;
        playerVel = Vector3.zero;
        HP = HPOrig;
        GetComponent<Animator>().enabled = false;
        GetComponent<Animator>().Rebind();
        gamemanager.instance.PlayerDeathScreen.gameObject.SetActive(false);
        deathCoroutineRun = false;
        updatePlayerUI();
    }

    public IEnumerator OnDeath()
    {
        controller.enabled = false;
        deathCoroutineRun = true;
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

}
