using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CEOBoss : Boss<CEOBoss.State>
{
    /* 
    Boss State Ideas
    Slash attack with suitcase
    Spawns Enemies
    Stunned
    Makes Shockwaves on the floor
    */

    Transform player;
    Rigidbody2D rb;

    [Header("Slashing")]

    public GameObject slashHitbox;
    public int totalSlashes;
    int slashes;

    public float slashLength;
    public float slashSpeed;
    float slashTimer = 0;
    public float slashTimeDelay;

    float slashDir = 0;

    [Header("Spawning")]
    public Transform spawnPoint;
    public float suitcaseVel;
    public GameObject suitcase;
    public int spawnAmount;
    public float preSpawnDelay, postSpawnDelay;

    float spawnTimer;
    float lastSpawnTimer;

    [Header("Shockwaves")]
    public int totalBounces;
    int bounces;

    public float preBounceDelay;   // telegraph pause before rising, feet on the ground
    public float bounceUpTime;     // time to reach the top of the bounce
    public float bounceDownTime;   // time to fall back down
    public float bounceHeight;     // how high the boss rises
    public float postLandDelay;    // pause after landing before the next bounce (or before ending)

    public GameObject shockwavePrefab;
    public float shockwaveSpeed;
    public float shockwaveLifetime;
    public float shockWaveY;

    enum ShockwavePhase { PreDelay, Rising, Falling, PostDelay }
    ShockwavePhase shockwavePhase;
    float shockwaveTimer;
    float shockwaveGroundY;

    [Header("Shooting")]
    public GameObject projectile;
    public Transform shootPoint;
    public float projectileSpeed;
    public int totalShots;
    int shots;

    public float preShootDelay;  // delay before the first shot
    public float shotInterval;   // time between each shot
    public float postShootDelay; // delay after the last shot before ending

    float shootTimer;


    public enum State
    {
        Slashing,
        Spawning,
        Shockwaves,
        Shooting
    }

    void Start()
    {
        spawnTimer = preSpawnDelay;
        lastSpawnTimer = spawnTimer;
        slashTimer = slashLength;
        slashes = totalSlashes;

        bounces = totalBounces;
        shockwavePhase = ShockwavePhase.PreDelay;
        shockwaveTimer = preBounceDelay;

        shots = totalShots;
        shootTimer = preShootDelay;

        player = FindFirstObjectByType<Player>().transform;
        rb = GetComponent<Rigidbody2D>();
        shockwaveGroundY = transform.position.y;

        RegisterState(State.Slashing, Slashing);
        RegisterState(State.Spawning, Spawning);
        RegisterState(State.Shockwaves, Shockwaves);
        RegisterState(State.Shooting, Shooting);
        GetNewState();
    }

    void Slashing()
    {
        Debug.Log("Slashing");
        slashTimer -= Time.deltaTime;

        if (slashTimer <= -slashTimeDelay)
        {
            slashes--;
            slashDir = 0;
            slashTimer = slashLength;
            if (slashes <= 0)
            {
                slashTimer = slashLength;
                slashes = totalSlashes;
                GetNewState();
                return;
            }
        }

        if (slashDir == 0) slashDir = player.position.x < transform.position.x ? -1 : 1;

        if (slashTimer < 0f)
        {
            slashHitbox.SetActive(false);
            rb.linearVelocity = Vector2.zero;
        }
        else
        {
            slashHitbox.SetActive(true);
            float slashX = Mathf.Abs(slashHitbox.transform.localPosition.x) * slashDir;
            slashHitbox.transform.localPosition = new Vector3(slashX, slashHitbox.transform.localPosition.y, 0f);
            rb.linearVelocity = new Vector2(slashDir * slashSpeed, 0f);
        }
    }


    void Spawning()
    {
        spawnTimer -= Time.deltaTime;

        if (lastSpawnTimer >= 0 && spawnTimer <= 0)
        {
            //Spawn
            for (int i = 0; i < spawnAmount; i++)
            {
                GameObject caseObj = Instantiate(suitcase, spawnPoint.position, Quaternion.identity);
                caseObj.GetComponent<Rigidbody2D>().linearVelocity = Random.insideUnitCircle * suitcaseVel;
            }
        }

        if (spawnTimer < -postSpawnDelay)
        {
            //End
            spawnTimer = preSpawnDelay;
            lastSpawnTimer = spawnTimer;
            GetNewState();
        }

        lastSpawnTimer = spawnTimer;

    }

    void Shockwaves()
    {
        Debug.Log("Shockwaves: " + shockwavePhase);
        shockwaveTimer -= Time.deltaTime;

        switch (shockwavePhase)
        {
            case ShockwavePhase.PreDelay:
                // Grounded, telegraphing the jump
                rb.linearVelocity = Vector2.zero;

                if (shockwaveTimer <= 0f)
                {
                    shockwavePhase = ShockwavePhase.Rising;
                    shockwaveTimer = bounceUpTime;
                }
                break;

            case ShockwavePhase.Rising:
                {
                    rb.linearVelocity = Vector2.zero;
                    float t = 1f - Mathf.Clamp01(shockwaveTimer / bounceUpTime);
                    float y = Mathf.Lerp(shockwaveGroundY, shockwaveGroundY + bounceHeight, t);
                    rb.MovePosition(new Vector2(transform.position.x, y));

                    if (shockwaveTimer <= 0f)
                    {
                        shockwavePhase = ShockwavePhase.Falling;
                        shockwaveTimer = bounceDownTime;
                    }
                }
                break;

            case ShockwavePhase.Falling:
                {
                    rb.linearVelocity = Vector2.zero;
                    float t = Mathf.Clamp01(shockwaveTimer / bounceDownTime);
                    float y = Mathf.Lerp(shockwaveGroundY, shockwaveGroundY + bounceHeight, t);
                    rb.MovePosition(new Vector2(transform.position.x, y));

                    if (shockwaveTimer <= 0f)
                    {
                        // Landed - snap to ground and fire off shockwaves both ways
                        rb.MovePosition(new Vector2(transform.position.x, shockwaveGroundY));
                        SpawnShockwave(-1);
                        SpawnShockwave(1);

                        bounces--;

                        if (bounces <= 0)
                        {
                            bounces = totalBounces;
                            shockwavePhase = ShockwavePhase.PreDelay;
                            shockwaveTimer = preBounceDelay;
                            GetNewState();
                            return;
                        }

                        shockwavePhase = ShockwavePhase.PostDelay;
                        shockwaveTimer = postLandDelay;
                    }
                }
                break;

            case ShockwavePhase.PostDelay:
                rb.linearVelocity = Vector2.zero;

                if (shockwaveTimer <= 0f)
                {
                    shockwavePhase = ShockwavePhase.PreDelay;
                    shockwaveTimer = preBounceDelay;
                }
                break;
        }
    }

    void SpawnShockwave(int dir)
    {
        GameObject wave = Instantiate(shockwavePrefab, new Vector3(transform.position.x, shockWaveY, 0f), Quaternion.identity);
        ShockwaveHitbox hitbox = wave.GetComponent<ShockwaveHitbox>();
        if (hitbox != null)
        {
            hitbox.Launch(dir, shockwaveSpeed, shockwaveLifetime);
        }
    }

    void Shooting()
    {
        rb.linearVelocity = Vector2.zero;
        shootTimer -= Time.deltaTime;

        if (shootTimer <= 0f)
        {
            if (shots > 0)
            {
                //Fire
                Vector3 spawnPos = shootPoint != null ? shootPoint.position : transform.position;
                Vector2 dir = ((Vector2)player.position - (Vector2)spawnPos).normalized;

                GameObject proj = Instantiate(projectile, spawnPos, Quaternion.identity);
                Destroy(proj, 8);

                Rigidbody2D projRb = proj.GetComponent<Rigidbody2D>();
                if (projRb != null)
                {
                    projRb.linearVelocity = dir * projectileSpeed;
                }

                // Optional: rotate the projectile to face the direction it's travelling.
                // Remove this line if your projectile sprite isn't drawn facing "right" by default.
                proj.transform.right = dir;

                shots--;
                shootTimer = shotInterval;
            }
            else if (shootTimer <= -postShootDelay)
            {
                //End
                shots = totalShots;
                shootTimer = preShootDelay;
                GetNewState();
            }
        }
    }

}