using System.Collections;
using UnityEngine;


public class SummonableStructureController : MonoBehaviour
{
    [SerializeField] GameObject wallPrefab;
    [SerializeField] float wallSegmentLength = 2f;
    [SerializeField] float fadeTime = 3f;
    [SerializeField] float spawnCoolDown = .5f;
    [SerializeField] float expandTime = 2f;
    [SerializeField] Transform player;
    [SerializeField] float groundCheckDist = 1f;

    public float lastSpawn;
    public bool isWallRunning;
    public Transform currWall;
    public bool expanding;
    public int numWallsTouching = 0;


    // Update is called once per frame
    void Update()
    {
        if (Grounded() && numWallsTouching == 0)
        {
            StopWallRun();
        }
        if (isWallRunning && Time.time - lastSpawn >= spawnCoolDown && !expanding)
        {
            SpawnWall();
            lastSpawn = Time.time;
        }
    }

    void StopWallRun()
    {
        isWallRunning = false;
        currWall = null;
        expanding = false;
    }

    bool Grounded()
    {
        return Physics.Raycast(player.position, Vector3.down, groundCheckDist);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("RunnableWall"))
        {
            numWallsTouching++;

            if (currWall == null || other.transform != currWall)
            {
                currWall = other.transform;
                isWallRunning = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("RunnableWall"))
        {
            numWallsTouching--;
            if (numWallsTouching <= 0)
            {
                numWallsTouching = 0;
                isWallRunning = false;
                currWall = null;
            }
        }
    }

    void SpawnWall()
    {
        if (currWall == null) return;

        Vector3 playerPosDot = player.position - currWall.position;
        float dotProduct = Vector3.Dot(currWall.right, player.forward);

        Vector3 spawnPosition;
        Quaternion spawnRotation = currWall.rotation;
        float segmentLength = wallSegmentLength > 0 ? wallSegmentLength : currWall.localScale.x;

        if (dotProduct >= 0)
        {
            spawnPosition = currWall.position + currWall.right * segmentLength;
        }
        else
        {
            spawnPosition = currWall.position - currWall.right * segmentLength;
        }

        GameObject wall = Instantiate(wallPrefab, spawnPosition, spawnRotation);

        StartCoroutine(ExpandFade(wall, dotProduct >= 0 ? currWall.right : -currWall.right, segmentLength));
    }

    IEnumerator ExpandFade(GameObject wall, Vector3 dir, float segLength)
    {
        expanding = true;
        float timeToExpand = 0f;

        Vector3 endPos = wall.transform.position;
        Vector3 startingPos = endPos - dir * segLength;
        wall.transform.position = startingPos;

        while (timeToExpand < expandTime)
        {
            wall.transform.position = Vector3.Lerp(startingPos, endPos, timeToExpand / expandTime);
            timeToExpand += Time.deltaTime;
            yield return null;
        }
        expanding = false;
        Renderer rend = wall.GetComponent<Renderer>();

        Material material = rend.material;
        Color origColor = material.color;

        float timeFade = 0f;
        while (timeFade < fadeTime)
        {
            float alphaColor = Mathf.Lerp(1f, 0f, timeFade / fadeTime);
            material.color = new Color(origColor.r, origColor.g, origColor.b, alphaColor);
            timeFade += Time.deltaTime;
            yield return null;
        }
        Destroy(wall);
    }
}