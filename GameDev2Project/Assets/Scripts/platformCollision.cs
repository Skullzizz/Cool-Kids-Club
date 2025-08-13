using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlatformCollision : MonoBehaviour
{
    [SerializeField] string playerTag = "Player";
    [SerializeField] Transform platform;

    [SerializeField] bool playerOnPlatform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag.Equals(playerTag))
        {
            playerOnPlatform = true;
            // Make the player a child of the platform
            other.gameObject.transform.parent = platform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals(playerTag))
        {
            playerOnPlatform = false;
            // Remove the player from the platform's hierarchy
            other.gameObject.transform.parent = null;
        }
    }
}

