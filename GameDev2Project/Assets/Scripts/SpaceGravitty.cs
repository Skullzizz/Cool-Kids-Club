using UnityEngine;

public class SpaceGravitty : MonoBehaviour
{
    [SerializeField] public float spaceGrav = 0;
     float ogGrav;
    public bool inSpace;

    public static SpaceGravitty instance;

    private void Awake()
    {
        instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (inSpace && gamemanager.instance.playerScript.locked || gamemanager.instance.isPaused)
        {
            ExitSpace();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !gamemanager.instance.playerScript.locked)
        {
            EnterSpace();
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ExitSpace();
        }
    }

    void EnterSpace()
    {
        inSpace = true;
        ogGrav = gamemanager.instance.playerScript.gravity;
        gamemanager.instance.playerScript.gravity = spaceGrav;
        Physics.gravity = new Vector3(0, spaceGrav, 0);
        gamemanager.instance.SpaceScreen(true);
    }

    void ExitSpace()
    {
        inSpace = false;
        gamemanager.instance.playerScript.gravity = ogGrav;
        gamemanager.instance.SpaceScreen(false);
    }
}
