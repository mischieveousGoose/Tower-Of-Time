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

    float[] timeRewardMults = new float[] { 1.5f, 1.3f, 0.7f };


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

        sfxManager.PlaySFX(5);


        health -= damage;
        iFrameTimer = 0.5f;
        if (health <= 0)
        {

            float mult = 1f;
            mult *= comboManager.GetCombo(transform.position);
            mult *= timeRewardMults[settingsManager.difficulty];
            FindFirstObjectByType<Timer>().AddTime(timeReward * mult);
            comboManager.GainPopup(mult * timeReward, transform.position);
            Destroy(gameObject);
        }
    }
}
