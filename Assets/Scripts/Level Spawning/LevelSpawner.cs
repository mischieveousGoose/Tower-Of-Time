using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class LevelSpawner : MonoBehaviour
{
    public Action<GameObject> OnRoomSpawned;

    EnemySpawner enemySpawner;
    public GameObject startRoom;
    public GameObject endRoom;
    [HideInInspector] public int levelLength;

    public bool infiniteLevel = false;


    public List<GameObject> rooms;

    [Space(8)]

    public int startRooms;
    public int increaseAmount;

    [Header("Progressive spawning")]
    public float spawnAheadDistance = 30f;
    public int roomsAheadBuffer = 2;

    Transform player;
    Vector3 nextSpawnPoint;
    int roomsSpawned = 0;
    bool levelComplete = false;
    bool isSpawning = false;

    public Vector3 startRoomSpawnPoint;

    Queue<GameObject> spawnedRooms = new Queue<GameObject>();


    [Header("Tilemap Stuff")]
    public TileBase endTile;
    public TileBase startTile;

    void Start()
    {
        enemySpawner = GetComponent<EnemySpawner>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

        int level = GameManager.Instance.runData.level;
        levelLength = startRooms + ((level - 1) * increaseAmount);

        GameObject start = Instantiate(startRoom, startRoomSpawnPoint, Quaternion.identity, transform);
        HideLogicTilemap(start);
        Vector3Int startEndCell = GetTileCellPosition(start, endTile);

        nextSpawnPoint = GetTileWorldPosition(start, endTile);
        DeleteStartAndEndTiles(start);
        spawnedRooms.Enqueue(start);

        for (int i = 0; i < roomsAheadBuffer; i++)
        {
            SpawnNextRoom();
        }
    }

    void Update()
    {
        if (levelComplete || isSpawning) return;

        float distance = Vector3.Distance(player.position, nextSpawnPoint);
        if (distance < spawnAheadDistance)
        {
            SpawnNextRoom();
        }
    }

    void SpawnNextRoom()
    {
        if (isSpawning) return;
        isSpawning = true;

        if (roomsSpawned >= levelLength && !infiniteLevel)
        {
            if (!levelComplete)
            {
                GameObject endRoomGO = PlaceRoomAt(endRoom, nextSpawnPoint);
                DeleteStartAndEndTiles(endRoomGO);
                HideLogicTilemap(endRoomGO);
                levelComplete = true;
            }
            isSpawning = false;
            return;
        }

        GameObject prefab = rooms[UnityEngine.Random.Range(0, rooms.Count)];
        GameObject roomGO = PlaceRoomAt(prefab, nextSpawnPoint);

        nextSpawnPoint = GetTileWorldPosition(roomGO, endTile);
        DeleteStartAndEndTiles(roomGO);
        enemySpawner.SpawnEnemies(roomGO);
        HideLogicTilemap(roomGO);
        spawnedRooms.Enqueue(roomGO);
        roomsSpawned++;

        TryCleanupOldRooms();
        isSpawning = false;
    }

    GameObject PlaceRoomAt(GameObject roomPrefab, Vector3 targetPos)
    {
        GameObject roomGO = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, transform);
        HideLogicTilemap(roomGO);

        Vector3Int startCell = GetTileCellPosition(roomGO, startTile);
        Vector3 startTileWorldPos = CellToWorldPosition(roomGO, startCell);
        Vector3 correction = targetPos - startTileWorldPos;
        roomGO.transform.position += correction + Vector3.right;
        OnRoomSpawned?.Invoke(roomGO);



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

    public Tilemap GetLogicTilemap(GameObject room)
    {
        return room.transform.Find("Grid").Find("Logic").GetComponent<Tilemap>();
    }

    void HideLogicTilemap(GameObject room)
    {
        TilemapRenderer renderer = GetLogicTilemap(room).GetComponent<TilemapRenderer>();
        if (renderer != null)
        {
            renderer.enabled = false;
        }
    }

    public Tilemap GetGroundTilemap(GameObject room)
    {
        Transform grid = room.transform.Find("Grid");
        Transform ground = grid != null ? grid.Find("Ground") : null;
        return ground != null ? ground.GetComponent<Tilemap>() : null;
    }

    public Vector3Int GetTileCellPosition(GameObject room, TileBase tile)
    {
        Tilemap ground = GetLogicTilemap(room);

        foreach (var pos in ground.cellBounds.allPositionsWithin)
        {
            if (ground.GetTile(pos) == tile)
            {
                return pos;
            }
        }
        Debug.LogWarning("Tile not found in room: " + tile.name + " (" + room.name + ")");
        return Vector3Int.zero;
    }

    public Vector3 GetTileWorldPosition(GameObject room, TileBase tile)
    {
        Tilemap ground = GetLogicTilemap(room);
        Vector3Int pos = GetTileCellPosition(room, tile);
        return ground.CellToWorld(pos) + ground.tileAnchor;

    }

    Vector3 CellToWorldPosition(GameObject room, Vector3Int cellPos)
    {
        Tilemap logic = GetLogicTilemap(room);
        return logic.CellToWorld(cellPos) + logic.tileAnchor;
    }

    void DeleteStartAndEndTiles(GameObject room)
    {
        Tilemap ground = GetLogicTilemap(room);

        foreach (var pos in ground.cellBounds.allPositionsWithin)
        {
            if (ground.GetTile(pos) == startTile || ground.GetTile(pos) == endTile)
            {
                ground.SetTile(pos, null);
            }
        }
    }
}