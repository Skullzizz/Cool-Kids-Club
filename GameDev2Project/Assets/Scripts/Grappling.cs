using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using NUnit.Framework.Internal.Filters;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private playerController pMove;
    public Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lineRend;

    [Header("Grappling")]
    public int maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    [Header("Cooldown")]
    public float grapplingCd;
    public float grapplingCdTimer;

    [Header("Input")]
    public KeyCode grapplingKey = KeyCode.Mouse1;

    private bool grappling;

    private void Start()
    {
        pMove = GetComponent<playerController>();

        lineRend.enabled = true;
        lineRend.SetPosition(1, grapplePoint);
    }

    private void LateUpdate()
    {
        if (grappling)
            lineRend.SetPosition(0, gunTip.position);
    }

    private void Update()
    {
        if (Input.GetKeyDown(grapplingKey)) StartGrapple();

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0) return;

        grappling = true;

        pMove.isGrappling = true;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;

            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;

            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
    }

    private void ExecuteGrapple()
    {
        pMove.isGrappling = false;

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        pMove.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        pMove.isGrappling = false;

        grappling = false;

        grapplingCdTimer = grapplingCd;

        lineRend.enabled = false;
    }
}