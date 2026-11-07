using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Mod Stat Effect", menuName = "Custom/Choice Effects/Mod Stat Effect")]
public class ModStatEffect : ChoiceEffect
{
    public List<StatModification> statModifications;

    public override void Apply()
    {
        StatsManager.Instance.AddModifiers(statModifications);
    }
}
