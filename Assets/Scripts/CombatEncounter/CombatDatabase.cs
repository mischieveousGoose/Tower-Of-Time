using System.Collections.Generic;
using UnityEngine;
using System;

public class CombatDatabase : MonoBehaviour
{
    public List<GameObject> arenas;
    public List<GameObject> enemies;
    public List<GameObject> elites;

    public GameObject GetArena()
    {
        return arenas[UnityEngine.Random.Range(0, arenas.Count)];
    }

    public GameObject GetEnemy()
    {
        return enemies[UnityEngine.Random.Range(0, enemies.Count)];
    }

    public GameObject GetElite()
    {
        return elites[UnityEngine.Random.Range(0, elites.Count)];
    }

    public EncounterType GetEncounterType()
    {
        int length = Enum.GetValues(typeof(EncounterType)).Length;
        return (EncounterType)UnityEngine.Random.Range(0, length);
    }
}
