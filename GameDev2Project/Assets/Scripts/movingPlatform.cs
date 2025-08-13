using System.Collections;
using UnityEngine;

public class movingPlatform : MonoBehaviour
{
    // set the platform to move, start and end points(empties in scene) of the platform's movement, and the speed at which it moves
    [SerializeField] GameObject platform;
    [SerializeField] Transform startPoint;
    [SerializeField] Transform endPoint;
    [SerializeField] float speed = 2.0f;
    [SerializeField] float delay = 1f;

    private Vector3 targetPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // move platform to the starting position and set target position to the end point at the start of the game
        platform.transform.position = startPoint.position;
        targetPosition = endPoint.position;
        // start the coroutine to move the platform
        StartCoroutine(MovePlatform());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator MovePlatform()
    {
        // makes infinite loop to move the platform back and forth between start and end points
        while (true)
        {
            // checks if platform is at target position
            while (Vector3.Distance(platform.transform.position, targetPosition) > 0.01f)
            {
                // move the platform towards the target position
                platform.transform.position = Vector3.MoveTowards(platform.transform.position, targetPosition, speed * Time.deltaTime);
                yield return null; // wait for the next frame
            }

            // switch target position to the other end point
            targetPosition = targetPosition == endPoint.position ? startPoint.position : endPoint.position;
    
            yield return new WaitForSeconds(delay); // wait for the specified delay before moving again
        }
    }
}
