using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    Transform player;
    public float speed;

    Rigidbody2D rb;
    SpriteRenderer sr;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
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
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * speed;

    }
}
