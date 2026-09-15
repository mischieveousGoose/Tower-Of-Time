using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class LevelSpawner : MonoBehaviour
{
    EnemySpawner enemySpawner;
    public GameObject startRoom;
    public GameObject[] endRooms; // Right, Up, Left, Down
    [HideInInspector] public int levelLength;
    public List<GameObject> rooms;

    [Space(8)]

    [SerializeField] List<GameObject> rightRooms = new List<GameObject>();
    [SerializeField] List<GameObject> upRooms = new List<GameObject>();
    [SerializeField] List<GameObject> downRooms = new List<GameObject>();
    [SerializeField] List<GameObject> leftRooms = new List<GameObject>();

    [Space(8)]

    public int startRooms;
    public int increaseAmount;

    [Header("Progressive spawning")]
    public float spawnAheadDistance = 30f;
    public int roomsAheadBuffer = 2;

    [Header("Overlap prevention")]
    [Tooltip("Shrinks each room's bounds by this much (world units) before checking overlap, so rooms sharing an exact edge/connector aren't treated as overlapping.")]
    public float overlapShrink = 0.1f;
    [Tooltip("How many times to try a different prefab for a slot before giving up and forcing placement anyway.")]
    public int maxPlacementAttempts = 8;

    Transform player;
    Vector3 nextSpawnPoint;
    int roomsSpawned = 0;
    bool levelComplete = false;

    public Vector3 startRoomSpawnPoint;
    Vector3 nextDirection = Vector3.right;

    Queue<GameObject> spawnedRooms = new Queue<GameObject>();
    Dictionary<GameObject, Bounds> roomBounds = new Dictionary<GameObject, Bounds>();

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
        Vector3 endTileRotation = GetTileRotation(startEndCell, GetLogicTilemap(start));
        CarveEntranceHole(start, startEndCell, endTileRotation);

        nextSpawnPoint = GetTileWorldPosition(start, endTile);
        DeleteStartAndEndTiles(start);
        spawnedRooms.Enqueue(start);
        RegisterBounds(start);

        for (int i = 0; i < roomsAheadBuffer; i++)
        {
            SpawnNextRoom();
        }
    }

    List<GameObject> GetRoomsForDirection(Vector3 direction)
    {
        if (direction == Vector3.right) return rightRooms;
        if (direction == Vector3.left) return leftRooms;
        if (direction == Vector3.up) return upRooms;
        if (direction == Vector3.down) return downRooms;

        Debug.LogError("Invalid direction: " + direction);
        return null;
    }

    // A room can have MULTIPLE start tiles placed in its Logic tilemap, each rotated to
    // face a different direction (e.g. one rotated to face left, another rotated to face
    // down). This lets the same room prefab serve as a "left room" or a "down room"
    // depending on which direction is actually needed when it's spawned. Every start tile
    // found on a room adds that room to the corresponding direction list.
    [ExecuteInEditMode]
    public void InitializeRoomLists()
    {
        leftRooms.Clear();
        rightRooms.Clear();
        upRooms.Clear();
        downRooms.Clear();

        foreach (var room in rooms)
        {
            Tilemap logic = GetLogicTilemap(room);
            List<Vector3Int> startCells = GetAllTileCellPositions(room, startTile);

            if (startCells.Count == 0)
            {
                Debug.LogWarning("Room has no start tile: " + room.name);
                continue;
            }

            foreach (var cell in startCells)
            {
                Vector3 rotation = GetTileRotation(cell, logic);
                List<GameObject> list = GetRoomsForDirection(rotation);

                if (list != null && !list.Contains(room))
                {
                    list.Add(room);
                }
            }
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
                int index = nextDirection == Vector3.right ? 0 : nextDirection == Vector3.up ? 1 : nextDirection == Vector3.left ? 2 : 3;
                GameObject endRoomGO = PlaceRoomAt(endRooms[index], nextSpawnPoint, nextDirection);
                DeleteStartAndEndTiles(endRoomGO);
                HideLogicTilemap(endRoomGO);
                levelComplete = true;
            }
            return;
        }

        List<GameObject> candidates = GetRoomsForDirection(nextDirection);
        GameObject roomGO = PlaceNonOverlappingRoom(candidates, nextSpawnPoint, nextDirection);

        nextSpawnPoint = GetTileWorldPosition(roomGO, endTile);
        nextDirection = GetTileRotation(GetTileCellPosition(roomGO, endTile), GetLogicTilemap(roomGO));
        DeleteStartAndEndTiles(roomGO);
        enemySpawner.SpawnEnemies(roomGO);
        HideLogicTilemap(roomGO);
        spawnedRooms.Enqueue(roomGO);
        roomsSpawned++;

        TryCleanupOldRooms();
    }

    // Tries several candidate prefabs (in random order) at targetPos and keeps the first
    // one whose bounds don't overlap an already-placed room. Falls back to the last
    // attempt if nothing clean is found within maxPlacementAttempts tries.
    GameObject PlaceNonOverlappingRoom(List<GameObject> candidates, Vector3 targetPos, Vector3 incomingDirection)
    {
        List<GameObject> shuffled = new List<GameObject>(candidates);
        Shuffle(shuffled);

        GameObject lastAttempt = null;
        int attempts = Mathf.Min(maxPlacementAttempts, shuffled.Count > 0 ? shuffled.Count * 3 : maxPlacementAttempts);

        for (int i = 0; i < attempts; i++)
        {
            GameObject prefab = shuffled[i % shuffled.Count];
            GameObject candidate = PlaceRoomAt(prefab, targetPos, incomingDirection);
            Bounds b = GetRoomWorldBounds(candidate);

            if (!WouldOverlap(b))
            {
                RegisterBounds(candidate);
                return candidate;
            }

            if (lastAttempt != null)
            {
                UnregisterBounds(lastAttempt);
                Destroy(lastAttempt);
            }
            lastAttempt = candidate;
        }

        // Nothing worked cleanly, keep the last attempt so the level doesn't dead-end.
        Debug.LogWarning("Could not find a non-overlapping room after " + attempts + " attempts, forcing placement.");
        RegisterBounds(lastAttempt);
        return lastAttempt;
    }

    void Shuffle(List<GameObject> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }

    // Instantiates a room, then shifts it so the start tile matching incomingDirection
    // lands exactly on targetPos. Since a room can have several start tiles (one per
    // direction it supports), we pick the one whose rotation matches the direction we
    // actually need for this slot. Once placed, a 3-wide hole is carved into the room's
    // Ground tilemap at that same start point so the connection is actually walkable.
    GameObject PlaceRoomAt(GameObject roomPrefab, Vector3 targetPos, Vector3 incomingDirection)
    {
        GameObject roomGO = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, transform);

        Vector3Int startCell = GetStartCellForDirection(roomGO, incomingDirection);
        Vector3 startTileWorldPos = CellToWorldPosition(roomGO, startCell);
        Vector3 correction = targetPos - startTileWorldPos;
        roomGO.transform.position += correction;

        CarveEntranceHole(roomGO, startCell, incomingDirection);

        return roomGO;
    }

    Bounds GetRoomWorldBounds(GameObject room)
    {
        Tilemap tilemap = GetLogicTilemap(room);
        tilemap.CompressBounds();
        BoundsInt cellBounds = tilemap.cellBounds;

        Vector3 min = tilemap.CellToWorld(cellBounds.min);
        Vector3 max = tilemap.CellToWorld(cellBounds.max);

        Bounds b = new Bounds();
        b.SetMinMax(
            new Vector3(Mathf.Min(min.x, max.x), Mathf.Min(min.y, max.y), -0.01f),
            new Vector3(Mathf.Max(min.x, max.x), Mathf.Max(min.y, max.y), 0.01f)
        );
        return b;
    }

    bool WouldOverlap(Bounds candidate)
    {
        Bounds shrunk = candidate;
        shrunk.Expand(new Vector3(-overlapShrink * 2f, -overlapShrink * 2f, 0f));

        foreach (var b in roomBounds.Values)
        {
            if (shrunk.Intersects(b))
            {
                return true;
            }
        }
        return false;
    }

    void RegisterBounds(GameObject room)
    {
        roomBounds[room] = GetRoomWorldBounds(room);
    }

    void UnregisterBounds(GameObject room)
    {
        roomBounds.Remove(room);
    }

    void TryCleanupOldRooms()
    {
        while (spawnedRooms.Count > roomsAheadBuffer + 3)
        {
            GameObject old = spawnedRooms.Peek(); // peek first, don't lose it if we're not destroying yet
            if (Vector3.Distance(player.position, old.transform.position) > spawnAheadDistance * 3f)
            {
                spawnedRooms.Dequeue();
                UnregisterBounds(old);
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

    // Same idea as GetLogicTilemap but for the "Ground" tilemap, used for carving the
    // entrance hole once a room's connection point has been chosen.
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

    // Returns every cell in the room's Logic tilemap that matches the given tile. Used so
    // a room can have several start tiles (one per direction it supports) instead of just one.
    public List<Vector3Int> GetAllTileCellPositions(GameObject room, TileBase tile)
    {
        Tilemap logic = GetLogicTilemap(room);
        List<Vector3Int> positions = new List<Vector3Int>();

        foreach (var pos in logic.cellBounds.allPositionsWithin)
        {
            if (logic.GetTile(pos) == tile)
            {
                positions.Add(pos);
            }
        }

        return positions;
    }

    // Finds the start tile cell whose rotation matches the direction we need for this
    // placement (e.g. if the room is being used as a "down room", find the start tile
    // rotated to face down). Falls back to the first start tile found if none match.
    Vector3Int GetStartCellForDirection(GameObject room, Vector3 direction)
    {
        Tilemap logic = GetLogicTilemap(room);
        List<Vector3Int> starts = GetAllTileCellPositions(room, startTile);

        foreach (var pos in starts)
        {
            if (GetTileRotation(pos, logic) == direction)
            {
                return pos;
            }
        }

        if (starts.Count > 0)
        {
            Debug.LogWarning("No start tile facing " + direction + " found in room " + room.name + ", falling back to first start tile found.");
            return starts[0];
        }

        Debug.LogWarning("Room has no start tile at all: " + room.name);
        return Vector3Int.zero;
    }

    public Vector3 GetTileWorldPosition(GameObject room, TileBase tile)
    {
        Tilemap ground = GetLogicTilemap(room);
        Vector3Int pos = GetTileCellPosition(room, tile);
        return ground.CellToWorld(pos) + ground.tileAnchor;

    }

    // Same as GetTileWorldPosition but for a specific cell we already know, rather than
    // one we have to search for by tile type (needed once we've picked a specific start cell).
    Vector3 CellToWorldPosition(GameObject room, Vector3Int cellPos)
    {
        Tilemap logic = GetLogicTilemap(room);
        return logic.CellToWorld(cellPos) + logic.tileAnchor;
    }

    // Carves a 3-wide gap into the room's Ground tilemap at the start cell that was
    // actually used for this connection, so the player can walk through it. If the room
    // connects vertically (up/down start) the gap runs horizontally (3 tiles across);
    // if it connects horizontally (left/right start) the gap runs vertically (3 tiles tall).
    void CarveEntranceHole(GameObject room, Vector3Int startCell, Vector3 incomingDirection)
    {
        Tilemap ground = GetGroundTilemap(room);
        if (ground == null)
        {
            Debug.LogWarning("No Ground tilemap found on room: " + room.name);
            return;
        }

        bool horizontalHole = (incomingDirection == Vector3.up || incomingDirection == Vector3.down);

        for (int offset = -1; offset <= 1; offset++)
        {
            Vector3Int cell = horizontalHole
                ? new Vector3Int(startCell.x + offset, startCell.y, startCell.z)
                : new Vector3Int(startCell.x, startCell.y + offset, startCell.z);

            ground.SetTile(cell, null);
        }
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

    Vector3 GetTileRotation(Vector3Int cellPosition, Tilemap tilemap)
    {
        float z = Mathf.Round(tilemap.GetTransformMatrix(cellPosition).rotation.eulerAngles.z);
        z = ((z % 360) + 360) % 360; // normalize

        if (Mathf.Approximately(z, 0)) return Vector3.right;
        if (Mathf.Approximately(z, 90)) return Vector3.up;
        if (Mathf.Approximately(z, 180)) return Vector3.left;
        if (Mathf.Approximately(z, 270)) return Vector3.down;

        Debug.LogError("Unrecognized tile rotation angle: " + z);
        return Vector3.right; // fallback
    }
}