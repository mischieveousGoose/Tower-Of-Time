using UnityEngine;


public class Enemy : MonoBehaviour, Attackable
{
    public float startHealth;
    float health;

    public float timeReward;
    SFXManager sfxManager;
    ComboManager comboManager;
    float iFrameTimer;
    SpriteRenderer sr;
    SettingsManager settingsManager;

    void Start()
    {
        settingsManager = FindFirstObjectByType<SettingsManager>();
        sfxManager = FindFirstObjectByType<SFXManager>();
        sr = GetComponent<SpriteRenderer>();
        health = startHealth;
        comboManager = FindFirstObjectByType<ComboManager>();
    }

    void Update()
    {
        iFrameTimer -= Time.deltaTime;

        if (iFrameTimer > 0f)
        {
            sr.color = Color.red;
            return;
        }
        sr.color = Color.white;

    }

    public void TakeDamage(float damage)
    {

        if (iFrameTimer > 0f)
        {
            return;
        }

        health -= damage;
        iFrameTimer = 0.5f;
        if (health <= 0)
        {
            if (comboManager == null) comboManager = FindFirstObjectByType<ComboManager>();

            float mult = 1f;
            mult *= comboManager.GetCombo(transform.position);
            mult *= StatsManager.Instance.GetStat(StatType.EnemyTimeReward);
            FindFirstObjectByType<Player>().AddTime(timeReward * mult);
            comboManager.GainPopup(mult * timeReward, transform.position);
            Destroy(gameObject);
        }
    }
}
