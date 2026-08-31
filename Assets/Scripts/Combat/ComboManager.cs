using UnityEngine;

public class ComboManager : MonoBehaviour
{

    public float comboSustainTime;
    float comboMultIncrease = 0.1f;
    float comboTimer;
    float currentMult;

    DamagePopupSpawner popupSpawner;


    void Start()
    {
        popupSpawner = FindFirstObjectByType<DamagePopupSpawner>();
        comboSustainTime = StatsManager.Instance.GetStat(StatType.ComboSustainTime);
        comboMultIncrease = StatsManager.Instance.GetStat(StatType.ComboMultIncrease);
    }

    public void GainPopup(float gain, Vector3 pos)
    {
        if (gain <= 0) return;
        popupSpawner.SpawnPopup(pos + Vector3.up + (Vector3.right * 2), "+" + RoundFloat(gain).ToString(), Color.green);
    }

    public float GetCombo(Vector3 enemyPos)
    {
        if (comboTimer > 0)
        {
            currentMult += comboMultIncrease;
            popupSpawner.SpawnPopup(enemyPos + Vector3.up, "x" + currentMult.ToString());
        }
        else
        {
            currentMult = 1;
        }

        comboTimer = comboSustainTime;
        return currentMult;
    }

    float RoundFloat(float num) { return Mathf.Round(num * 100) / 100; }

    void Update()
    {
        comboTimer -= Time.deltaTime;
    }


}
