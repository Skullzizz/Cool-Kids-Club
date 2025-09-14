using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float waypointSize = 1f;

    private int direction = 1;  // 1 = going forward -1 = going backwards

    private void OnDrawGizmos()
    {
        foreach(Transform t in transform)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, waypointSize);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount -1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }

        Gizmos.DrawLine(transform.GetChild(transform.childCount - 1).position, transform.GetChild(0).position);
    }

    public Transform GetNextWaypoint(Transform currentWaypoint)
    {
        if (currentWaypoint == null)
        {
            return transform.GetChild(0);
        }

        int index = currentWaypoint.GetSiblingIndex();
        if (direction == 1 && index >= transform.childCount -1)
        {
            direction = -1;
        }
        else if (direction == -1 && index <= 0)
        {
            direction = 1;
        }

        return transform.GetChild(index + direction);
    }
}
