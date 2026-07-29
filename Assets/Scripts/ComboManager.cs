using UnityEngine;

public class ComboManager : MonoBehaviour
{

    public float comboSustainTime;
    float comboMultIncrease = 0.1f;
    float comboTimer;
    float currentMult;

    DamagePopupSpawner popupSpawner;
    PlayerItems items;

    void Start()
    {

        items = FindFirstObjectByType<PlayerItems>();
        popupSpawner = FindFirstObjectByType<DamagePopupSpawner>();
        comboSustainTime *= 1 + (0.2f * items.CountItem("Freezer"));
        comboMultIncrease += (0.1f * items.CountItem("Combo Booster"));
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
