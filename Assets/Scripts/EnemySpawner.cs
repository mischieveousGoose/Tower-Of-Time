using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public List<GameObject> enemies;

    public void SpawnEnemies(GameObject room)
    {
        // Get the SpawnPoints parent object
        Transform spawnPointsParent = room.transform.Find("SpawnPoints");
        if (spawnPointsParent == null)
        {
            Debug.LogWarning("No SpawnPoints found in room: " + room.name);
            return;
        }

        // Iterate through each child of the SpawnPoints parent
        foreach (Transform spawnPoint in spawnPointsParent)
        {
            // Randomly select an enemy from the list
            GameObject enemyPrefab = enemies[Random.Range(0, enemies.Count)];
            // Instantiate the enemy at the spawn point's position and rotation
            Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
        }
    }
}
