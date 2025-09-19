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

    private LineRenderer lr;
    private Vector3 grapplePoint;
    private bool isGrappling;
    private bool grabbingThrowable;
    private GameObject targetThrowable;
    private Vector3 playerVelocity;
    private Vector3 throwableVelocity;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(grappleKey))
            TryStartGrapple();

        if (Input.GetKeyUp(grappleKey))
            StopAllActions();

        if (isGrappling || grabbingThrowable)
            DrawRope();
    }

    void FixedUpdate()
    {
        if (isGrappling)
        {
            // Smoothly move player toward the hit point
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
            // Smoothly move the throwable toward your holdPoint
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
                // Finalize pickup
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
            grapplePoint = hit.point;
            lr.enabled = true;
            lr.positionCount = 2;

            // If object is tagged "Throwable", grab it instead
            if (hit.collider.CompareTag("throwable"))
            {
                grabbingThrowable = true;
                targetThrowable = hit.collider.gameObject;
            }
            else
            {
                isGrappling = true;
            }
        }
    }

    public void StopAllActions()
    {
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
}
