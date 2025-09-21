using System.Collections;
using TMPro;
using UnityEngine;
public class Collectible : MonoBehaviour
{
    public int healAmount = 20;
    [SerializeField] AudioSource pickUpSound;
    MeshRenderer mesh;
    SphereCollider sphereCollider;
    public TextMeshProUGUI collectibleText;
    [SerializeField] public string basePrefabPath;
    public GameObject basePrefab;


    private void Awake()
    {
        pickUpSound=GetComponent<AudioSource>();
        mesh = GetComponent<MeshRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
        collectibleText = gamemanager.instance.CollectibleText;

    }

    private void Start()
    {
        var basePrefabPathCheck = Resources.Load(basePrefabPath, typeof(GameObject));
        if (basePrefabPathCheck != null)
        {
            //Debug.Log("Saved base prefab path object as type " + basePrefabPathCheck.GetType());
        }
        else
        {
            //Debug.Log("Saved base prefab path object as null");
        }
        basePrefab = basePrefabPathCheck as GameObject;
    }
    public enum Collectibles
    {
        Armor_Ability,
        Health_Pack
    }

    public Collectibles collectibleType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
           switch(collectibleType)
            {
                case Collectibles.Armor_Ability:
                    gamemanager.instance.playerScript.hasShield = true;
                    gamemanager.instance.playerScript.updatePlayerUI();
                    break;

                case Collectibles.Health_Pack:
                    gamemanager.instance.playerScript.updateStats(playerController.PlayerStats.Health, healAmount);
                    break;
            }
            collectibleText.text = collectibleType.ToString().Replace("_", " ") + " Aquired";

            pickUpSound.Play();
            mesh.enabled = false;
            sphereCollider.enabled = false;
            StartCoroutine(waitForSound());
        }
        
    }

    IEnumerator waitForSound()
    {
        gamemanager.instance.collectiblePopup.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        gamemanager.instance.collectiblePopup.gameObject.SetActive(false);
        Destroy(gameObject);
    }
}