using UnityEngine;

public class LeapingEnemy : MonoBehaviour
{
    Transform player;
    public float jumpTimer;
    float timer;
    public Vector2 jumpForce;

    Rigidbody2D rb;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = jumpTimer;
            Vector2 forceToAdd = jumpForce;
            forceToAdd.x *= player.position.x < transform.position.x ? -1 : 1;
            rb.linearVelocity = forceToAdd;
        }
    }
}
