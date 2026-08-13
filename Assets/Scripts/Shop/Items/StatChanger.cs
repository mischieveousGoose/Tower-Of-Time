using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Stat Changer")]
public class StatChanger : Item, IOnSetStats
{
    public List<StatModification> statModifications;

    public void OnSetStats(Dictionary<StatType, StatMultiplier> multipliers)
    {

        foreach (StatModification mod in statModifications)
        {
            if (mod.modType == StatModificationType.Add)
            {
                multipliers[mod.stat] = new StatMultiplier(multipliers[mod.stat].value + mod.mod, multipliers[mod.stat].mult);
            }
            else if (mod.modType == StatModificationType.AddMult)
            {
                multipliers[mod.stat] = new StatMultiplier(multipliers[mod.stat].value, multipliers[mod.stat].mult + mod.mod);
            }
            else if (mod.modType == StatModificationType.MultMult)
            {
                multipliers[mod.stat] = new StatMultiplier(multipliers[mod.stat].value, multipliers[mod.stat].mult * mod.mod);
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
    Add,
    AddMult,
    MultMult
}
