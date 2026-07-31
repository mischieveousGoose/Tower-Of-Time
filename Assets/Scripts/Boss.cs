
using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class Boss : MonoBehaviour, Attackable
{
    public Slider healthbar;
    public float maxHealth;
    public float changeStateTime;
    float health;
    float iFrameCounter;
    SFXManager sfxManager;

    public float bombSpeed;
    float bombTimer;
    public float bombVel;

    public GameObject enemyPrefab;
    public GameObject barrier;

    public float runSpeed;

    public float spawnTime;
    float spawnTimer;
    public GameObject jumpingEnemy;
    public float enemyLaunchVel;



    float stateTimer;

    Transform player;

    BossState state;
    public GameObject music;
    Rigidbody2D rb;

    bool startedFight;

    [Header("Falling Bullets")]
    public float leftSpawnBarrier;
    public float rightSpawnBarrier;
    public GameObject clock;
    public float spawnRate;
    float spawnRateTimer;
    public float spawnY;
    public float fallSpeed;

    public GameObject brock;

    BossState lastState;

    SpriteRenderer sr;



    public void TakeDamage(float damage)
    {
        if (iFrameCounter > 0) return;
        sfxManager.PlaySFX(5);
        iFrameCounter = 0.4f;
        health -= damage;
        healthbar.value = health / maxHealth;
    }

    void Start()
    {
        music.SetActive(false);
        sfxManager = FindFirstObjectByType<SFXManager>();
        sr = GetComponent<SpriteRenderer>();
        sr.enabled = false;

        healthbar.transform.parent.gameObject.SetActive(false);
        barrier.SetActive(false);

        rb = GetComponent<Rigidbody2D>();
        state = BossState.Idle;
        lastState = BossState.Idle;
        health = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        brock.SetActive(true);
    }

    void Update()
    {
        if (health < 0)
        {

            GetComponent<SpriteRenderer>().enabled = false;
            FindFirstObjectByType<TimeManager>().Win();
        }

        if (!startedFight) return;
        stateTimer -= Time.deltaTime;
        iFrameCounter -= Time.deltaTime;

        if (stateTimer < 0f)
        {
            stateTimer = changeStateTime;
            while (state == lastState)
                state = (BossState)Random.Range(0, 4);
            lastState = state;
        }



        switch (state)
        {
            case BossState.Running:
                Running();
                break;

            case BossState.Bombing:
                Bombing();
                break;

            case BossState.FallingBullets:
                FallingBullets();
                break;

            case BossState.Spawning:
                Spawning();
                break;

        }


    }

    public void Menu()
    {
        FindFirstObjectByType<LevelLoader>().LoadLevel("Menu");
    }


    public void StartFight()
    {
        music.SetActive(true);
        sr.enabled = true;
        barrier.SetActive(true);
        startedFight = true;
        healthbar.transform.parent.gameObject.SetActive(true);
        brock.SetActive(false);
    }

    void Running()
    {
        rb.linearVelocity = Vector2.right * (player.position.x < transform.position.x ? -1 : 1f) * runSpeed;
    }

    void Bombing()
    {
        rb.linearVelocity = Vector2.right * (player.position.x < transform.position.x ? -1 : 1f) * runSpeed * 0.5f;
        bombTimer -= Time.deltaTime;
        if (bombTimer < 0f)
        {
            bombTimer = bombSpeed;
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

            enemy.GetComponent<Rigidbody2D>().linearVelocity = Random.insideUnitCircle * bombVel;
        }
    }

    void Spawning()
    {
        rb.linearVelocity = Vector2.right * (player.position.x < transform.position.x ? -1 : 1f) * runSpeed * 0.25f;
        spawnTimer -= Time.deltaTime;
        if (spawnTimer < 0f)
        {
            spawnTimer = spawnTime;
            GameObject enemy = Instantiate(jumpingEnemy, transform.position, Quaternion.identity);
            float dir = Random.Range(0, 2) == 0 ? -1 : 1;

            enemy.GetComponent<Rigidbody2D>().linearVelocity = Vector2.right * enemyLaunchVel * dir;
        }
    }

    void FallingBullets()
    {
        spawnRateTimer -= Time.deltaTime;
        if (spawnRateTimer < 0f)
        {
            spawnRateTimer = spawnRate;
            Vector3 spawnPos = new Vector3(Random.Range(leftSpawnBarrier, rightSpawnBarrier), spawnY, 0f);
            GameObject proj = Instantiate(clock, spawnPos, Quaternion.identity);
            proj.GetComponent<Rigidbody2D>().linearVelocity = Vector2.down * fallSpeed;

        }
    }




}



public enum BossState
{
    Running,
    FallingBullets,
    Bombing,
    Spawning,
    Idle
}