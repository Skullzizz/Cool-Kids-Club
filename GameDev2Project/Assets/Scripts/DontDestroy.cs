using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static GameObject[] persObjects = new GameObject[3];
    public int objIndex;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {

        if(persObjects[objIndex] == null)
        {
            persObjects[objIndex] = gameObject;
            DontDestroyOnLoad(gameObject); 
        }
        else if (persObjects[objIndex]!=gameObject)
        {
            Destroy(gameObject);
        }

        
    }
    
}
