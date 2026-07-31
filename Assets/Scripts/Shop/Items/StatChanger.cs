using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Stat Changer")]
public class StatChanger : Item, IOnSetStats
{
    public List<StatModification> statModifications;

    public void OnSetStats(Dictionary<StatType, float> multipliers)
    {

        foreach (StatModification mod in statModifications)
        {
            if (mod.modType == StatModificationType.Addition)
            {
                multipliers[mod.stat] += mod.mod;
            }
            else
            {
                multipliers[mod.stat] *= mod.mod;
            }
        }

    }
}

[System.Serializable]
public struct StatModification
{
    public StatType stat;
    public float mod;
    public StatModificationType modType;
}

[System.Serializable]
public enum StatModificationType
{
    Addition,
    Multiplication
}
