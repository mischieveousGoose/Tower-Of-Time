using UnityEngine;

public class Damager : MonoBehaviour
{
    public float damage;
    public LayerMask attackLayer;
    public Vector2 size;

    public bool destroyOnContact = false;

    void OnCollisionEnter2D(Collision2D col)
    {
        if (destroyOnContact) Destroy(gameObject);
    }

    void Update()
    {
        Collider2D collider = Physics2D.OverlapBox(transform.position, size, 0, attackLayer);
        if (collider != null)
        {
            collider.GetComponent<Attackable>().TakeDamage(damage);
            if (destroyOnContact) Destroy(gameObject);
        }
    }
}
