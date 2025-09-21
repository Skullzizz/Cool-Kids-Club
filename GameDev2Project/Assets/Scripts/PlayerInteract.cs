using System.Xml.Serialization;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [SerializeField] Transform interactablePosition;
    [SerializeField] float interactRadius;
    [SerializeField] LayerMask interactLayerMask;
    [SerializeField] private PromptUI promptUI;
    public float interactRange;

    private readonly Collider[] colliders = new Collider[3];
    [SerializeField] private int numberFound;

    private IInteract interactable;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        numberFound = Physics.OverlapSphereNonAlloc(interactablePosition.position, interactRadius, colliders, (int)interactLayerMask);

        if (numberFound > 0)
        {
            interactable = colliders[0].GetComponent<IInteract>();

            if (interactable != null)
            {
                if (!promptUI.isDisplayed)
                {
                    promptUI.SetText(interactable.InteractionPropmt);
                }
                else
                {
                    promptUI.SetPos(interactable.position);
                }

                if (Input.GetButtonDown("Interact"))
                {
                    interactable.ActivateInteract(this);
                }
            }
        }
        else
        {
            if (interactable != null)
            {
                interactable = null;
            }
            if (promptUI.isDisplayed)
            {
                 promptUI.Close();
            }
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(interactablePosition.position, interactRadius);
    }
}
