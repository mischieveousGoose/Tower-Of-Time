
using UnityEngine;

[System.Serializable]
public struct Stats
{
    public float maxTime;
    public float damage;
    public float speed;

    public float shopPrices;
    public float enemyTimeReward;

    [Header("Combo")]
    public float comboSustainTime;
    public float comboMultIncrease;


    public Stats Clone()
    {
        Stats output = new Stats();
        output.damage = damage;
        output.speed = speed;

        output.comboMultIncrease = comboMultIncrease;
        output.comboSustainTime = comboSustainTime;
        output.shopPrices = shopPrices;
        output.enemyTimeReward = enemyTimeReward;
        output.maxTime = maxTime;

        return output;
    }

}

public enum StatType
{
    Damage,
    Speed,
    ComboSustainTime,
    ComboMultIncrease,
    ShopPrices,
    EnemyTimeReward,
    MaxTime
}
