using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoints")]
    public List<Transform> waypoints = new List<Transform>();

    [Header("Movement")]
    public float speed = 2f;
    public float waitTime = 0f;

    [Header("Settings")]
    public bool loop = true;
    public bool pingPong = false;

    private int currentWaypoint = 0;
    private bool movingForward = true;
    private float waitTimer = 0f;

    private void Start()
    {
        if (waypoints.Count > 0)
        {
            transform.position = waypoints[0].position;
            currentWaypoint = waypoints.Count > 1 ? 1 : 0;
        }
    }

    private void Update()
    {
        if (waypoints.Count < 2)
            return;

        if (waitTimer > 0f)
        {
            waitTimer -= Time.deltaTime;
            return;
        }

        Transform target = waypoints[currentWaypoint];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, target.position) < 0.01f)
        {
            waitTimer = waitTime;
            GetNextWaypoint();
        }
    }

    private void GetNextWaypoint()
    {
        if (pingPong)
        {
            if (movingForward)
            {
                currentWaypoint++;

                if (currentWaypoint >= waypoints.Count)
                {
                    movingForward = false;
                    currentWaypoint = waypoints.Count - 2;
                }
            }
            else
            {
                currentWaypoint--;

                if (currentWaypoint < 0)
                {
                    movingForward = true;
                    currentWaypoint = 1;
                }
            }
        }
        else
        {
            currentWaypoint++;

            if (currentWaypoint >= waypoints.Count)
            {
                if (loop)
                {
                    currentWaypoint = 0;
                }
                else
                {
                    enabled = false;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count == 0)
            return;

        Gizmos.color = Color.green;

        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null)
                continue;

            Gizmos.DrawSphere(waypoints[i].position, 0.15f);

            if (i < waypoints.Count - 1 && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }

            if (loop && i == waypoints.Count - 1 && waypoints[0] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[0].position);
            }
        }
    }
}