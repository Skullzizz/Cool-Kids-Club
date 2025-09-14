using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject linkedPortal;
    public Transform exitPos;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = linkedPortal.GetComponent<Portal>().exitPos.position;
            other.transform.rotation = linkedPortal.GetComponent<Portal>().exitPos.rotation;
        }
    }
}
