
using UnityEngine;

[System.Serializable]
public class Stats
{
    public float damage;
    public float speed;

    [Header("Combo")]
    public float comboSustainTime;
    public float comboMultIncrease;

    public Stats()
    {
        damage = 0f;
        speed = 12f;
        comboSustainTime = 2f;
        comboMultIncrease = 0.1f;
    }

    public Stats Clone()
    {
        Stats output = new Stats();
        output.damage = damage;
        output.speed = speed;

        output.comboMultIncrease = comboMultIncrease;
        output.comboSustainTime = comboSustainTime;

        return output;
    }

}

public enum StatType
{
    Damage,
    Speed,
    ComboSustainTime,
    ComboMultIncrease
}
