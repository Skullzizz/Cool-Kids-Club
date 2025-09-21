using UnityEngine;

public class ChangeMusicTrigger : MonoBehaviour
{
    [SerializeField] AudioClip nextMusic;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gamemanager.instance.ChangeMusic(nextMusic);
        }
    }
}
