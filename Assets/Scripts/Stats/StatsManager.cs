using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour
{

    public static StatsManager Instance;

    public Stats startStats;
    [HideInInspector] public Stats stats;
    Stats tempStats;

    Dictionary<StatType, float> statMultipliers;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        stats = startStats;
    }

    void ResetMultipliers()
    {
        if (statMultipliers == null)
            statMultipliers = new Dictionary<StatType, float>();

        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            statMultipliers[stat] = 1f;
        }
    }

    public float GetStat(StatType type)
    {
        switch (type)
        {
            case StatType.Damage:
                return tempStats.damage;

            case StatType.ComboMultIncrease:
                return tempStats.comboMultIncrease;

            case StatType.Speed:
                return tempStats.speed;

            case StatType.ComboSustainTime:
                return tempStats.comboSustainTime;
            default:
                return -1;
        }
    }

    public void SetStat(StatType type, float value)
    {
        switch (type)
        {
            case StatType.Damage:
                tempStats.damage = value;
                break;

            case StatType.ComboMultIncrease:
                tempStats.comboMultIncrease = value;
                break;

            case StatType.Speed:
                tempStats.speed = value;
                break;

            case StatType.ComboSustainTime:
                tempStats.comboSustainTime = value;
                break;

        }
    }

    public void CalculateStats()
    {
        tempStats = stats.Clone();
        ResetMultipliers();

        List<IOnSetStats> items = InventoryManager.Instance.GetInterfaceList<IOnSetStats>();
        foreach (IOnSetStats item in items)
        {
            item.OnSetStats(statMultipliers);
        }

        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            float value = GetStat(stat);

            if (statMultipliers.TryGetValue(stat, out float multiplier))
            {
                SetStat(stat, value * multiplier);
            }
        }
        stats = tempStats;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "SampleScene" || scene.name == "Tutorial" || scene.name == "Boss Fight")
        {
            CalculateStats();
        }
    }
}
