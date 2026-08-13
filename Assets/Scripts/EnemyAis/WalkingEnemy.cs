using UnityEngine;

public class WalkingEnemy : MonoBehaviour
{
    Rigidbody2D rb;

    public LayerMask groundLayer;

    public float speed;
    float dir;

    public float sideCastLength;
    public float groundCastLength;
    public Vector3 groundCastOffset;

    void Start()
    {
        dir = 1;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector3 directionGCastOffset = groundCastOffset;
        directionGCastOffset.x *= dir;

        // Check if we're standing on the ground
        bool grounded = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            0.65f,
            groundLayer
        );

        // Check for wall
        bool sideCast = Physics2D.Raycast(
            transform.position,
            Vector2.right * dir,
            sideCastLength,
            groundLayer
        );

        // Check for ground ahead
        bool groundCast = Physics2D.Raycast(
            transform.position + directionGCastOffset,
            Vector2.down,
            groundCastLength,
            groundLayer
        );

        if (grounded)
        {
            // Only walk when we're on the ground
            rb.linearVelocity = new Vector2(
                speed * dir,
                rb.linearVelocity.y
            );

            // Turn around at walls or platform edges
            if (!groundCast || sideCast)
            {
                dir *= -1;
            }
        }
        else
        {
            // We're airborne.
            // Stop horizontal movement and let gravity take over.
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );
        }
    }

    void OnDrawGizmos()
    {
        Vector3 directionGCastOffset = groundCastOffset;
        directionGCastOffset.x *= dir;

        // Wall ray
        Debug.DrawLine(
            transform.position,
            transform.position + Vector3.right * dir * sideCastLength
        );

        // Ground ahead ray
        Debug.DrawLine(
            transform.position + directionGCastOffset,
            transform.position + directionGCastOffset +
            Vector3.down * groundCastLength
        );

        // Grounded ray
        Debug.DrawLine(
            transform.position,
            transform.position + Vector3.down * 0.15f
        );
    }
}