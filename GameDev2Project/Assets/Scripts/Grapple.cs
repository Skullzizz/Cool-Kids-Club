using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GrapplingGun : MonoBehaviour
{
    [Header("Grapple Settings")]
    public float maxGrappleDistance = 30f;
    public float pullSmoothTime = 0.2f;
    public KeyCode grappleKey = KeyCode.Mouse0;

    [Header("References")]
    public Transform gunTip;
    public Transform player;
    public Transform holdPoint;

    [Header("Audio Sources")]
    [Tooltip("AudioSource that plays when the grapple is launched")]
    public AudioSource launchSource;
    [Tooltip("AudioSource that plays when the grapple latches")]
    public AudioSource latchSource;

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private bool isGrappling;
    private bool grabbingThrowable;
    private GameObject targetThrowable;
    private Vector3 playerVelocity;
    private Vector3 throwableVelocity;
    [SerializeField] float grappleCooldown;
    [SerializeField] bool canGrapple = true;

    void Start()
    {
        canGrapple=true;
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;

        // Ensure playOnAwake is off
        if (launchSource != null)
            launchSource.playOnAwake = false;
        if (latchSource != null)
            latchSource.playOnAwake = false;
    }

    void Update()
    {
        if (!gamemanager.instance.playerScript.isDead&&canGrapple)
        {
            if (Input.GetKeyDown(grappleKey))
                TryStartGrapple();

            if (Input.GetKeyUp(grappleKey))
                StopAllActions();

            if (isGrappling || grabbingThrowable)
                DrawRope();
        }
    }

    void FixedUpdate()
    {
        if (isGrappling)
        {
            player.position = Vector3.SmoothDamp(
                player.position,
                grapplePoint,
                ref playerVelocity,
                pullSmoothTime
            );

            if (Vector3.Distance(player.position, grapplePoint) < 0.5f)
                StopAllActions();
        }
        else if (grabbingThrowable && targetThrowable != null)
        {
            targetThrowable.transform.position = Vector3.SmoothDamp(
                targetThrowable.transform.position,
                holdPoint.position,
                ref throwableVelocity,
                pullSmoothTime
            );

            if (Vector3.Distance(
                    targetThrowable.transform.position,
                    holdPoint.position
                ) < 0.2f)
            {
                targetThrowable.transform.SetParent(holdPoint);
                grabbingThrowable = false;
                lr.enabled = false;
            }
        }
    }

    void TryStartGrapple()
    {
        if (Physics.Raycast(
                gunTip.position,
                gunTip.forward,
                out RaycastHit hit,
                maxGrappleDistance
            ))
        {
            // Play launch sound
            if (launchSource != null)
                launchSource.Play();

            grapplePoint = hit.point;
            lr.enabled = true;
            lr.positionCount = 2;

            if (hit.collider.CompareTag("throwable"))
            {
                grabbingThrowable = true;
                targetThrowable = hit.collider.gameObject;
            }
            else
            {
                isGrappling = true;

                // Play latch sound
                if (latchSource != null)
                    latchSource.Play();
            }
        }
    }

    public void StopAllActions()
    {
        StartCoroutine(GrappleCooldown());
        isGrappling = false;
        grabbingThrowable = false;
        targetThrowable = null;
        lr.enabled = false;
        lr.positionCount = 0;
        playerVelocity = Vector3.zero;
        throwableVelocity = Vector3.zero;
    }

    void DrawRope()
    {
        lr.SetPosition(0, gunTip.position);

        if (grabbingThrowable && targetThrowable != null)
            lr.SetPosition(1, targetThrowable.transform.position);
        else
            lr.SetPosition(1, grapplePoint);
    }

    IEnumerator GrappleCooldown()
    {
        canGrapple = false;
        yield return new WaitForSeconds(grappleCooldown);
        canGrapple=true;
    }
}