using UnityEngine;

// Attach this to your shockwave prefab. CEOBoss calls Launch() on it
// right after spawning, once for the left wave and once for the right.
public class ShockwaveHitbox : MonoBehaviour
{
    float direction;
    float speed;

    public void Launch(float dir, float moveSpeed, float lifetime)
    {
        direction = Mathf.Sign(dir);
        speed = moveSpeed;

        Destroy(gameObject, lifetime);

        // Flip the sprite/collider to face the direction it's travelling, if it isn't symmetrical
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * direction;
        transform.localScale = scale;
    }

    void Update()
    {
        transform.position += Vector3.right * direction * speed * Time.deltaTime;
    }
}