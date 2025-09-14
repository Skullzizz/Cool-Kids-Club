using UnityEngine;

public interface IInteract
{
    public bool ActivateInteract(PlayerInteract interactor);
    public string InteractionPropmt { get; }
    public Transform position { get; }

}