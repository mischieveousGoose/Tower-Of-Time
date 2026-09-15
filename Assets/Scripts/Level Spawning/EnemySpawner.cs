using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public TileBase enemyTile;
    public List<GameObject> enemyPrefabs;

    public void SpawnEnemies(GameObject room)
    {
        Tilemap tilemap = room.transform.Find("Grid").Find("Logic").GetComponent<Tilemap>();
        if (tilemap == null) return;

        BoundsInt bounds = tilemap.cellBounds;
        TileBase[] allTiles = tilemap.GetTilesBlock(bounds);

        for (int x = 0; x < bounds.size.x; x++)
        {
            for (int y = 0; y < bounds.size.y; y++)
            {
                TileBase tile = allTiles[x + y * bounds.size.x];
                if (tile == enemyTile)
                {
                    Vector3Int localPlace = new Vector3Int(x + bounds.x, y + bounds.y, 0);
                    Vector3 place = tilemap.CellToWorld(localPlace) + tilemap.tileAnchor;
                    SpawnEnemy(place);
                    tilemap.SetTile(localPlace, null); // Remove the enemy tile after spawning
                }
            }
        }
    }

    public void SpawnEnemy(Vector3 position)
    {
        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
        Instantiate(enemyPrefab, position, Quaternion.identity);
    }
}
