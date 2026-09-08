using UnityEngine;

public class ChargingEnemy : MonoBehaviour
{
    Transform playerTransform;
    Rigidbody2D rb;

    bool isCharging = false;

    public float chargeSpeed = 10f;
    public float chargeDistance = 4;

    void Start()
    {
        playerTransform = FindFirstObjectByType<Player>().transform;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (!isCharging)
        {
            if (Vector2.Distance(transform.position, playerTransform.position) < chargeDistance)
            {
                isCharging = true;
            }
            return;
        }

        Vector2 direction = playerTransform.position.x < transform.position.x ? Vector2.left : Vector2.right;
        rb.linearVelocity = direction * chargeSpeed;


    }
}
