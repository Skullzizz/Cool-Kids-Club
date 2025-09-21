using UnityEngine;

public class BossTrigger : MonoBehaviour
{
    [SerializeField] GameObject boss;
    [SerializeField] GameObject bossDoor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!bossDoor.activeSelf||!boss.activeSelf){
                boss.SetActive(true);
                bossDoor.SetActive(true);
                gamemanager.instance.enemyCountText.SetActive(true);
                Destroy(gameObject);
            }
        }
    }
}
