using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;

public class CombatEncounter : MonoBehaviour
{
    // An arenas first child has to be a parent containing all of the spawnpoints as children.
    // The second child of the arena is the elite spawn point.

    CombatDatabase db;
    List<Vector3> spawnPositions;
    RankManager rankManager;

    float spawnTimer;
    int waveCounter = 0;
    float surviveTimer;

    [HideInInspector] public float timer = 0f;
    [HideInInspector] public float kills = 0f;

    public List<EncounterParams> encounterParams;
    EncounterParams curParams;
    [HideInInspector] public EncounterType encounterType;

    public Transform enemyParent;
    GameObject elite;

    GameManager gameManager => GameManager.Instance;

    public TMP_Text goalText;

    void Start()
    {
        rankManager = FindFirstObjectByType<RankManager>();
        db = FindFirstObjectByType<CombatDatabase>();

        encounterType = db.GetEncounterType();
        curParams = encounterParams[(int)encounterType];

        surviveTimer = curParams.requiredSurvivalTime;

        spawnTimer = -1;
        GameObject arena = Instantiate(db.GetArena(), Vector3.zero, Quaternion.identity);
        spawnPositions = new List<Vector3>();

        foreach (Transform child in arena.transform.GetChild(0))
        {
            spawnPositions.Add(child.position);
        }

        if (encounterType == EncounterType.Elite)
        {
            Vector3 eliteSpawn = arena.transform.GetChild(1).position;
            goalText.text = "Kill The Elite";
            elite = Instantiate(db.GetElite(), eliteSpawn, Quaternion.identity);
        }
        else if (encounterType == EncounterType.KillAll)
        {
            goalText.text = "Kill All The Enemies";
        }
    }



    void Update()
    {
        timer += Time.deltaTime;
        surviveTimer -= Time.deltaTime;
        spawnTimer -= Time.deltaTime;

        if (encounterType == EncounterType.Survive)
        {
            goalText.text = "Survive " + Mathf.Ceil(surviveTimer) + "s";
        }

        if (spawnTimer < 0)
        {
            SpawnEnemyWave();
        }

        // Check Win Conditions
        switch (encounterType)
        {
            case EncounterType.KillAll:
                if (waveCounter >= curParams.totalEnemyWaves && enemyParent.childCount == 0) rankManager.Continue();
                break;

            case EncounterType.Survive:
                if (surviveTimer < 0f) rankManager.Continue();
                break;

            case EncounterType.Elite:
                if (elite == null) rankManager.Continue();
                break;

        }

    }

    void SpawnEnemyWave()
    {
        if (encounterType == EncounterType.KillAll && waveCounter >= curParams.totalEnemyWaves) return;
        int amountToSpawn = Mathf.Min(curParams.enemySpawnAmount, spawnPositions.Count);

        // Shuffles list to ensure random spawn points are used
        spawnPositions.OrderBy(_ => Random.Range(0f, 1f));

        for (int i = 0; i < amountToSpawn; i++)
        {
            Instantiate(db.GetEnemy(), spawnPositions[i], Quaternion.identity, enemyParent);
        }

        waveCounter++;
        spawnTimer = curParams.enemySpawnTime;
    }

    private void OnEnable()
    {
        gameManager.OnEnemyKilled += OnKill;
    }

    private void OnDisable()
    {
        gameManager.OnEnemyKilled -= OnKill;
    }

    private void OnKill()
    {
        kills++;
    }
}

public enum EncounterType
{
    KillAll,
    Elite,
    Survive
}

[System.Serializable]
public struct EncounterParams
{
    public float enemySpawnTime;
    public int enemySpawnAmount;

    [Header("Kill All")]
    public int totalEnemyWaves;

    [Header("Survive")]
    public float requiredSurvivalTime;
}
