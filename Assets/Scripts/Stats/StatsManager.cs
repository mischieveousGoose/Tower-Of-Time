using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour
{

    public static StatsManager Instance;

    public Stats startStats;
    [HideInInspector] public Stats stats;
    public Stats curStats;

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

    public void ResetStats()
    {
        stats = startStats;
        curStats = null;
        ResetMultipliers();
    }

    public float GetStat(StatType type)
    {
        switch (type)
        {
            case StatType.Damage:
                return curStats.damage;

            case StatType.ComboMultIncrease:
                return curStats.comboMultIncrease;

            case StatType.Speed:
                return curStats.speed;

            case StatType.ComboSustainTime:
                return curStats.comboSustainTime;

            case StatType.EnemyTimeReward:
                return curStats.enemyTimeReward;

            case StatType.ShopPrices:
                return curStats.shopPrices;

            default:
                return -1;
        }
    }

    public void SetStat(StatType type, float value)
    {
        switch (type)
        {
            case StatType.Damage:
                curStats.damage = value;
                break;

            case StatType.ComboMultIncrease:
                curStats.comboMultIncrease = value;
                break;

            case StatType.Speed:
                curStats.speed = value;
                break;

            case StatType.ComboSustainTime:
                curStats.comboSustainTime = value;
                break;
            case StatType.EnemyTimeReward:
                curStats.enemyTimeReward = value;
                break;
            case StatType.ShopPrices:
                curStats.shopPrices = value;
                break;


        }
    }

    public void CalculateStats()
    {
        ResetStats();
        curStats = stats.Clone();


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
        stats = curStats;
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        GameManager.Instance.OnReset += ResetStats;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        GameManager.Instance.OnReset -= ResetStats;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Menu" && scene.name != "Cutscene")
        {
            CalculateStats();
        }
    }
}
