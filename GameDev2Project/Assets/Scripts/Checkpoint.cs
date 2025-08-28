using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gamemanager.instance.playerSpawnPos.transform.position != transform.position)
        {
            gamemanager.instance.playerSpawnPos.transform.position = transform.position;
            model.material.color = Color.lightBlue;
        }
    }

}
