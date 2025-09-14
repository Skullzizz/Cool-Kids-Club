using UnityEngine;

public class InteractableObject : MonoBehaviour, IInteract
{
    [SerializeField] private string prompt;

    public string InteractionPropmt => prompt;

    public Transform position => transform;

    public bool ActivateInteract(PlayerInteract interactor)
    {
        Debug.Log("Interactable Activated");
        return true;
  
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
