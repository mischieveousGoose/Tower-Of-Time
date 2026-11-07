using System.Collections.Generic;
using UnityEngine;

public class CEOBoss : MonoBehaviour
{
    enum State
    {
        Fleeing,
        Lasers,
        Saw
    }

    public float laserDuration;
    public float fleeingDuration;
    float stateTimer;
    Transform player;

    State state;

    public float speed;
    public Queue<Transform> pointsQueue = new Queue<Transform>();

    Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = FindFirstObjectByType<Player>().transform;
    }

    void Update()
    {
        stateTimer -= Time.deltaTime;
        switch (state)
        {
            case State.Fleeing:
                Fleeing();
                break;
        }
    }

    void GetNextState()
    {
        float distance = Vector2.Distance(transform.position, player.position);

    }

    void Fleeing()
    {
        FollowPath();
    }

    void Lasers()
    {

    }

    void OnEnable()
    {
        FindFirstObjectByType<LevelSpawner>().OnRoomSpawned += OnRoomSpawned;
    }

    void OnDisable()
    {
        FindFirstObjectByType<LevelSpawner>().OnRoomSpawned -= OnRoomSpawned;
    }

    public void OnRoomSpawned(GameObject room)
    {
        foreach (Transform child in room.transform.Find("Path"))
        {
            pointsQueue.Enqueue(child);
        }
    }

    void FollowPath()
    {
        Transform nextPoint = pointsQueue.Peek();
        rb.linearVelocity = (nextPoint.position - transform.position).normalized * speed;

        if (Vector2.Distance(transform.position, nextPoint.position) < 0.5f)
        {
            pointsQueue.Dequeue();
        }
    }



}

