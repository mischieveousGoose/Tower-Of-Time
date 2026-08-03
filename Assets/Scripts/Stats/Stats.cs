
using UnityEngine;

[System.Serializable]
public class Stats
{
    public float damage;
    public float speed;

    public float shopPrices;
    public float enemyTimeReward;

    [Header("Combo")]
    public float comboSustainTime;
    public float comboMultIncrease;

    public Stats()
    {
        damage = 1f;
        speed = 12f;
        comboSustainTime = 2f;
        comboMultIncrease = 0.1f;
        shopPrices = 1f;
        enemyTimeReward = 1f;
    }

    public Stats Clone()
    {
        Stats output = new Stats();
        output.damage = damage;
        output.speed = speed;

        output.comboMultIncrease = comboMultIncrease;
        output.comboSustainTime = comboSustainTime;
        output.shopPrices = shopPrices;
        output.enemyTimeReward = enemyTimeReward;

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
    EnemyTimeReward
}
