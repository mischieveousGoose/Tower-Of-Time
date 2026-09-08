using System.Collections.Generic;
using UnityEngine;

public class LevelSpawner : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject endRoom;
    [HideInInspector] public int levelLength;
    public List<GameObject> rooms;

    public int startRooms;
    public int increaseAmount;

    [Header("Progressive spawning")]
    public float spawnAheadDistance = 30f; // spawn next room when player is this close to the cursor
    public int roomsAheadBuffer = 2;       // how many rooms to keep pre-spawned ahead of the player

    Transform player;
    Transform nextSpawnPoint;   // where the next room attaches
    int roomsSpawned = 0;
    bool levelComplete = false;

    Queue<GameObject> spawnedRooms = new Queue<GameObject>(); // for optional cleanup

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        int level = GameManager.Instance.runData.level;
        levelLength = startRooms + ((level - 1) * increaseAmount);

        // Spawn the start room immediately
        GameObject start = Instantiate(startRoom, Vector3.zero, Quaternion.identity);
        nextSpawnPoint = start.transform.GetChild(0);
        spawnedRooms.Enqueue(start);

        // Pre-fill a small buffer so the player never sees empty space
        for (int i = 0; i < roomsAheadBuffer; i++)
        {
            SpawnNextRoom();
        }
    }

    void Update()
    {
        if (levelComplete || nextSpawnPoint == null) return;

        float distance = Vector3.Distance(player.position, nextSpawnPoint.position);
        if (distance < spawnAheadDistance)
        {
            SpawnNextRoom();
        }
    }

    void SpawnNextRoom()
    {
        if (roomsSpawned >= levelLength)
        {
            if (!levelComplete)
            {
                Instantiate(endRoom, nextSpawnPoint.position, Quaternion.identity);
                levelComplete = true;
            }
            return;
        }

        GameObject roomGO = Instantiate(
            rooms[Random.Range(0, rooms.Count)],
            nextSpawnPoint.position,
            Quaternion.identity
        );

        nextSpawnPoint = roomGO.transform.GetChild(0);
        spawnedRooms.Enqueue(roomGO);
        roomsSpawned++;

        // Optional: despawn rooms far behind the player to save memory
        TryCleanupOldRooms();
    }

    void TryCleanupOldRooms()
    {
        // Keep only a handful of rooms behind the player alive
        while (spawnedRooms.Count > roomsAheadBuffer + 3)
        {
            GameObject old = spawnedRooms.Dequeue();
            if (Vector3.Distance(player.position, old.transform.position) > spawnAheadDistance * 3f)
            {
                Destroy(old);
            }
            else
            {
                // not far enough yet, put it back and stop
                // (simple approach — a List with manual indexing gives more control if you need it)
                break;
            }
        }
    }
}