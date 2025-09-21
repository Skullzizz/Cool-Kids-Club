using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteract
{
    [SerializeField] private string prompt;

    public string InteractionPropmt => prompt;

    public Transform position => transform;

    public virtual bool ActivateInteract(PlayerInteract interactor)
    {
        Debug.Log("Interactable Activated");
        return true;
    }

}
