
using UnityEngine;

public class WalkingEnemy : MonoBehaviour
{
    Rigidbody2D rb;
    public LayerMask groundLayer;

    public float speed;
    float dir;

    public float sideCastLength, groundCastLength;
    public Vector3 groundCastOffset;

    void Start()
    {
        dir = 1;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        rb.linearVelocity = Vector2.right * speed * dir;

        Vector3 directionGCastOffset = groundCastOffset;
        directionGCastOffset.x *= dir;

        bool sideCast = Physics2D.Raycast(transform.position, Vector2.right * dir, sideCastLength, groundLayer);
        bool groundCast = Physics2D.Raycast(transform.position + directionGCastOffset, Vector2.down, groundCastLength, groundLayer);

        if (!groundCast || sideCast)
        {
            dir *= -1;
        }
    }

    void OnDrawGizmos()
    {
        Vector3 directionGCastOffset = groundCastOffset;
        directionGCastOffset.x *= dir;

        Debug.DrawLine(transform.position, transform.position + (Vector3.right * dir * sideCastLength));
        Debug.DrawLine(transform.position + directionGCastOffset, (transform.position + directionGCastOffset) + (Vector3.down * groundCastLength));
    }
}
