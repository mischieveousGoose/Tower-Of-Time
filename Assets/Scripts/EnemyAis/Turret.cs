using UnityEngine;

public class Turret : MonoBehaviour
{
    public float shootTime;
    public float bulletSpeed;
    public GameObject projPref;
    Transform player;
    float shootTimer;
    public float spawnOffset;

    SpriteRenderer sr;


    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;


    }

    void Update()
    {
        if (player.position.x < transform.position.x)
        {
            sr.flipX = true;
        }
        else
        {
            sr.flipX = false;
        }
        shootTimer -= Time.deltaTime;
        if (shootTimer < 0f)
        {
            shootTimer = shootTime;
            Vector2 dir = (player.position - transform.position).normalized;
            float angle = Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x);
            GameObject proj = Instantiate(projPref, transform.position + (Vector3)(dir * spawnOffset), Quaternion.Euler(0, 0, angle));

            proj.GetComponent<Rigidbody2D>().linearVelocity = bulletSpeed * dir;

        }
    }
}
