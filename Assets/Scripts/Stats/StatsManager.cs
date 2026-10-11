
using System;
using System.Collections.Generic;
using UnityEngine;

public class StatsManager : MonoBehaviour
{
    public static StatsManager Instance;

    public Stats startStats;
    public Stats curStats;

    // The base values before item bonuses.
    private Stats baseStats;

    // Active item modifiers, identified by a unique ID.
    private readonly Dictionary<int, List<ActiveStatModifier>>
        activeModifiers = new();

    private int nextModifierId = 0;

    private class ActiveStatModifier
    {
        public StatType stat;
        public StatModificationType type;
        public float value;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        ResetStats();
    }

    public void ResetStats()
    {
        if (GameManager.Instance.character != null)
            baseStats = GameManager.Instance.character.stats;
        else
            baseStats = startStats;

        curStats = CopyStats(baseStats);
        activeModifiers.Clear();
    }

    private Stats CopyStats(Stats source)
    {
        // IMPORTANT:
        // This assumes Stats is a class with the listed fields.
        // Copy every field in your actual Stats definition here.
        return new Stats
        {
            damage = source.damage,
            comboMultIncrease = source.comboMultIncrease,
            speed = source.speed,
            comboSustainTime = source.comboSustainTime,
            enemyTimeReward = source.enemyTimeReward,
            shopPrices = source.shopPrices,
            maxTime = source.maxTime
        };
    }

    public int AddModifiers(List<StatModification> modifications)
    {
        int id = ++nextModifierId;
        var modifiers = new List<ActiveStatModifier>();

        foreach (var mod in modifications)
        {
            modifiers.Add(new ActiveStatModifier
            {
                stat = mod.stat,
                type = mod.modType,
                value = mod.mod
            });
        }

        activeModifiers.Add(id, modifiers);
        RecalculateStats();

        return id;
    }

    public void RemoveModifiers(int id)
    {
        if (activeModifiers.Remove(id))
            RecalculateStats();
    }

    public float GetStat(StatType type)
    {
        return type switch
        {
            StatType.Damage => curStats.damage,
            StatType.ComboMultIncrease => curStats.comboMultIncrease,
            StatType.Speed => curStats.speed,
            StatType.ComboSustainTime => curStats.comboSustainTime,
            StatType.EnemyTimeReward => curStats.enemyTimeReward,
            StatType.ShopPrices => curStats.shopPrices,
            StatType.MaxTime => curStats.maxTime,
            _ => -1f
        };
    }

    private void SetStat(StatType type, float value)
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

    private void RecalculateStats()
    {
        curStats = CopyStats(baseStats);

        foreach (StatType stat in Enum.GetValues(typeof(StatType)))
        {
            float flatBonus = 0f;
            float multiplier = 1f;

            foreach (var entry in activeModifiers.Values)
            {
                foreach (var mod in entry)
                {
                    if (mod.stat != stat)
                        continue;

                    switch (mod.type)
                    {
                        case StatModificationType.Add:
                            flatBonus += mod.value;
                            break;

                        case StatModificationType.Mult:
                            multiplier *= mod.value;
                            break;
                    }
                }
            }

            SetStat(stat, (GetStatFromBase(stat) + flatBonus) * multiplier);
        }
    }

    private float GetStatFromBase(StatType type)
    {
        return type switch
        {
            StatType.Damage => baseStats.damage,
            StatType.ComboMultIncrease => baseStats.comboMultIncrease,
            StatType.Speed => baseStats.speed,
            StatType.ComboSustainTime => baseStats.comboSustainTime,
            StatType.EnemyTimeReward => baseStats.enemyTimeReward,
            StatType.ShopPrices => baseStats.shopPrices,
            StatType.MaxTime => baseStats.maxTime,
            _ => 0f
        };
    }
}
