using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    Transform player;
    public float speed;

    Rigidbody2D rb;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * speed;

    }
}
