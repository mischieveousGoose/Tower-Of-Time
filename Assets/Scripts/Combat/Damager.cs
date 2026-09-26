using TreeEditor;
using UnityEngine;

public class Damager : MonoBehaviour
{
    public float damage;
    public LayerMask attackLayer;
    public Vector2 size;
    public Vector2 offset = Vector2.zero;

    public bool destroyOnContact = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (destroyOnContact) Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position + (Vector3)offset, size);
    }

    void Update()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position + (Vector3)offset, size, 0, attackLayer);
        if (collider != null)
        {
            collider.GetComponent<Attackable>().TakeDamage(damage);
            if (destroyOnContact) Destroy(gameObject);
        }
    }
}
