using UnityEngine;

public class RagdollToggle : MonoBehaviour
{
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;

    private RagdollToggle ragdollToggle;
    private bool isDead = false;

    private Animator animator;
    private CharacterController characterController;

    void Start()
    {
        rigidbodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();

        ToggleRagdoll(false);
    }

    public void ToggleRagdoll(bool isRagdoll)
    {

        foreach (Rigidbody rb in rigidbodies)
        {
            rb.isKinematic = !isRagdoll;
        }

        foreach (Collider col in colliders)
        {
            col.enabled = isRagdoll;
        }

        animator.enabled = !isRagdoll;
        if (characterController != null)
        {
            characterController.enabled = !isRagdoll;
        }
    }
}