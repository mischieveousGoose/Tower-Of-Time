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

    public GameObject deathParticles;



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

            FindFirstObjectByType<CameraController>().ScreenShake(10f, 0.2f);

            comboManager.GainPopup(mult * timeReward, transform.position);
            GameManager.Instance.OnEnemyKilled?.Invoke();

            if (deathParticles != null)
            {
                float angle = FindFirstObjectByType<PlayerCombat>().GetMouseAngle();
                Instantiate(deathParticles, transform.position, Quaternion.Euler(0, 0, angle));
            }

            Destroy(gameObject);
        }
    }
}
