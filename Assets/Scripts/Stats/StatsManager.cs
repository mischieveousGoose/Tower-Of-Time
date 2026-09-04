using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StatsManager : MonoBehaviour
{

    public static StatsManager Instance;

    public Stats startStats;
    public Stats curStats;

    Dictionary<StatType, StatMultiplier> statMultipliers;

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
        curStats = startStats;
    }

    void ResetMultipliers()
    {
        if (statMultipliers == null)
            statMultipliers = new Dictionary<StatType, StatMultiplier>();

        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            statMultipliers[stat] = new StatMultiplier(GetStat(stat), 1f);
        }
    }

    public void ResetStats()
    {
	if(GameManager.Instance.character != null) 
	    curStats = GameManager.Instance.character.stats;
	else
        curStats = startStats;
        ResetMultipliers();
    }

    public void SetStats(Stats stats)
    {
	this.startStats = stats;
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

            case StatType.MaxTime:
                return curStats.maxTime;

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

            case StatType.MaxTime:
                curStats.maxTime = value;
                break;


        }
    }

    public void CalculateStats()
    {
        ResetStats();
        Debug.Log("Recalculating Stats");

        List<IOnSetStats> items = InventoryManager.Instance.GetInterfaceList<IOnSetStats>();
        foreach (IOnSetStats item in items)
        {
            item.OnSetStats(statMultipliers);
        }

        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            if (statMultipliers.TryGetValue(stat, out StatMultiplier multiplier))
            {
                SetStat(stat, multiplier.value * multiplier.mult);
            }
        }

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

public struct StatMultiplier
{
    public float value;
    public float mult;

    public StatMultiplier(float value, float mult)
    {
        this.value = value;
        this.mult = mult;
    }
}
