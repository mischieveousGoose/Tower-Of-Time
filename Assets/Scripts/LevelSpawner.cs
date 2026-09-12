using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelSpawner : MonoBehaviour
{
    public GameObject startRoom;
    public GameObject endRoom;
    [HideInInspector] public int levelLength;
    public List<GameObject> rooms;

    public int startRooms;
    public int increaseAmount;

    [Header("Progressive spawning")]
    public float spawnAheadDistance = 30f;
    public int roomsAheadBuffer = 2;

    Transform player;
    Vector3 nextSpawnPoint;
    int roomsSpawned = 0;
    bool levelComplete = false;

    public Vector3 startRoomSpawnPoint;

    Queue<GameObject> spawnedRooms = new Queue<GameObject>();

    [Header("Tilemap Stuff")]
    public TileBase endTile;
    public TileBase startTile;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        int level = GameManager.Instance.runData.level;
        levelLength = startRooms + ((level - 1) * increaseAmount);

        GameObject start = Instantiate(startRoom, startRoomSpawnPoint, Quaternion.identity);
        nextSpawnPoint = GetTileWorldPosition(start, endTile);
        spawnedRooms.Enqueue(start);

        for (int i = 0; i < roomsAheadBuffer; i++)
        {
            SpawnNextRoom();
        }
    }

    void Update()
    {
        if (levelComplete) return;

        float distance = Vector3.Distance(player.position, nextSpawnPoint);
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
                PlaceRoomAt(endRoom, nextSpawnPoint);
                levelComplete = true;
            }
            return;
        }

        GameObject roomPrefab = rooms[Random.Range(0, rooms.Count)];
        GameObject roomGO = PlaceRoomAt(roomPrefab, nextSpawnPoint);

        nextSpawnPoint = GetTileWorldPosition(roomGO, endTile);
        DeleteStartAndEndTiles(roomGO);
        spawnedRooms.Enqueue(roomGO);
        roomsSpawned++;

        TryCleanupOldRooms();
    }

    // Instantiates a room, then shifts it so its startTile lands exactly on targetPos.
    GameObject PlaceRoomAt(GameObject roomPrefab, Vector3 targetPos)
    {
        GameObject roomGO = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, transform);

        Vector3 startTileWorldPos = GetTileWorldPosition(roomGO, startTile);
        Vector3 correction = targetPos - startTileWorldPos;
        roomGO.transform.position += correction;

        return roomGO;
    }

    void TryCleanupOldRooms()
    {
        while (spawnedRooms.Count > roomsAheadBuffer + 3)
        {
            GameObject old = spawnedRooms.Peek(); // peek first, don't lose it if we're not destroying yet
            if (Vector3.Distance(player.position, old.transform.position) > spawnAheadDistance * 3f)
            {
                spawnedRooms.Dequeue();
                Destroy(old);
            }
            else
            {
                break;
            }
        }
    }

    public Tilemap GetGroundTilemap(GameObject room)
    {
        return room.transform.Find("Grid").Find("Ground").GetComponent<Tilemap>();
    }

    public Vector3 GetTileWorldPosition(GameObject room, TileBase tile)
    {
        Tilemap ground = GetGroundTilemap(room);

        foreach (var pos in ground.cellBounds.allPositionsWithin)
        {
            if (ground.GetTile(pos) == tile)
            {
                return ground.CellToWorld(pos) + ground.tileAnchor;
            }
        }
        Debug.LogWarning("Tile not found in room: " + tile.name + " (" + room.name + ")");
        return room.transform.position;
    }

    void DeleteStartAndEndTiles(GameObject room)
    {
        Tilemap ground = GetGroundTilemap(room);

        foreach (var pos in ground.cellBounds.allPositionsWithin)
        {
            if (ground.GetTile(pos) == startTile || ground.GetTile(pos) == endTile)
            {
                ground.SetTile(pos, null);
            }
        }
    }
}