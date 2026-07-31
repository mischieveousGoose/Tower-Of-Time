using System.Collections.Generic;
using UnityEngine;

public interface IOnSetStats
{
    public void OnSetStats(Dictionary<StatType, float> curStats);
}
