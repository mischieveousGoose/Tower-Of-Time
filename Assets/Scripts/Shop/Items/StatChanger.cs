
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Custom/Items/Stat Changer")]
public class StatChanger : Item, IOnBuy, IOnRemove
{
    public List<StatModification> statModifications;

    private readonly List<int> activeModifierIds = new();

    public void OnBuy(ShopManager shopManager)
    {
        int id = StatsManager.Instance.AddModifiers(
            statModifications
        );

        activeModifierIds.Add(id);
    }

    public void OnRemove()
    {
        if (activeModifierIds.Count == 0)
            return;

        int lastIndex = activeModifierIds.Count - 1;
        int id = activeModifierIds[lastIndex];

        StatsManager.Instance.RemoveModifiers(id);
        activeModifierIds.RemoveAt(lastIndex);
    }
}

[Serializable]
public struct StatModification
{
    public StatType stat;
    public float mod;
    public StatModificationType modType;
}

public enum StatModificationType
{
    Add,
    Mult
}
