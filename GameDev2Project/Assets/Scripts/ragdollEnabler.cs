using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator))]
public class RagdollController : MonoBehaviour
{
    [Tooltip("Downward impulse applied when ragdoll activates")]
    public float downForce = 20f;

    private Animator animator;
    private NavMeshAgent navAgent;
    private Rigidbody[] ragdollBodies;
    private Collider[] ragdollColliders;
    private bool isRagdollActive;

    void Awake()
    {
        // Cache Animator and NavMeshAgent
        animator = GetComponent<Animator>();
        navAgent = GetComponent<NavMeshAgent>();

        // Gather all child rigidbodies and colliders (including root)
        ragdollBodies = GetComponentsInChildren<Rigidbody>();
        ragdollColliders = GetComponentsInChildren<Collider>();

        // Disable ragdoll at start
        foreach (var rb in ragdollBodies)
        {
            rb.isKinematic = true;
        }
        foreach (var col in ragdollColliders)
        {
            // Keep the root collider active for hits, disable all bone colliders
            if (col.gameObject != gameObject)
                col.enabled = false;
        }
    }

    /// <summary>
    /// Call this when the enemy dies.
    /// </summary>
    public void ActivateRagdoll()
    {
        if (isRagdollActive) return;
        isRagdollActive = true;

        // Turn off animation and navigation
        animator.enabled = false;
        if (navAgent != null) navAgent.enabled = false;

        // Enable physics on every bone
        foreach (var col in ragdollColliders)
            col.enabled = true;

        foreach (var rb in ragdollBodies)
            rb.isKinematic = false;

        // Apply a downward impulse to the hips/root body
        var rootRb = GetComponent<Rigidbody>();
        if (rootRb != null)
        {
            rootRb.AddForce(Vector3.down * downForce, ForceMode.Impulse);
        }
    }
}
