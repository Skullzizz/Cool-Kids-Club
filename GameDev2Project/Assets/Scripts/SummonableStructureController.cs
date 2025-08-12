using System.Collections;
using System.Net;
using UnityEngine;
using UnityEngine.UIElements;

public class SummonableStructureController : MonoBehaviour
{
    [SerializeField] GameObject wallPrefab;
    [SerializeField] bool isWallRunning;
    [SerializeField] float wallSegmentLength = 2f;
    [SerializeField] float fadeTime = 3f;
    [SerializeField] float spawnCoolDown = .5f;
    [SerializeField] float expandTime = 2f;

    public float lastSpawn;

    public Transform player;
    public Transform cameraTransform;
    public int wallCount;
    public Transform currWall;
    public bool expanding = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isWallRunning && Time.time - lastSpawn >= spawnCoolDown &&!expanding)
        {
            SpawnWall();
            lastSpawn = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RunnableWall"))
        {
            wallCount++;
            currWall = other.transform;
            isWallRunning = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RunnableWall"))
        {
            if (wallCount <= 0)
            {
                wallCount = 0;
                isWallRunning = false;
                currWall = null;
            }
        }
    }

    void SpawnWall()
    {
        if (currWall == null) return;
        Vector3 playerPosDot = player.position-currWall.position;
        float dotProduct = Vector3.Dot(currWall.right, playerPosDot);

        Vector3 spawnPosition;
        Quaternion spawnRotation = currWall.rotation;
        wallSegmentLength = currWall.localScale.x;

        if (dotProduct>= 0)
        {
            spawnPosition = currWall.position + currWall.right * wallSegmentLength;
        }
        else
        {
            spawnPosition = currWall.position - currWall.right * wallSegmentLength;
        }

        GameObject wall = Instantiate(wallPrefab, spawnPosition, spawnRotation);
        currWall = wall.transform;

        StartCoroutine(ExpandWall(wall, dotProduct >= 0 ? currWall.right : -currWall.right));
    }
    IEnumerator FadeDestroy(GameObject wall)
    {
        Renderer rend = wall.GetComponent<Renderer>();

        Material material = rend.material;
        Color origColor = material.color;

        float timeFade = 0f;
        while(timeFade < fadeTime)
        {
            float alphaColor = Mathf.Lerp(1f,0f,timeFade/fadeTime);
            material.color = new Color(origColor.r,origColor.g, origColor.b, alphaColor);
            timeFade+=Time.deltaTime;
            yield return null;
        }
        wallCount--;
        Destroy(wall);
    }
    IEnumerator ExpandWall(GameObject wall,Vector3 directon)
    {
        float timeToExpand = 0f;

        Vector3 endPos = wall.transform.position;
        Vector3 startingPos = endPos - directon * wallSegmentLength;
        wall.transform.position = startingPos;

        expanding = true;
        while (timeToExpand < expandTime)
        {
            wall.transform.position = Vector3.Lerp(startingPos, endPos, timeToExpand / expandTime);
            timeToExpand += Time.deltaTime;
            yield return null;
        }
        expanding = false;
        StartCoroutine(FadeDestroy(wall));
    }
}