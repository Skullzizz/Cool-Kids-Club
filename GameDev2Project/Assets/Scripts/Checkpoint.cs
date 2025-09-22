using System.Collections;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] Transform checkpointTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && gamemanager.instance.playerSpawnPos.transform.position != transform.position)
        {
            gamemanager.instance.playerSpawnPos.transform.position = checkpointTrigger.position;
            gamemanager.instance.playerSpawnPos.transform.rotation = checkpointTrigger.rotation;
            
           model.material.color = Color.lightBlue;
            gamemanager.instance.SaveAsync();
            StartCoroutine(checkpointFeedback());
        }
    }

    IEnumerator checkpointFeedback()
    {
        gamemanager.instance.checkpointPopup.SetActive(true);
        yield return new WaitForSeconds(2f);
        gamemanager.instance.checkpointPopup.SetActive(false);
    }
}
