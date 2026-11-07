using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "Change Time Effect", menuName = "Custom/Choice Effects/Change Time Effect")]
public class ChangeTimeEffect : ChoiceEffect
{
    public float timeChangeAmount;
    public bool isMultiplicative;

    public override void Apply()
    {
        if (isMultiplicative)
        {
            GameManager.Instance.runData.time *= timeChangeAmount;
        }
        else
        {
            GameManager.Instance.runData.time += timeChangeAmount;
        }
    }
}
